using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigurationProviderInterfaceProject;

namespace Reflection2;

public abstract class ConfigurationComponentBase
{
    private static Dictionary<string, IConfigurationProvider> _providers = new ();
    public void SaveSettings()
    {
        foreach (var property in GetPropertiesWithConfigurationAttributes())
        {
            var attribute = property.GetCustomAttribute<ConfigurationItemAttribute>();
            var value = property.GetValue(this);

            var provider = GetProvider(attribute.ProviderType);
            provider.Save(attribute.SettingName, value);
        }
    }

    public void LoadSettings()
    {
        foreach(var property in GetPropertiesWithConfigurationAttributes())
        {
            var attribute = property.GetCustomAttribute<ConfigurationItemAttribute>();

            var provider = GetProvider(attribute.ProviderType);
            var value = provider.Load(property.PropertyType, attribute.SettingName);

            property.SetValue(this, value);
        }
    }

    private IConfigurationProvider GetProvider(string providerType)
    {
        if (!_providers.ContainsKey(providerType))
        {
            _providers[providerType] = LoadProvider(providerType);
        }
        return _providers[providerType];
    }

    private IConfigurationProvider LoadProvider(string providerType)
    {
        //Scan the plugins for the assemblies containing providers
        var pluginFolder = AppDomain.CurrentDomain.BaseDirectory + "Plugins";
        var pluginFiles = System.IO.Directory.GetFiles(pluginFolder, "*.dll");

        foreach (var dll in pluginFiles)
        {
            var assembly = Assembly.LoadFrom(dll);
            var providerTypeInAssembly = assembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && typeof(IConfigurationProvider).IsAssignableFrom(t));

            if (providerTypeInAssembly?.Name.Contains(providerType, StringComparison.OrdinalIgnoreCase) == true)
            {
                return (IConfigurationProvider)Activator.CreateInstance(providerTypeInAssembly);
            }
        }
        throw new InvalidOperationException($"provider type '{providerType}' not found");
    }

    private IEnumerable<PropertyInfo> GetPropertiesWithConfigurationAttributes()
    {
        return GetType().GetProperties()
            .Where(p => p.IsDefined(typeof(ConfigurationItemAttribute), false));
    }
}
