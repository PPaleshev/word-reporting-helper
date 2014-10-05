using System.Collections.Generic;
using System.Linq;

namespace SampleWordHelper.Model
{
    public class NodeView
    {
        readonly ICatalog catalog;
        readonly HashSet<string> entries = new HashSet<string>();
        readonly IList<string> roots = new List<string>();
        readonly Dictionary<string, List<string>> hierarchy = new Dictionary<string, List<string>>();

        readonly string filter;

        public NodeView(ICatalog catalog, string filter)
        {
            this.catalog = catalog;
            this.filter = filter.ToLower();
            FilterElements();
        }

        public IEnumerable<string> GetRootElements()
        {
            return roots;
        }

        public IEnumerable<string> GetChildElements(string parent)
        {
            List<string> child;
            return hierarchy.TryGetValue(parent, out child) ? child : Enumerable.Empty<string>();
        }

        public bool Contains(string id)
        {
            return entries.Contains(id);
        }

        void FilterElements()
        {
            foreach (var root in catalog.GetRootElements().Where(FilterInChild))
            {
                entries.Add(root);
                roots.Add(root);
            }
        }

        bool FilterInChild(string parentId)
        {
            var result = false;
            foreach (var childId in catalog.GetChildElements(parentId))
            {
                if (catalog.IsGroup(childId) ? FilterInChild(childId) : SatisfiesFilter(childId))
                    result |= AddToHierarchy(parentId, childId);
            }
            return result;
        }

        bool AddToHierarchy(string parent, string elementId)
        {
            List<string> elements;
            if (!hierarchy.TryGetValue(parent, out elements))
                hierarchy.Add(parent, elements = new List<string>());
            elements.Add(elementId);
            entries.Add(elementId);
            return true;
        }

        bool SatisfiesFilter(string id)
        {
            return string.IsNullOrWhiteSpace(id) || catalog.GetName(id).ToLower().Contains(filter);
        }
    }
}
