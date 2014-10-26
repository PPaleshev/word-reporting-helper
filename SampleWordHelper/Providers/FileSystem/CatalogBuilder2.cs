using System;
using System.IO;
using System.Linq;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Построитель каталога.
    /// </summary>
    public class CatalogBuilder2
    {
        /// <summary>
        /// Фильтр файлов.
        /// </summary>
        const string SEARCH_PATTERN = "*.doc";

        /// <summary>
        /// Флаг, равный true, если нужно сохранять пустые ветви, иначе false.
        /// </summary>
        readonly bool materializeEmptyPaths;

        /// <summary>
        /// Полный путь к корневой директории каталога.
        /// </summary>
        readonly string rootDirectory;

        /// <summary>
        /// Uri корневой директории каталога.
        /// </summary>
        readonly Uri rootUri;

        public CatalogBuilder2(string rootDirectory, bool materializeEmptyPaths)
        {
            this.rootDirectory = rootDirectory;
            this.materializeEmptyPaths = materializeEmptyPaths;
            rootUri = new Uri(FileSystemUtils.EnsureDirectory(rootDirectory));
        }

        /// <summary>
        /// Заполняет каталог данными.
        /// </summary>
        public void Build(Catalog catalog)
        {
            foreach (var file in Directory.EnumerateFiles(rootDirectory, SEARCH_PATTERN).Where(FilterFile))
                AddFile(catalog, file, null);
            foreach (var directory in Directory.EnumerateDirectories(rootDirectory))
                ScanDirectory(directory, null, catalog);
        }

        /// <summary>
        /// Фильтрует файлы по названию.
        /// </summary>
        /// <param name="filePath">Полный путь к файлу.</param>
        /// <returns>Возвращает true, если файл может быть добавлен в каталог, иначе false.</returns>
        static bool FilterFile(string filePath)
        {
            return !Path.GetFileName(filePath).StartsWith("~$");
        }

        /// <summary>
        /// Выполняет сканирование директории.
        /// </summary>
        /// <param name="directoryFullName">Полный путь к директории.</param>
        /// <param name="parentId">Идентификатор родительской директории.</param>
        /// <param name="catalog">Каталог, в который складываются результаты поиска.</param>
        /// <returns>Возвращает true, если директория или одна из её поддиректорий содержат искомые файлы, иначе false.</returns>
        bool ScanDirectory(string directoryFullName, string parentId, Catalog catalog)
        {
            var id = FileSystemUtils.GetRelativePath(rootUri, directoryFullName, true);
            var files = Directory.EnumerateFiles(directoryFullName, SEARCH_PATTERN).Where(FilterFile);
            var materializeThis = materializeEmptyPaths;
            foreach (var file in files)
            {
                materializeThis = true;
                AddFile(catalog, file, id);
            }
            materializeThis = Directory.EnumerateDirectories(directoryFullName).Aggregate(materializeThis, (val, dir) => val | ScanDirectory(dir, id, catalog));
            if (materializeThis)
                AddGroup(catalog, directoryFullName, id, parentId);
            return materializeThis;
        }

        /// <summary>
        /// Добавляет файл в каталог.
        /// </summary>
        /// <param name="catalog">Каталог.</param>
        /// <param name="file">Полный путь к файлу.</param>
        /// <param name="parent">Идентификатор родителя.</param>
        void AddFile(Catalog catalog, string file, string parent)
        {
            catalog.AddItem(FileSystemUtils.GetRelativePath(rootUri, file, false), parent, Path.GetFileNameWithoutExtension(file), file);
        }

        /// <summary>
        /// Добавляет группу в каталог.
        /// </summary>
        /// <param name="catalog">Каталог.</param>
        /// <param name="directory">Полный путь к директории.</param>
        /// <param name="id">Идентификатор директории. </param>
        /// <param name="parent">Идентификатор родителя.</param>
        static void AddGroup(Catalog catalog, string directory, string id, string parent)
        {
            var shortName = Path.GetFileName(FileSystemUtils.EnsureFile(directory));
            catalog.AddGroup(id, parent, shortName, directory);
        }
    }
}