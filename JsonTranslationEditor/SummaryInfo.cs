using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace JsonTranslationEditor
{
    public class SummaryInfo
    {
        public double Languages { get; private set; }
        public double Translations { get; private set; }
        public List<SummaryItem> Details { get; private set; } = new List<SummaryItem>();

        public void Update(IEnumerable<LanguageSetting> settings)
        {

            var allNamespace = settings.ToNamespaces().ToList();
            var allLanguages = settings.ToLanguages().ToList();

            Languages = allLanguages.Count;
            Translations = allNamespace.Count;

            Details.Clear();
            foreach (var language in allLanguages)
            {
                var languageNamespaces = settings.ToNamespaces(language).ToList();
                double missingLanguageCount = allNamespace.Except(languageNamespaces).Count();

                var translated = languageNamespaces.Count - missingLanguageCount;
                Details.Add(new SummaryItem()
                {
                    Language = language,
                    Potential = this.Translations,
                    Missing = missingLanguageCount
                });
            }


        }

    }
}
