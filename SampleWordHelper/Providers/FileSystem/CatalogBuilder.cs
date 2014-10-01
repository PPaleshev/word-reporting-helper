using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Вспомогательный класс для построения модели каталога.
    /// </summary>
    [Obsolete]
    public class CatalogBuilder
    {
        /// <summary>
        /// Фильтр файлов.
        /// </summary>
        const string FILTER = "*.docx";

        /// <summary>
        /// Разделитель элементов URI.
        /// </summary>
        const char URI_SEPARATOR = '/';

        /// <summary>
        /// Uri корневого каталога.
        /// </summary>
        readonly Uri rootUri;

        /// <summary>
        /// Корневая директория.
        /// </summary>
        readonly DirectoryInfo root;

        public CatalogBuilder(DirectoryInfo root)
        {
            this.root = root;
            rootUri = new Uri(EnsureDirectoryPath(root));
        }

        /// <summary>
        /// Заполняет каталог данными.
        /// </summary>
        public void BuildCatalog(Catalog catalog)
        {
            var rootNode = new Node(root.Name, null, null);
            var index = new Dictionary<string, Node>();
            var items = root.GetFiles(FILTER, SearchOption.AllDirectories);
            foreach (var item in items)
                AddFile(item, rootNode, index);
            FillCatalog(rootNode, catalog);
        }

        /// <summary>
        /// Создаёт путь для хранения указанного файла.
        /// </summary>
        /// <param name="file">Файл.</param>
        /// <param name="rootNode">Корневой узел.</param>
        /// <param name="pathIndex">Отображение из уже существующего относительного пути в узел, содержащий информацию об этом узле.</param>
        void AddFile(FileInfo file, Node rootNode, IDictionary<string, Node> pathIndex)
        {
            var id = GetRelativePath(file.FullName);
            var parentId = GetRelativePath(EnsureDirectoryPath(file.Directory));
            Node node;
            if (!pathIndex.TryGetValue(parentId, out node))
                node = CreateNode(parentId, rootNode, pathIndex);
            node.items.Add(Tuple.Create(id, file));
        }

        /// <summary>
        /// Строит путь в дереве для размещения родительского узла.
        /// </summary>
        /// <param name="parentId">Относительный путь родительского каталога.</param>
        /// <param name="rootNode">Корневой элемент дерева.</param>
        /// <param name="pathIndex">Отображение из уже существующего относительного пути в узел, содержащий информацию об этом узле.</param>
        static Node CreateNode(string parentId, Node rootNode, IDictionary<string, Node> pathIndex)
        {
            var parts = parentId.Split(new[] {URI_SEPARATOR}, StringSplitOptions.RemoveEmptyEntries);
            var node = rootNode;
            var pathBuilder = new StringBuilder();
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                var parent = pathBuilder.ToString();
                if (i > 0)
                    pathBuilder.Append(URI_SEPARATOR);
                pathBuilder.Append(part);
                Node temp;
                if (node.groups.TryGetValue(part, out temp))
                    node = temp;
                else
                {
                    temp = new Node(pathBuilder.ToString(), parent, part);
                    node.groups.Add(part, temp);
                    node = temp;
                    pathIndex.Add(pathBuilder.ToString(), node);
                }
            }
            return node;
        }

        /// <summary>
        /// Заполняет каталог в соответствии с разобранным деревом.
        /// </summary>
        /// <param name="rootNode">Корневой элемент дерева.</param>
        /// <param name="catalog">Каталог.</param>
        static void FillCatalog(Node rootNode, Catalog catalog)
        {
            var stack = new Stack<Node>();
            foreach (var levelOneNode in rootNode.groups)
                stack.Push(levelOneNode.Value);
            foreach (var item in rootNode.items)
                catalog.AddItem(item.Item1, null, Path.GetFileNameWithoutExtension(item.Item2.Name), item.Item2.FullName);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                catalog.AddGroup(current.id, current.parentId, current.name, current.id);
                foreach (var item in current.items)
                    catalog.AddItem(item.Item1, current.id, Path.GetFileNameWithoutExtension(item.Item2.Name), item.Item2.FullName);
                foreach (var node in current.groups)
                    stack.Push(node.Value);
            }
        }

        /// <summary>
        /// Возвращает путь относительно корневого каталога <see cref="root"/>.
        /// </summary>
        /// <param name="fullName">Текущий путь.</param>
        /// <remarks>Вырезает из конца результата все символы <see cref="URI_SEPARATOR"/>.</remarks>
        string GetRelativePath(string fullName)
        {
            var relUri = rootUri.MakeRelativeUri(new Uri(fullName));
            return Uri.UnescapeDataString(relUri.OriginalString).TrimEnd(URI_SEPARATOR);
        }

        /// <summary>
        /// Возвращает полный путь к каталогу с добавлением <see cref="Path.DirectorySeparatorChar"/> в конец при его отсутствии.
        /// </summary>
        static string EnsureDirectoryPath(DirectoryInfo directory)
        {
            var path = directory.FullName;
            if (path[path.Length - 1] != Path.DirectorySeparatorChar)
                path += Path.DirectorySeparatorChar;
            return path;
        }

        /// <summary>
        /// Класс узла дерева для хранения информации.
        /// </summary>
        class Node
        {
            /// <summary>
            /// Идентификатор узла.
            /// </summary>
            public readonly string id;

            /// <summary>
            /// Идентификатор корневого узла.
            /// </summary>
            public readonly string parentId;

            /// <summary>
            /// Название элемента.
            /// </summary>
            public readonly string name;

            /// <summary>
            /// Отображение из названия узла в его объект.
            /// </summary>
            public readonly Dictionary<string, Node> groups = new Dictionary<string, Node>();

            /// <summary>
            /// Список пар из идентификатора файла в представляющий его объект.
            /// </summary>
            public readonly List<Tuple<string, FileInfo>> items = new List<Tuple<string, FileInfo>>();

            public Node(string id, string parentId, string name)
            {
                this.id = id;
                this.parentId = parentId;
                this.name = name;
            }
        }
    }
}