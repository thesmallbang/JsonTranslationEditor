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

        public LanguageGroup(string ns)
        {
            Namespace = ns;
        }
        public void LoadSettings(IEnumerable<LanguageSetting> settings)
        {
            Translations = settings;
        }
    }
}
