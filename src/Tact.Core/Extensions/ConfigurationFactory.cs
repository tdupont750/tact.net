using Tact.Configuration;

namespace Tact
{
    public static class ConfigurationFactoryExtesions
    {
        public static T Resolve<T>(this IConfigurationFactory configurationFactory)
        {
            var type = typeof(T);
            return (T) configurationFactory.CreateObject(type);
        }
    }
}
