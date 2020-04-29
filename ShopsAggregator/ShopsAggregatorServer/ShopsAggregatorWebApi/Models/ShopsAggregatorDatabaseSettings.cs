namespace ShopsAggregatorWebApi.Models
{
    public class ShopsAggregatorDatabaseSettings : IShopsAggregatorDatabaseSettings
    {
        public string UsersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string PostsCollectionName { get; set; }
    }
}