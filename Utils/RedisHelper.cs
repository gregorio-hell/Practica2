using StackExchange.Redis;

namespace pc2.Utils
{
    public static class RedisHelper
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect("localhost:6379");
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public static IDatabase GetDatabase()
        {
            return Connection.GetDatabase();
        }

        public static bool TestConnection()
        {
            try
            {
                var db = GetDatabase();
                return db.Ping().TotalSeconds < 1;
            }
            catch
            {
                return false;
            }
        }
    }
}