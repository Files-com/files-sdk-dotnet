using System.Configuration;
namespace Files
{
    public class FilesConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("ApiKey", IsRequired = true)]
        public string ApiKey
        {
            get { return (string) this["ApiKey"]; }
            set { this["ApiKey"] = value; }
        }

        [ConfigurationProperty("BaseUrl", DefaultValue = "https://app.files.com/")]
        public string BaseUrl
        {
            get { return (string) this["BaseUrl"]; }
            set { this["BaseUrl"] = value; }
        }

        [ConfigurationProperty("OpenTimeout", DefaultValue = 30.0D)]
        public double OpenTimeout
        {
            get { return (double) this["OpenTimeout"]; }
            set { this["OpenTimeout"] = value; }
        }

        [ConfigurationProperty("ReadTimeout", DefaultValue = 80.0D)]
        public double ReadTimeout
        {
            get { return (double) this["ReadTimeout"]; }
            set { this["ReadTimeout"] = value; }
        }

        [ConfigurationProperty("InitialNetworkRequestDelay", DefaultValue = 0.5D)]
        public double InitialNetworkRequestDelay
        {
            get { return (double) this["InitialNetworkRequestDelay"]; }
            set { this["InitialNetworkRequestDelay"] = value; }
        }

        [ConfigurationProperty("MaxNetworkRetries", DefaultValue = 3)]
        public int MaxNetworkRetries
        {
            get { return (int) this["MaxNetworkRetries"]; }
            set { this["MaxNetworkRetries"] = value; }
        }

        [ConfigurationProperty("MaxNetworkRetryDelay", DefaultValue = 2.0D)]
        public double MaxNetworkRetryDelay
        {
            get { return (double) this["MaxNetworkRetryDelay"]; }
            set { this["MaxNetworkRetryDelay"] = value; }
        }
    }
}
