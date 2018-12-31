namespace Xemio.Logic.Configuration
{
    public class DatabaseConfiguration
    {
        public string Urls { get; set; }
        public string DatabaseName { get; set; }
        public bool UseEmbeddedTestServer { get; set; }
        public bool CreateRandomDatabaseNameForEmbeddedUsage { get; set; }
    }
}