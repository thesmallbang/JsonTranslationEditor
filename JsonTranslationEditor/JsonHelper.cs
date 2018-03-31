using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public void SaveSettings(SaveStyles style, string path, Dictionary<string, IEnumerable<LanguageSetting>> languageSettings)
        {
            switch (style)
            {
                case SaveStyles.Json:
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
                    break;
                default:
                    throw new NotImplementedException("Save Style");
            }
        }

        public enum SaveStyles
        {
            Json,
            Namespaced,
        }
    }
}
