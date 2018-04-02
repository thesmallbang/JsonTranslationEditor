using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JsonTranslationEditor
{
    public class JsonHelper
    {

        public List<LanguageSetting> Load(string folder)
        {
            var files = System.IO.Directory.GetFiles(folder, "*.json");
            var settings = new List<LanguageSetting>();

            foreach (var filePath in files)
            {
                var newFiles = new List<LanguageSetting>();
                var file = System.IO.Path.GetFileName(filePath);
                var language = file.Replace(".json", "");

                var content = string.Join(Environment.NewLine, System.IO.File.ReadAllLines(filePath));
                FromNestMethod(newFiles, language, content);
                if (!newFiles.Any())
                    newFiles.AddRange(new LanguageSetting[] { new LanguageSetting() { Language = language } });
                settings.AddRange(newFiles);
            }
             GenerateLargeTestData(settings, settings.ToLanguages().ToList());
            return settings;
        }

        private void FromNestMethod(List<LanguageSetting> settings, string language, string content)
        {
            var languageSettings = new List<LanguageSetting>();
            try
            {
                dynamic myObj = JsonConvert.DeserializeObject(content);
                foreach (JProperty jproperty in myObj)
                {
                    ProcessSettings(language, languageSettings, (JToken)jproperty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid json in " + language);
            }
            settings.AddRange(languageSettings);
        }

        private void GenerateLargeTestData(List<LanguageSetting> settings, IEnumerable<string> languages)
        {
            foreach (var language in languages)
            {
                for (int i = 0; i < 20; i++)
                {
                    for (int n = 0; n < 5; n++)
                    {

                        for (int s = 0; s < 200; s++)
                        {
                            settings.Add(new LanguageSetting() { Language = language, Namespace = $"test.{i}.{n}.{s}", Value = "generatedval" });
                        }
                    }
                }
            }
        }

        private void ProcessSettings(string language, List<LanguageSetting> list, JToken property)
        {
            if (property.Children().Any())
            {
                foreach (var childProperty in property.Children())
                {
                    ProcessSettings(language, list, childProperty);
                }
            }
            else
            {
                list.Add(new LanguageSetting() { Namespace = CleanPath(property.Path), Value = property.ToObject<string>(), Language = language });
            }
        }

        private string CleanPath(string path)
        {
            var newPath = path;
            if (newPath.StartsWith("['"))
            {
                newPath = newPath.Substring(2);
            }
            if (newPath.EndsWith("']"))
            {
                newPath = newPath.Substring(0, newPath.Length - 2);
            }

            return newPath;
        }


        public void SaveNsJson(string path, List<NsTreeItem> items, List<string> languages)
        {

            foreach (var language in languages)
            {
                var dyn = new Dictionary<string, dynamic>();

                for (int i = 0; i < items.Count; i++)
                {
                    items[i].ToJson(dyn, language);
                }
                var newFilePath = System.IO.Path.Combine(path, language + ".json");
                var json = JsonConvert.SerializeObject(dyn, Formatting.Indented);
                System.IO.File.WriteAllText(newFilePath,json);
            }

        }


        public void SaveJson(string path, Dictionary<string, IEnumerable<LanguageSetting>> languageSettings)
        {

            foreach (var languageSetting in languageSettings)
            {
                var newFilePath = System.IO.Path.Combine(path, languageSetting.Key + ".json");
                var contentBuilder = new StringBuilder("{\n");
                var counter = 0;
                foreach (var setting in languageSetting.Value.NoEmpty().OrderBy(o => o.Namespace))
                {
                    counter++;
                    contentBuilder.AppendLine((counter == 1 ? "" : ",") + "\t\"" + setting.Namespace + "\" : \"" + setting.Value + "\"");
                }

                contentBuilder.AppendLine("}");
                System.IO.File.WriteAllText(newFilePath, contentBuilder.ToString());
            }
        }
    }




    public enum SaveStyles
    {
        Json,
        Namespaced,
    }
}
