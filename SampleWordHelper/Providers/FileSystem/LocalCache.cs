using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SampleWordHelper.Core.IO;

namespace SampleWordHelper.Providers.FileSystem
{
    public class LocalCache
    {
        readonly Dictionary<string, string> keyToPathMap = new Dictionary<string, string>();
        readonly Dictionary<string, string> pathToKeyMap = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        readonly string cacheDirectory;
        readonly string catalogDirectory;
        readonly bool enabled;

        public LocalCache(string cacheDirectory, string catalogDirectory, bool enabled)
        {
            this.cacheDirectory = cacheDirectory;
            this.catalogDirectory = catalogDirectory;
            this.enabled = enabled;
        }

        public string TranslateFileName(string fullName)
        {
            if (!enabled)
                return fullName;
            var key = Guid.NewGuid().ToString();
            keyToPathMap.Add(key, fullName);
            pathToKeyMap.Add(fullName, key);
            return GetKeyFileName(key);
        }

        public void RemoveFile(string fullName)
        {
            if (!enabled)
                return;
            string key;
            if (!pathToKeyMap.TryGetValue(fullName, out key))
                return;
            pathToKeyMap.Remove(fullName);
            keyToPathMap.Remove(key);
        }

        public void Clear()
        {
            if (!enabled)
                return;
            foreach (var file in pathToKeyMap.Keys)
                LongPathFile.Delete(file);
        }

        public void Materialize()
        {
            if (!enabled)
                return;
            Clear();
            var files = keyToPathMap.ToArray();
            foreach (var pair in files)
            {
                if (!LongPathFile.Exists(pair.Value))
                {
                    RemoveFile(pair.Value);
                    continue;
                }
                LongPathFile.Copy(pair.Value, GetKeyFileName(pair.Key), true);
            }
        }

        /// <summary>
        /// Возвращает имя локального файла в кэше.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetKeyFileName(string key)
        {
            return Path.Combine(cacheDirectory, Path.ChangeExtension(key, ".docx"));
        }
    }
}
