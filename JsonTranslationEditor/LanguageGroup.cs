using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTranslationEditor
{
    public class LanguageGroup
    {
        public string Namespace { get; private set; }
        public IEnumerable<LanguageSetting> Translations { get; private set; }
        private string[] languages { get; set; }

        public LanguageGroup(string ns, string[] languages)
        {
            Namespace = ns;
            this.languages = languages;
        }
        public void LoadSettings(IEnumerable<LanguageSetting> settings)
        {
            var missingLanguages = languages.Except(settings.Select(o=>o.Language));
            foreach(var missingLanguage in missingLanguages)
            {
                ((List<LanguageSetting>)settings).Add(new LanguageSetting() { Language = missingLanguage, Namespace = Namespace, Value= "" });
            }

            Translations = settings.OrderBy(o=>o.Language);
        }
    }
}
