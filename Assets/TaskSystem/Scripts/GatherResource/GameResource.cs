using System;
using System.Collections.Generic;

namespace TaskSystem.GatherResource
{
 


    public static class GameResource
    {
        public static Action OnGoldAmountChanged;
        public static Action OnWoodAmountChange;
        private static int goldAmount;
        private static Dictionary<ResourceType, int> resourceTypeDictionary = new Dictionary<ResourceType, int>();
        private static Dictionary<ResourceType, Action> resourceActionPairDictionary = new Dictionary<ResourceType, Action>();
        public enum ResourceType
        {
            Gold,
            Wood,
        }

        public static void Init()
        {
            foreach (ResourceType VARIABLE in System.Enum.GetValues(typeof(ResourceType)))
            {
                resourceTypeDictionary[VARIABLE] = 0;
            }

            resourceActionPairDictionary = new Dictionary<ResourceType, Action>
            {
                {ResourceType.Gold, OnGoldAmountChanged},
                {ResourceType.Wood, OnWoodAmountChange}
            };
        }
        public static void AddAmount(ResourceType resourceType,int amount)
        {
            resourceTypeDictionary[resourceType] += amount;
            resourceActionPairDictionary[resourceType]?.Invoke();
        }

        public static int GetAmount(ResourceType resourceType)
        {
            return resourceTypeDictionary[resourceType];
        }
    }
}