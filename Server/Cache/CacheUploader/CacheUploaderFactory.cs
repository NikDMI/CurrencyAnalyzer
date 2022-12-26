using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Cache.CacheUploader
{
    internal static class CacheUploaderFactory
    {
        public enum CacheUploadersType { FILE_JSON };

        public static ICacheUploader GetCacheUploader(CacheUploadersType cacheUploaderType)
        {
            switch (cacheUploaderType)
            {
                case CacheUploadersType.FILE_JSON:
                    return new JsonCacheUploader(FILE_NAME);

                default:
                    throw new ArgumentException("Not supported cache uploader");
            }
        }

        private const string FILE_NAME = "CurrenciesJson.json";
    }
}
