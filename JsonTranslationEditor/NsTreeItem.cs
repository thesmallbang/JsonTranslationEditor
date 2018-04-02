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
        public bool IsLoaded { get; set; }

        public string Name { get; set; }
        public string Namespace { get; set; }
        public string ImagePath { get; set; }
        private List<NsTreeItem> _storage { get; set; } = new List<NsTreeItem>();

        public IEnumerable<NsTreeItem> Items
        {
            get
            {
                if (!IsLoaded)
                    if (HeldSetttings != null)
                    {
                        var node = this;
                        Extensions.ProcessNs(node, Namespace, HeldSetttings.ToList());
                        Console.WriteLine(node.Namespace);
                        HeldSetttings = null;
                    }

                return _storage;
            }
            set { _storage = value.ToList(); }
        }

        public IEnumerable<LanguageSetting> Settings { get; set; }

        public bool HasSiblingBefore
        {
            get
            {
                if (Parent == null)
                    return false;


                if (Parent._storage.Count() <= 0)
                    throw new Exception("Item has a parent set but parent has no items set");

                if (Parent._storage.Count() == 1)
                    return false;

                var firstChild = Parent._storage.OrderBy(o => o.Namespace).FirstOrDefault();
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

                if (Parent._storage.Count() == 1)
                    return false;

                if (Parent._storage.Count() <= 0)
                    throw new Exception("Item has a parent set but parent has no items set");

                var lastChild = Parent._storage.OrderByDescending(o => o.Namespace).FirstOrDefault();
                if (lastChild != this)
                    return true;

                return false;
            }

        }

        public bool HasItems => _storage == null ? false : true;
        public bool HasParent => Parent != null;

        public NsTreeItem() { }

        public string ToJson(string language, int tabindex = 0)
        {
            tabindex++;
            var result = string.Empty;
            var setting = Settings?.FirstOrDefault(o => o.Language == language);



            result += new string('\t', tabindex); //tab if it has a parent

            result += $"\"{Name}\": "; //write out property name in all scenarios

            if (!Items.Any())
            {
                result += $"\"{setting.Value}\"";

            }
            else
            {
                result += "{\n";
                foreach (var item in _storage)
                {
                    result += item.ToJson(language, tabindex) + "\n";
                }

                result += new string('\t', tabindex) + "}";



            }
            if (HasSiblingAfter)
                result += ",";


            return result;
        }

        //public override string ToString()
        //{
        //    return $"{Name} | {Namespace} | Items: {(_storage == null ? 0 : _storage.Count())}";
        //}

        public List<LanguageSetting> HeldSetttings;

        public void AddChild(NsTreeItem child)
        {
            _storage.Add(child);
        }
        public void Clear()
        {
            _storage.Clear();
        }

    }
}
