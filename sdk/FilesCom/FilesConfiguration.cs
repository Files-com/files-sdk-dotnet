using System;
using System.Configuration;
namespace FilesCom
{
    public class FilesConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("ApiKey")]
        public string ApiKey
        {
            get { return (string)this["ApiKey"]; }
            set { this["ApiKey"] = value; }
        }

        [ConfigurationProperty("SessionId")]
        public string SessionId
        {
            get { return (string)this["SessionId"]; }
            set { this["SessionId"] = value; }
        }

        [ConfigurationProperty("BaseUrl", DefaultValue = "https://app.files.com/")]
        public string BaseUrl
        {
            get { return (string)this["BaseUrl"]; }
            set { this["BaseUrl"] = value; }
        }

        [ConfigurationProperty("OpenTimeout", DefaultValue = 30.0D)]
        public double OpenTimeout
        {
            get { return (double)this["OpenTimeout"]; }
            set { this["OpenTimeout"] = value; }
        }

        [ConfigurationProperty("ReadTimeout", DefaultValue = 60.0D)]
        public double ReadTimeout
        {
            get { return (double)this["ReadTimeout"]; }
            set { this["ReadTimeout"] = value; }
        }

        [ConfigurationProperty("InitialNetworkRequestDelay", DefaultValue = 0.5D)]
        public double InitialNetworkRequestDelay
        {
            get { return (double)this["InitialNetworkRequestDelay"]; }
            set { this["InitialNetworkRequestDelay"] = value; }
        }

        [ConfigurationProperty("MaxNetworkRetries", DefaultValue = 3)]
        public int MaxNetworkRetries
        {
            get { return (int)this["MaxNetworkRetries"]; }
            set { this["MaxNetworkRetries"] = value; }
        }

        [ConfigurationProperty("MaxNetworkRetryDelay", DefaultValue = 2.0D)]
        public double MaxNetworkRetryDelay
        {
            get { return (double)this["MaxNetworkRetryDelay"]; }
            set { this["MaxNetworkRetryDelay"] = value; }
        }
    }
}