using System.Collections;
using Betting;
using Core;
using Data;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace JokerTests.PlayMode
{
    [TestFixture]
    [Category("Integration")]
    public class FullSpinIntegrationTest
    {
        [SetUp] public void SetUp() => SaveSystem.WipeAll();
        [TearDown] public void TearDown() => SaveSystem.WipeAll();

        [UnityTest]
        public IEnumerator FullRound_LandsOnDeterministicNumber()
        {
            yield return SceneManager.LoadSceneAsync("Game");
            yield return null;

            Assert.IsNotNull(GameManager.Instance, "GameManager.Instance not initialised");
            var gm = GameManager.Instance;

            int startingBalance = gm.Wallet.Balance;
            Assert.IsTrue(startingBalance > 0, "Wallet should be seeded");

            Assert.IsTrue(gm.Bets.TryPlaceBet(new Bet(BetCategory.Straight, 17, 0, 10)));
            Assert.AreEqual(GameState.Betting, gm.State);

            gm.SetDeterministicNext(17);
            gm.Spin();

            float timeout = 15f;
            while (gm.State != GameState.Betting && timeout > 0f)
            {
                timeout -= UnityEngine.Time.unscaledDeltaTime;
                yield return null;
            }

            Assert.AreEqual(GameState.Betting, gm.State, "Spin did not return to Betting in time");
            Assert.AreEqual(17, gm.Spinner.LastWinningNumber);
            Assert.IsTrue(gm.Spinner.LastWasDeterministic);

            Assert.AreEqual(startingBalance + 350, gm.Wallet.Balance,
                "Wallet should reflect 35:1 payout on a straight win");
        }
    }
}
