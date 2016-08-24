using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PokemonGo.RocketBot.Logic.Logging;

namespace PokemonGo.RocketBot.Window.Plugin
{
    /// <summary>
    ///     Plugin loader.
    /// </summary>
    internal class PluginManager
    {
        // Private vars.
        private readonly PluginInitializerInfo _initInfo;
        private readonly List<INecroPlugin> _plugins;
        private readonly Type _pluginType = typeof(INecroPlugin);


        /// <summary>
        ///     Plugin loader.
        /// </summary>
        /// <param name="initInfo"></param>
        public PluginManager(PluginInitializerInfo initInfo)
        {
            _initInfo = initInfo;
            _plugins = new List<INecroPlugin>();
        }


        /// <summary>
        ///     Load all the plugins found in the Necro root folder.
        /// </summary>
        public void InitPlugins()
        {
            // Get all the plugin DLLs.
            var pluginDir = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);

            var dllFiles = Directory.GetFiles(pluginDir, "*.dll");

            // Attempt to load all the assemblies from the DLLs.
            var assemblies = new List<Assembly>();
            foreach (var dll in dllFiles)
            {
                var assembly = AttemptAssemblyLoad(dll);
                if (assembly != null)
                    assemblies.Add(assembly);
            }

            // Attemp to load all plugins.
            LoadPlugins(assemblies);
        }


        /// <summary>
        ///     Attempt to load an assembly.
        /// </summary>
        /// <param name="dll">List of DLLs</param>
        /// <returns>Assembly if found, null if not.</returns>
        private Assembly AttemptAssemblyLoad(string dll)
        {
            try
            {
                var an = AssemblyName.GetAssemblyName(dll);
                var assembly = Assembly.Load(an);
                return assembly;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        ///     Load plugins from an assembly.
        /// </summary>
        /// <param name="assemblies">List of assemblies.</param>
        private void LoadPlugins(List<Assembly> assemblies)
        {
            // Get all the types from this assembly that match our plugin type.
            ICollection<Type> pluginTypes = new List<Type>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                            continue;

                        if (type.GetInterface(_pluginType.FullName) != null)
                            pluginTypes.Add(type);
                    }
                }
                catch
                {
                    Logger.Write("Could not load assembly: " + assembly.GetName().FullName, LogLevel.Error);
                }
            }

            // Iterate through them all and create an instance of the plugin.
            foreach (var type in pluginTypes)
            {
                try
                {
                    var plugin = (INecroPlugin) Activator.CreateInstance(type);
                    plugin.Initialize(_initInfo);
                    _plugins.Add(plugin);
                }
                catch
                {
                    Logger.Write("Could not load plugin: " + type.FullName, LogLevel.Error);
                }
            }
        }
    }
}