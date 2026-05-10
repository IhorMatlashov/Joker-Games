# Deterministic Roulette — Unity Case Study

A 3D single-player Unity prototype of European/American roulette with a forced-outcome dropdown for testing, persistent wallet + lifetime stats, full payout math, an audio mixer, and polished VFX/SFX.

Built for the Joker Games Unity Developer case (May 2026, Unity 6000.0.X).

---

## Table of contents

- [Setup & running](#setup--running)
- [Controls & gameplay](#controls--gameplay)
- [Round flow](#round-flow)
- [Architecture overview](#architecture-overview)
- [Design patterns used](#design-patterns-used)
- [Folder layout](#folder-layout)
- [Persistence](#persistence)
- [Tests](#tests)
- [Bonus features completed](#bonus-features-completed)
- [Known issues / future improvements](#known-issues--future-improvements)
- [Demo video](#demo-video)

---

## Setup & running

1. **Unity version:** `6000.3.10f1` (any 6000.0.X-compatible release)
2. Open the project, then open `Assets/Scenes/Splash.unity`
3. Press **Play**. The flow is `Splash → Main Menu → Game`

Build settings already include the three scenes in order: Splash (0), Main Menu (1), Game (2).

To wipe persisted state during testing: **Reset Session** button in-game, or call `SaveSystem.WipeAll()`.

---

## Controls & gameplay

### Main Menu
| Action | Effect |
|---|---|
| **Play** | Opens the variant picker (European / American) |
| **Statistics** | Lifetime stats (spins, wins, losses, win rate, biggest win/loss, streak) |
| **Quit** | Exits the application |

### Game scene
| Action | Effect |
|---|---|
| **Click chip button** | Selects the chip denomination |
| **Left-click bet cell** | Places one chip of the selected denomination (debits the wallet immediately) |
| **Right-click bet cell** | Refunds the most recent chip on that cell only |
| **CLEAR BETS** | Refunds every active bet |
| **Deterministic dropdown** | Force the next spin to land on a specific number. Selection is **sticky** — every spin returns the chosen number until you pick "Random". `00` only available on American |
| **Variant toggle** | European ⇄ American. Locked while a spin is in flight |
| **Settings** | Music + SFX sliders. Drags apply in real time via the AudioMixer (no need to wait for the next clip) |
| **SPIN** | Starts the round. Disabled until at least one bet is placed |
| **Reset Session** | Clears bets, restores starting balance, wipes lifetime stats |
| **MENU** | Returns to Main Menu |

There are no keyboard shortcuts — the prototype is mouse-only.

### Bet types
- **Inside:** Straight (35:1), Split (17:1), Street (11:1), Corner (8:1), Six Line (5:1)
- **Outside:** Red/Black, Even/Odd, High/Low (1-18 / 19-36) — 1:1
- **Group:** Dozens (1st / 2nd / 3rd 12), Columns — 2:1

> The current table prefab exposes Straight, outside bets, dozens, columns and 0/00 cells. Split/Street/Corner/Six Line are fully implemented in `Bet.cs` and `BetManager.Resolve` — extending the UI is a layout-only pass.

---

## Round flow

```
Betting ──[player presses SPIN, has bets]──► Spinning
                                                │
                                            ball + wheel coroutines
                                            (ball uses one of 10 SpinProfiles)
                                                │
Spinning ──[wheel + ball settle]──► Resolving ──► Betting (loop)
              │
              └─► BetManager.Resolve → PlayerStats.Record → EventBus.RaiseRoundResolved
```

`RoundStateMachine` enforces the legal transitions; out-of-order calls log a warning and are ignored.

---

## Architecture overview

```
                  ┌──────────────┐
                  │  EventBus    │  static publish/subscribe
                  └──────┬───────┘
                         │
   ┌─────────────────────┼─────────────────────────────┐
   │                     │                             │
┌──▼───────┐    ┌────────▼──────┐    ┌─────────────────▼─────┐
│   UI     │    │  GameManager  │    │  Audio / VFX          │
│ (HUD,    │◄───┤  (composition │───►│  (subscribed listeners│
│  bets,   │    │   root +      │    │   - never queried)    │
│  banner) │    │   facade)     │    └───────────────────────┘
└──────────┘    └────┬──────────┘
                     │ owns
        ┌────────────┼─────────────────┐
        │            │                 │
   ┌────▼────┐  ┌────▼─────┐    ┌──────▼──────────┐
   │ Wallet  │  │ Stats    │    │  BetManager     │
   └────┬────┘  └────┬─────┘    └─────────────────┘
        │           │
        └─────┬─────┘
              │ persisted via
        ┌─────▼─────────┐
        │  SaveSystem   │   single source of truth (JSON @ persistentDataPath)
        └───────────────┘
```

### Composition root + DI

`GameManager` is the only composition root. In `Awake` it builds the core models (`PlayerWallet`, `PlayerStats`, `BetManager`, picks the variant). In `Start` it constructs a `GameContext` snapshot and pushes it into every component that implements `IGameInitializable`:

```csharp
private void Start()
{
    var ctx = new GameContext(config, Wallet, Stats, Bets, Variant, State, chipSelector, spinner);
    foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        if (mb is IGameInitializable initable) initable.Initialize(ctx);
}
```

This sidesteps `[DefaultExecutionOrder]`. `Start` runs after every `Awake`/`OnEnable` regardless of script load order, so the push is always order-safe. Adding a new UI component that needs startup state is one line: implement the interface.

### Why an EventBus

UI, audio, VFX, the stats panel and HUD widgets all need to react to the same gameplay events (bet placed, spin started, round resolved, balance changed, variant switched). Wiring direct references between them creates a tangle; the static `EventBus` decouples producers from consumers. `GameManager` doesn't know `WinCelebrationVFX` exists, and `WinCelebrationVFX` doesn't know about `GameManager`. They meet at an event signature.

### Audio routing

A single `MainMixer.mixer` with **Master → Music + SFX** groups. Both volume parameters are exposed and driven by `AudioManager.ApplySavedVolumes` via `mixer.SetFloat(name, LinearToDb(linear))`. Slider drags apply to in-flight audio — mixer parameters are read every audio frame, unlike a per-call `PlayOneShot` `volumeScale` that bakes in at trigger time.

`BackgroundMusic` (poker-room loop) outputs to **Music**; spin loop also outputs to **Music**; one-shots (chip place, fret clack, ball roll, win/lose, ball land) output to **SFX**.

### Spin animation

The spinner picks one of 10 designer-authored `SpinAnimationProfileSo` profiles per round (Classic, LongHang, QuickSnap, Bouncy, Wobbly, …). Each profile holds:

- duration
- sweep / radius / height curves driving the ball orbit
- fret-hit times driving fret-clack audio

`RouletteBall.OrbitTo` reads the curves to lerp angle/radius/height; the wheel runs its own coroutine in parallel. The spinner emits `OnSpinStarted`/`OnSpinEnded`, which audio + VFX subscribe to.

---

## Design patterns used

| Pattern | Where | Why |
|---|---|---|
| **Composition root + property injection (DI)** | `GameManager.Start` + `IGameInitializable` + `GameContext` | Order-safe init without execution-order hacks |
| **Singleton (Facade)** | `GameManager.Instance`, `AudioManager.Instance`, `ChipPool.Instance` | One-of-a-kind scene services with a stable entry point |
| **Observer (pub/sub)** | `EventBus` (`OnSpinStarted`, `OnRoundResolved`, `OnBalanceChanged`, …) | Decouples gameplay producers from UI/audio/VFX consumers |
| **State Machine** | `RoundStateMachine` (`Betting → Spinning → Resolving → Betting`) | Enforces legal round flow |
| **Strategy** | `DeterministicOutcomeProvider.Roll` | Swappable outcome source — RNG vs forced number |
| **Object Pool** | `ChipPool` | Reuses 3D chip GameObjects across rounds, avoids GC pressure during dense betting |
| **Builder (light)** | `BetSpotConfig.Build(amount)` | Authoring data → domain `Bet` instance |
| **ScriptableObject as data container** | `GameConfigSo`, `AudioLibrarySo`, `ChipDenominationSo`, `SpinAnimationProfileSo` | Designer-authored, runtime-readonly configs |

---

## Folder layout

```
Assets/
├─ Audio/                MainMixer.mixer + clips (Music, Roulette, Chips, Win)
├─ Layer Lab/            third-party UI button sprites + font (allowed per case rules — no code)
├─ Models/               FBX wheel + chip models
├─ Mateials/             wheel + table materials (typo on disk; preserved to keep refs intact)
├─ Prefabs/              Table, BettingTable, chip prefabs
├─ Shaders/              custom roulette wheel shader
├─ SOs/                  GameConfig, AudioLibrary, ChipDenominations, SpinProfiles
├─ Scenes/               Splash, MainMenu, Game
├─ Scripts/
│  ├─ Audio/             AudioManager, AudioLibrarySo
│  ├─ Betting/           Bet, BetCategory, BetManager, BetSpotConfig
│  ├─ Core/              GameManager, GameContext, IGameInitializable,
│  │                     EventBus, RoundStateMachine, SaveSystem,
│  │                     EditorApplicationHook
│  ├─ Data/              snapshots, configs, enums, WheelLayout
│  ├─ Menu/              MainMenuController, SplashController, BackToMenuButton
│  ├─ Player/            PlayerWallet, PlayerStats
│  ├─ Roulette/          RouletteSpinner, RouletteWheel, RouletteBall,
│  │                     RouletteVariantSwitcher,
│  │                     DeterministicOutcomeProvider,
│  │                     RouletteWheelTextureRotator,
│  │                     SpinAnimationProfileSo
│  ├─ UI/                BetSpot, BettingTableController, ChipButton,
│  │                     ChipSelectorUI, ChipPool, WalletUI, WagerUI,
│  │                     StatsPanelUI, SpinButtonUI, ClearBetsButtonUI,
│  │                     VariantToggleUI, VariantIndicatorUI,
│  │                     DeterministicSelectorUI, ResetSessionButtonUI,
│  │                     SettingsPanelUI, BannerUI, BetResultDisplayUI,
│  │                     BankruptcyHandler
│  └─ VFX/               CameraShake, PulseLightOnWin, WinCelebrationVFX,
│                        WinningPocketHighlight
├─ Settings/             URP / quality / input configs
└─ Tests/                EditMode + PlayMode tests
```

---

## Persistence

All saves go through [`SaveSystem`](Assets/Scripts/Core/SaveSystem.cs) — a static layer over `JsonUtility` writing to `Application.persistentDataPath/joker_save.json`:

```json
{
  "version": 1,
  "balance": 867,
  "variant": 0,
  "musicVolume": 0.5,
  "sfxVolume": 0.8,
  "stats": { "spins": 12, "won": 4, "lost": 8, "wagered": 220, "returned": 175, "biggestWin": 70, "biggestLoss": 25, "streak": -2 }
}
```

- Every `Write*` call writes immediately. Atomic: existing save is copied to `.bak` first, then the new file is written. On read, `.bak` is the fallback if the primary is corrupt.
- A hidden `EditorApplicationHook` GameObject is created lazily on first load to flush on `OnApplicationPause` / `OnApplicationQuit` as a safety net.
- `IsFirstLaunch` is derived from whether a save file exists, not stored.
- Persisted: wallet balance, lifetime stats, last-picked variant, music/SFX volumes.

---

## Tests

Open **Window → General → Test Runner**.

- `BetTests`, `BetManagerTests`, `PlayerWalletTests` — pure C# unit tests for payout math, wallet debit/credit, bet lifecycle.
- `WheelLayoutTests`, `DeterministicOutcomeProviderTests`, `RoundStateMachineTests` — domain rules and state transitions.
- `FullSpinIntegrationTest` (PlayMode) — loads the Game scene, places a $10 straight on 17, forces 17 as outcome, asserts the wallet ends at `start + 350` (35:1 payout net).

55 tests total; all green at last run.

---

## Bonus features completed

- ✅ European + American variants (toggle in Main Menu and in-game).
- ✅ Auto-save on every state-changing event; resume on next launch.
- ✅ Persistent stats, variant, volumes across sessions.
- ✅ AudioMixer routing — slider drags rescale in-flight clips.
- ✅ Background music + spin loop both routed through the **Music** mixer group.

---

## Known issues / future improvements

- **No balance change at loss-resolve.** Chips are debited at placement (casino-correct), so on a losing spin the wallet number doesn't move at the moment of resolution — only the result banner reflects the loss. A brief negative-flash on the wallet label would help clarity.
- **`BetResultDisplayUI` lacks an entrance animation.** The text just toggles active; a pop-in / fade-in would feel better.
- **Chip stacking is visual-only.** Past `GameConfigSo.maxStackedChipsPerSpot` (default 25), additional chips on the same spot still count toward the bet but don't render as a tower. The chip-marker label always shows the true total. Casino-style "swap N small chips for one bigger denomination chip" would look better than a flat ceiling.
- **Inside bet UI is partial.** The math handles Split/Street/Corner/Six Line — only the table authoring is missing. Adding spots + `BetSpotConfig` entries extends it.
- **Single deterministic shot.** Can only force the *next* spin. A queue of forced outcomes for scripted demo sequences would be useful.
- **No undo for an in-flight spin.** Once SPIN is pressed, bets are locked. Right-click refund only works pre-spin.
- **Settings panel is minimal.** Volume sliders only — no quality, language, or rebind UI.
- **Variant toggle disabled mid-spin without explanation.** Tooltip / disabled-reason text would be friendlier.
- **No localization.** All strings hardcoded in English.

---

## Demo video

https://drive.google.com/file/d/1f2Mr0XV5vl01TSHHMYcFzTvw6qO2PAE1/view?usp=drive_link

---

## Tech stack

- Unity **6000.3.10f1** (URP)
- TextMeshPro for all UI text
- Unity Test Framework (NUnit) for the domain test suite under `Assets/Tests/`
- Unity AudioMixer for music/SFX routing

No third-party code, plugins, or SDKs (no DoTween). The only third-party content is `Layer Lab` UI button sprites + font — sprites/textures are explicitly allowed by the case ("All assets like textures, audio and 3D models can be sourced from the internet").
