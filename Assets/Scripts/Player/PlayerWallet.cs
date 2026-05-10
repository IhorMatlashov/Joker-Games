using Core;

namespace Player
{
    public sealed class PlayerWallet
    {
        public int Balance { get; private set; }

        public PlayerWallet(int startingBalance)
        {
            Balance = SaveSystem.IsFirstLaunch
                ? startingBalance
                : SaveSystem.ReadWalletBalance(startingBalance);
            if (SaveSystem.IsFirstLaunch) SaveSystem.WriteWalletBalance(Balance);
        }

        public bool TryDebit(int amount)
        {
            if (amount <= 0 || amount > Balance) return false;
            Set(Balance - amount);
            return true;
        }

        public void Credit(int amount)
        {
            if (amount <= 0) return;
            Set(Balance + amount);
        }

        public void SetBalance(int amount) => Set(amount < 0 ? 0 : amount);

        private void Set(int next)
        {
            int prev = Balance;
            Balance = next;
            SaveSystem.WriteWalletBalance(next);
            if (prev != next) EventBus.RaiseBalanceChanged(prev, next);
        }
    }
}
