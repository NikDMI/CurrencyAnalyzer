using System;
using ConfigLibrary.ConfigImpl;

namespace ConfigLibrary
{
    public class ConfigFactory
    {
        //Storage type of configurations
        public enum ConfigType { ASSEMBLY_MEMORY, XML };


        /// 
        /// Returns config object according to the type
        /// 
        /// <param name="configType">Storage config type</param>
        /// <returns></returns>
        public static IConfig GetConfig(ConfigType configType)
        {
            switch (configType)
            {
                case ConfigType.ASSEMBLY_MEMORY:
                    return _memoryConfig;

                case ConfigType.XML:
                    return null;

                default:
                    throw new ArgumentException("Not supported config types");
            }
        }


        /// 
        /// Type constructor (to initialize singelton objects)
        /// 
        static ConfigFactory()
        {
            _memoryConfig = new MemoryConfig();
        }

        private ConfigFactory() { }

        private static MemoryConfig _memoryConfig; //Singelton of memory configs
    }
}
