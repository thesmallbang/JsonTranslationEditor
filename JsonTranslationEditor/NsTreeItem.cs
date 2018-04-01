using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTranslationEditor
{
    public class NsTreeItem
    {
        public NsTreeItem Parent { get; private set; }

        public string Name { get; set; }
        public string Namespace { get; set; }
        public string ImagePath { get; set; }
        public IEnumerable<NsTreeItem> Items { get; set; }
        public LanguageSetting Setting { get; set; }

        public NsTreeItem() { }

       


    }
}
