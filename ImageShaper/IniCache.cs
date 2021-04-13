using System;
using System.Collections.Generic;
using System.IO;

namespace ImageShaper
{
    public class IniCache
    {
        private readonly Dictionary<string, IniFile> _cache;

        public IniCache()
        {
            _cache =  new Dictionary<string, IniFile>();
        }

        ~IniCache()
        {
            
        }

        public IniFile GetOrOpenOrCreate(string filename)
        {
            if (_cache.ContainsKey(filename))
                return _cache[filename];
            if (!File.Exists(filename))
            {
                var iniFile = new IniFile();
                _cache.Add(filename, iniFile);
                return iniFile;
            }
            var fs = File.Open(filename, FileMode.Open);
            var ini = new IniFile(fs);
            fs.Close();
            _cache.Add(filename, ini);
            return ini;
        }

        public void Save(string filename)
        {
            if (!_cache.ContainsKey(filename))
                return;
            if (File.Exists(filename))
                File.Delete(filename);
            var fs = File.Open(filename, FileMode.Create); 
            _cache[filename].WriteToStream(fs);
            fs.Close();
        }
        
        public void Close(string filename)
        {
            if (_cache.ContainsKey(filename))
            {
                _cache.Remove(filename);
            }
        }
    }
}