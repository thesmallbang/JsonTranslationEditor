using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTranslationEditor
{
    public class SummaryItem
    {
        public string Language { get; set; }
        public double PercentageMissing { get; set; }
        public double Missing { get; set; }
        public DateTime Updated { get; } = DateTime.Now;
        public string Stats => Missing > 0 ? $"{Language} : Missing: {Missing} ({PercentageMissing}%)" : $"{Language} : OK!";

     }
}
