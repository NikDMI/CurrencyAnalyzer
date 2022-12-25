﻿using System;
using ConfigLibrary.ConfigImpl;

namespace ConfigLibrary
{
    public class ConfigFactory
    {
        public enum ConfigType { ASSEMBLY_MEMORY, XML };


        public IConfig GetConfig(ConfigType configType)
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


        static ConfigFactory()
        {
            _memoryConfig = new MemoryConfig();
        }

        private ConfigFactory() { }

        private static MemoryConfig _memoryConfig; //Singelton of memory configs
    }
}
