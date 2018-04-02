using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JsonTranslationEditor.JsonHelper;

namespace JsonTranslationEditor
{
    public class AppOptions
    {
        public SaveStyles SaveStyle { get; set; }
        public string DefaultPath { get; set; }
        public int PageSize { get; set; }

        static string path = System.IO.Path.Combine(Environment.GetFolderPath(
               Environment.SpecialFolder.MyDoc‌​uments), "JsonTranslationEditor");


        public static AppOptions FromDisk()
        {
            if (File.Exists(Path.Combine(path, "settings.json")))
            {
                var loaded = File.ReadAllText(Path.Combine(path, "settings.json"));
                var options = JsonConvert.DeserializeObject<AppOptions>(loaded);

                if (options.PageSize == 0)
                    options.PageSize = 50;

                return options;

            }

            return new AppOptions() { SaveStyle = SaveStyles.Json, PageSize = 50 };

        }
        public void ToDisk()
        {

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText(Path.Combine(path, "settings.json"), json);

        }
    }
}
