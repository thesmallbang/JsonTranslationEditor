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
        public bool IsLoaded { get; set; }
        public List<LanguageSetting> PendingLoad { get; set; }

        public List<NsTreeItem> Items { get; set; } = new List<NsTreeItem>();

        public List<NsTreeItem> LoadedItems
        {
            get
            {
                if (!IsLoaded)
                {
                    if (PendingLoad == null || !PendingLoad.Any())
                    {
                        IsLoaded = true;
                        return Items.ToList();
                    }


                    Extensions.ProcessNs(this, Namespace, PendingLoad.ToList(),1);
                    IsLoaded = true;
                }

                return Items.ToList();
            }
            
        }
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


        public override string ToString()
        {
            return $"{Name} | {Namespace} | Items: {Items.Count()}";
        }

    }
}
