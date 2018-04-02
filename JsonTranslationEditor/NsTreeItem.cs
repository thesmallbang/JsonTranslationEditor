using Newtonsoft.Json;
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
                        Extensions.ProcessNs(this, Namespace, HeldSetttings.ToList());

                        var copy = _storage.ToList();
                        _storage.Clear();
                        foreach (var child in copy)
                        {
                            child.Parent = this;
                            _storage.AddRange(child.Items);
                        }

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

        public bool HasItems => _storage == null ? false : _storage.Count >0;
        public bool HasParent => Parent != null;

        public NsTreeItem() { }


        public void ToJson(StringBuilder jsonBuilder, string language, int tabindex = 0)
        {

            var tab = new string('\t', tabindex); //tab if it has a parent
            tabindex++;


            jsonBuilder.Append(tab);

            jsonBuilder.Append($"\"{Name}\": ");

            if (!Items.Any())
            {
                var setting = Settings?.FirstOrDefault(o => o.Language == language);

                jsonBuilder.Append($"\"{setting.Value}\"");
            }
            else
            {
                jsonBuilder.Append("{\n");
                foreach (var item in _storage.Distinct())
                {
                    item.ToJson(jsonBuilder, language, tabindex);
                }

                jsonBuilder.Append(tab + "}");



            }
            if (HasSiblingAfter)
                jsonBuilder.Append(",");

            jsonBuilder.Append("\n");

        }


        public override string ToString()
        {
            return $"{Name} | {Namespace} | Items: {(_storage == null ? 0 : _storage.Count())}";
        }

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
