using System.Collections.Generic;
using System.Linq;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Реализация пустого каталога.
    /// </summary>
    public class EmptyCatalog : ICatalog
    {
        public IEnumerable<string> GetRootElements()
        {
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetChildElements(string parentId)
        {
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> All()
        {
            return Enumerable.Empty<string>();
        }

        public string GetName(string id)
        {
            return "";
        }

        public string GetDescription(string id)
        {
            return "";
        }

        public string GetLocation(string id)
        {
            return "";
        }

        public bool IsGroup(string id)
        {
            return false;
        }

        public bool Contains(string id)
        {
            return false;
        }
    }
}