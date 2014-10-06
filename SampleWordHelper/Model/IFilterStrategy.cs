using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using SampleWordHelper.Indexation;

namespace SampleWordHelper.Model
{
    public interface IFilterStrategy
    {
        bool PreserveCatalogStructure { get; }
        bool Satisfies(string elementId);
    }

    public class NullFilterStrategy : IFilterStrategy
    {
        public bool PreserveCatalogStructure
        {
            get { return true; }
        }

        public bool Satisfies(string elementId)
        {
            return true;
        }
    }

    public class ElementNameFilterStrategy : IFilterStrategy
    {
        readonly string name;
        readonly ICatalog catalog;

        public ElementNameFilterStrategy(ICatalog catalog, string name)
        {
            this.catalog = catalog;
            this.name = (name ?? "").ToLower();
        }

        public bool PreserveCatalogStructure
        {
            get { return false; }
        }

        public bool Satisfies(string elementId)
        {
            var elementName = (catalog.GetName(elementId) ?? "").ToLower();
            return elementName.Contains(name);
        }
    }

    public class ContentFilterStrategy : IFilterStrategy
    {
        readonly HashSet<string> suitableIds;

        public ContentFilterStrategy(ISearchEngine searchEngine, string filter)
        {
            suitableIds = new HashSet<string>(searchEngine.Search(filter));
        }

        public bool PreserveCatalogStructure
        {
            get { return false; }
        }

        public bool Satisfies(string elementId)
        {
            return suitableIds.Contains(elementId);
        }
    }
}