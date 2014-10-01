using System;
using System.Collections.Generic;

namespace SampleWordHelper.Model
{
    public interface ICatalog
    {
        IEnumerable<string> GetRootElements();
        IEnumerable<string> GetChildElements(string parentId);
        string GetName(string id);
        string GetDescription(string id);
        string GetLocation(string id);
        bool IsGroup(string id);
        bool Contains(string id);
    }
}
