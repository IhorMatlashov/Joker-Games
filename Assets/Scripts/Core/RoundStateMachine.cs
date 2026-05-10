using UnityEngine;

namespace Core
{
    public sealed class RoundStateMachine
    {
        public GameState State { get; private set; } = GameState.Betting;

        public bool TryTransition(GameState next)
        {
            if (next == State) return false;

            bool allowed =
                (State == GameState.Betting   && next == GameState.Spinning)  ||
                (State == GameState.Spinning  && next == GameState.Resolving) ||
                (State == GameState.Resolving && next == GameState.Betting);

            if (!allowed)
            {
                return false;
            }

            var prev = State;
            State = next;
            EventBus.RaiseGameStateChanged(next);
            return true;
        }
    }
}
