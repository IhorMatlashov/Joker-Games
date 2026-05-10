using Core;
using NUnit.Framework;

namespace JokerTests
{
    [TestFixture, Category("Domain")]
    public class RoundStateMachineTests
    {
        [Test]
        public void Default_state_is_Betting()
        {
            var sm = new RoundStateMachine();
            Assert.AreEqual(GameState.Betting, sm.State);
        }

        [Test]
        public void Allows_full_legal_cycle()
        {
            var sm = new RoundStateMachine();
            Assert.IsTrue(sm.TryTransition(GameState.Spinning));
            Assert.IsTrue(sm.TryTransition(GameState.Resolving));
            Assert.IsTrue(sm.TryTransition(GameState.Betting));
        }

        [Test]
        public void Rejects_skipping_states()
        {
            var sm = new RoundStateMachine();
            Assert.IsFalse(sm.TryTransition(GameState.Resolving));
            Assert.AreEqual(GameState.Betting, sm.State);
        }

        [Test]
        public void Rejects_same_state()
        {
            var sm = new RoundStateMachine();
            Assert.IsFalse(sm.TryTransition(GameState.Betting));
        }

        [Test]
        public void Rejects_backward_transitions()
        {
            var sm = new RoundStateMachine();
            sm.TryTransition(GameState.Spinning);
            Assert.IsFalse(sm.TryTransition(GameState.Betting));
            Assert.AreEqual(GameState.Spinning, sm.State);
        }
    }
}
