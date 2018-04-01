using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTranslationEditor
{
    public class NsTreeItem
    {
        public NsTreeItem Parent { get; set; }

        public string Name { get; set; }
        public string Namespace { get; set; }
        public string ImagePath { get; set; }

        public List<NsTreeItem> Items { get; set; } = new List<NsTreeItem>();


        public IEnumerable<LanguageSetting> Settings { get; set; }

        public bool HasSiblingBefore
        {
            get
            {
                if (Parent == null)
                    return false;


                if (Parent.Items.Count() <= 0)
                    throw new Exception("Item has a parent set but parent has no items set");

                if (Parent.Items.Count() == 1)
                    return false;

                var firstChild = Parent.Items.OrderBy(o => o.Namespace).FirstOrDefault();
                if (firstChild != this)
                    return true;

                return false;
            }

        }
        public bool HasSiblingAfter
        {
            get
            {
                if (Parent == null)
                    return false;

                if (Parent.Items.Count() == 1)
                    return false;

                if (Parent.Items.Count() <= 0)
                    throw new Exception("Item has a parent set but parent has no items set");

                var lastChild = Parent.Items.OrderByDescending(o => o.Namespace).FirstOrDefault();
                if (lastChild != this)
                    return true;

                return false;
            }

        }

        public bool HasItems => Items.Count() > 0;
        public bool HasParent => Parent != null;

        public NsTreeItem() { }

        public string ToJson(string language,int  tabindex = 0)
        {
            tabindex++;
            var result = string.Empty;
            var setting = Settings?.FirstOrDefault(o => o.Language == language);

            

            if (Parent != null)
                result += new string('\t',tabindex); //tab if it has a parent

            result += $"\"{Name}\": "; //write out property name in all scenarios

            if (!Items.Any())
            {
                result += $"\"{setting.Value}\"";

            }
            else
            {
                result += "{\n";
                foreach (var item in Items)
                {
                    result += item.ToJson(language, tabindex) + "\n";
                }

                result += new string('\t', tabindex) + "}";

           

            }
            if (HasSiblingAfter)
                result += ",";

      
            return result;
        }

        public override string ToString()
        {
            return $"{Name} | {Namespace} | Items: {Items.Count()}";
        }

    }
}
