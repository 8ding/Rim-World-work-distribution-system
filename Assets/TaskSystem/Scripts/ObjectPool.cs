namespace TaskSystem
{
    public class ObjectPool
    {
        private static ObjectPool instance;

        public static ObjectPool Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ObjectPool();
                }
                return instance;
            }
        }
    }
}