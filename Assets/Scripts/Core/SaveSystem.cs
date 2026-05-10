using System;
using System.IO;
using Data;
using UnityEngine;

namespace Core
{
    public static class SaveSystem
    {
        private const int CurrentVersion = 1;
        private const string FileName = "joker_save.json";
        private const string BackupSuffix = ".bak";

        private static SaveData _data = new() { version = CurrentVersion };
        private static string FilePath       => Path.Combine(Application.persistentDataPath, FileName);
        private static string BackupFilePath => FilePath + BackupSuffix;

        private static bool _dirty;
        private static float _lastFlushUnscaledTime;
        private const float FlushIntervalSeconds = 0.25f;

        public static bool IsLoaded { get; private set; }

        public static bool IsFirstLaunch { get; private set; } = true;

        public static int LastBalance => _data.balance;
        public static PlayerStatsSnapshot LastStats => _data.stats.ToSnapshot();
        public static RouletteVariant LastVariant => (RouletteVariant)_data.variant;
        public static float MusicVolume => _data.musicVolume;
        public static float SfxVolume   => _data.sfxVolume;


        public static void Load()
        {
            if (!TryLoadFile(FilePath))
            {
                if (File.Exists(BackupFilePath))
                {
                    TryLoadFile(BackupFilePath);
                }
            }

            IsLoaded = true;
            EditorApplicationHook.Init();
        }

        private static bool TryLoadFile(string path)
        {
            try
            {
                if (!File.Exists(path)) return false;
                var json = File.ReadAllText(path);
                var parsed = JsonUtility.FromJson<SaveData>(json);
                if (parsed == null) return false;
                _data = Migrate(parsed);
                IsFirstLaunch = false;
                return true;
            }
            catch (Exception e)
            {
                _data = new SaveData { version = CurrentVersion };
                return false;
            }
        }

        public static void WipeAll()
        {
            _data = new SaveData { version = CurrentVersion };
            try
            {
                if (File.Exists(FilePath)) File.Delete(FilePath);
                if (File.Exists(BackupFilePath)) File.Delete(BackupFilePath);
            }
            catch (Exception e)
            {

            }

            IsFirstLaunch = true;
            IsLoaded = false;
            _dirty = false;
        }


        public static int ReadWalletBalance(int fallback)
            => IsFirstLaunch ? fallback : _data.balance;

        public static void WriteWalletBalance(int balance)
        {
            _data.balance = balance;
            Persist();
        }


        public static RouletteVariant ReadVariant(RouletteVariant fallback)
            => IsFirstLaunch ? fallback : (RouletteVariant)_data.variant;

        public static void WriteVariant(RouletteVariant v)
        {
            _data.variant = (int)v;
            Persist();
        }


        public static void WriteVolumes(float music, float sfx)
        {
            _data.musicVolume = Mathf.Clamp01(music);
            _data.sfxVolume = Mathf.Clamp01(sfx);
            Persist();
        }


        public static PlayerStatsSnapshot ReadStats() => _data.stats.ToSnapshot();

        public static void WriteStats(int spins, int won, int lost,
                                       int wagered, int returned,
                                       int biggestWin, int biggestLoss, int streak)
        {
            _data.stats = new StatsBlock
            {
                spins = spins, won = won, lost = lost,
                wagered = wagered, returned = returned,
                biggestWin = biggestWin, biggestLoss = biggestLoss, streak = streak
            };
            Persist();
        }


        private static void Persist()
        {
            _dirty = true;
            Flush();
        }

        public static void Flush()
        {
            if (!_dirty) return;
            try
            {
                _data.version = CurrentVersion;
                if (File.Exists(FilePath))
                    File.Copy(FilePath, BackupFilePath, overwrite: true);
                File.WriteAllText(FilePath, JsonUtility.ToJson(_data, prettyPrint: true));
                IsFirstLaunch = false;
                _dirty = false;
                _lastFlushUnscaledTime = Time.realtimeSinceStartup;
            }
            catch (Exception e)
            {

            }
        }

        internal static void TickFlush()
        {
            if (!_dirty) return;
            if (Time.realtimeSinceStartup - _lastFlushUnscaledTime < FlushIntervalSeconds) return;
            Flush();
        }

        private static SaveData Migrate(SaveData loaded)
        {
            if (loaded.version < CurrentVersion)
                loaded.version = CurrentVersion;
            return loaded;
        }


        [Serializable]
        private class SaveData
        {
            public int version;
            public int balance;
            public int variant;
            public float musicVolume = 0.5f;
            public float sfxVolume = 0.8f;
            public StatsBlock stats = new();
        }

        [Serializable]
        private class StatsBlock
        {
            public int spins, won, lost, wagered, returned;
            public int biggestWin, biggestLoss, streak;

            public PlayerStatsSnapshot ToSnapshot()
                => new(spins, won, lost, wagered, returned, biggestWin, biggestLoss, streak);
        }
    }
}
