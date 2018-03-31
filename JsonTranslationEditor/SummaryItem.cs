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
        public double Percentage { get { return Math.Round((((Potential - Missing) / Potential) * 100), 2); } }
        public double Missing { get; set; }
        public DateTime Updated { get; } = DateTime.Now;
        public double Potential { get; set; }

        public string Stats => Missing > 0 ? $"{Language} : {Percentage}% (missing {Missing} of {Potential})" :  $"{Language} : OK!";
           

     }
}
