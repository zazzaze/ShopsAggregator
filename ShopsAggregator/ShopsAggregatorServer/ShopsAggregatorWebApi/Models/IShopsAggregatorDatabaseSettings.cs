using System;

namespace ShopsAggregatorWebApi.Models
{
    public interface IShopsAggregatorDatabaseSettings
    {
        public String UsersCollectionName { get; set; }
        public String ConnectionString { get; set; }
        public String DatabaseName { get; set; }
        
        public String PostsCollectionName { get; set; }
    }
}