namespace Demo.Rpc.Configuration
{
    public static class Constants
    {
        private const string ConfigPathPrefix = "Tact.Rpc.";

        public const string DefaultsConfigKey = "DefaultValues";

        public const string ServiceConfigPathPrefix = ConfigPathPrefix + "Services";

        public const string ServiceConfigDefaults = ServiceConfigPathPrefix + "." + DefaultsConfigKey;

        public const string HostsConfigPathPrefix = ConfigPathPrefix + "Hosts";

        public const string HostsConfigDefaults = HostsConfigPathPrefix + "." + DefaultsConfigKey;
    }
}
