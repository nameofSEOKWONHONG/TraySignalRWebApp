using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Logging;
using PluginLib;

namespace TrayBackgroundApp;

public class PluginLoader
{
    public List<McMaster.NETCore.Plugins.PluginLoader> Loaders { get; }
    private readonly ILogger _logger;
    public PluginLoader(ILogger<PluginLoader> logger)
    {
        _logger = logger;
        Loaders = new List<McMaster.NETCore.Plugins.PluginLoader>();
    }
    
    public void LoadFromDir(Type[] sharedInterfaceTypes, string pluginDirName = "plugins")
    {
        // create plugin loaders
        var pluginsDir = Path.Combine(AppContext.BaseDirectory, pluginDirName);
        foreach (var dir in Directory.GetDirectories(pluginsDir))
        {
            var dirName = Path.GetFileName(dir);
            var pluginDll = Path.Combine(dir, dirName + ".dll");
            if (File.Exists(pluginDll))
            {
                var loader = McMaster.NETCore.Plugins.PluginLoader.CreateFromAssemblyFile(
                    pluginDll, 
                    true,
                    sharedInterfaceTypes,
                    config => config.EnableHotReload = true
                    );
                    
                loader.Reloaded += (s, e) =>
                {
                    _logger.Log(LogLevel.Information, $"{e.Loader.GetType().FullName} is reloaded.");
                };
                Loaders.Add(loader);
            }
        }
    }

    public T ActivateInstance<T>()
    {
        foreach (var loader in this.Loaders)
        {
            foreach (var pluginType in loader
                         .LoadDefaultAssembly()
                         .GetTypes()
                         .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract))
            {
                // This assumes the implementation of IPlugin has a parameterless constructor
                return (T)Activator.CreateInstance(pluginType);
            }
        }

        return default;
    }
}