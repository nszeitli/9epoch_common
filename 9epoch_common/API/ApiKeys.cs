using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace _9epoch_common.API
{
    public class ApiKeys
    {
        public KeyStore KeyStore;

        public ApiKeys(string path = "keys.json")
        {
            try
            {
                string json = System.IO.File.ReadAllText(path);
                KeyStore = JsonConvert.DeserializeObject<KeyStore>(json);
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }
    }

    public class KeyStore
    {
        public string ScraperKey;
        public string TwitterKey;
        public string _9epochKey;
    }
}
