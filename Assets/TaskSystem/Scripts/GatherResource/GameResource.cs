using System;

namespace TaskSystem.GatherResource
{
    public static class GameResource
    {
        public static Action OnGoldAmountChanged;
        private static int goldAmount;

        public static void AddAmount(int amount)
        {
            goldAmount += amount;
            OnGoldAmountChanged?.Invoke();
        }

        public static int GetGoldAmount()
        {
            return goldAmount;
        }
    }
}