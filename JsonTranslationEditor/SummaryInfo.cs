using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTranslationEditor
{
    public class SummaryInfo
    {
        public double Languages { get; private set; }
        public double Translations { get; private set; }
        public List<SummaryItem> Details { get; } = new List<SummaryItem>();

        public void Update(IEnumerable<LanguageSetting> settings)
        {

            var allNamespace = settings.Select(o => o.Namespace).Distinct().ToList();
            var allLanguages = settings.Select(o => o.Language).Distinct().ToList();
            Details.Clear();

            Languages = allLanguages.Count;
            Translations = allNamespace.Count;

            foreach(var language in allLanguages)
            {
                var languageNamespaces = settings.Where(o => o.Language == language && !string.IsNullOrWhiteSpace(o.Value)).Select(o=>o.Namespace).Distinct().ToList();
                double languageMissing = allNamespace.Except(languageNamespaces).ToList().Count;

                var translated = languageNamespaces.Count - languageMissing;
                Details.Add(new SummaryItem(){Language = language, Missing = languageMissing, PercentageMissing = Math.Round(((languageMissing / allNamespace.Count) * 100),2)  });
            }

        }

    }
}
