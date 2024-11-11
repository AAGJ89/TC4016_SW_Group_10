namespace WebAPI_SmartInventory.Data
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public Dictionary<string, string> Collections { get; set; }
    }
}
