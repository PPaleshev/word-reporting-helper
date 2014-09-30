using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SampleWordHelper.Model;
using Microsoft.Experimental.IO;

namespace SampleWordHelper.Providers.FileSystem
{
    public class CatalogBuilder2
    {        
        /// <summary>
        /// Фильтр файлов.
        /// </summary>
        const string SEARCH_PATTERN = "*.docx";

        /// <summary>
        /// Разделитель элементов URI.
        /// </summary>
        const char URI_SEPARATOR = '/';

        readonly bool materializeEmptyPaths;
        readonly string rootDirectory;
        readonly Uri rootUri;

        public CatalogBuilder2(string rootDirectory, bool materializeEmptyPaths)
        {
            this.rootDirectory = rootDirectory;
            this.materializeEmptyPaths = materializeEmptyPaths;
            rootUri = new Uri(EnsureDirectory(rootDirectory));
        }

        public void Build(CatalogModel catalog)
        {
            foreach (var file in LongPathDirectory.EnumerateFiles(rootDirectory, SEARCH_PATTERN))
                AddFile(catalog, file, "");
            foreach (var directory in LongPathDirectory.EnumerateDirectories(rootDirectory))
                ProcessDirectory(directory, null, catalog);
        }

        bool ProcessDirectory(string directoryFullName, string parentId, CatalogModel catalog)
        {
            var shortName = Path.GetFileName(EnsureFile(directoryFullName));
            var id = GetRelativePath(directoryFullName, true);
            var files = LongPathDirectory.EnumerateFiles(directoryFullName, SEARCH_PATTERN);
            var materializeThis = false;
            foreach (var file in files)
            {
                materializeThis = true;
                AddFile(catalog, file, id);
            }
            materializeThis = LongPathDirectory.EnumerateDirectories(directoryFullName).Aggregate(materializeThis, (val, dir) => val | ProcessDirectory(dir, id, catalog));
            if (materializeThis)
                catalog.AddGroup(id, parentId, shortName);
            return materializeThis;
        }

        /// <summary>
        /// Добавляет файл в каталог.
        /// </summary>
        /// <param name="catalog">Каталог.</param>
        /// <param name="file">Полный путь к файлу.</param>
        /// <param name="parent">Идентификатор родителя.</param>
        void AddFile(CatalogModel catalog, string file, string parent)
        {
            catalog.AddItem(GetRelativePath(file, false), parent, Path.GetFileNameWithoutExtension(file), file);
        }

        /// <summary>
        /// Возвращает полный путь к каталогу с добавлением <see cref="Path.DirectorySeparatorChar"/> в конец при его отсутствии.
        /// </summary>
        static string EnsureDirectory(string path)
        {
            if (path[path.Length - 1] != Path.DirectorySeparatorChar)
                path += Path.DirectorySeparatorChar;
            return path;
        }

        static string EnsureFile(string path)
        {
            return path[path.Length - 1] == Path.DirectorySeparatorChar ? path.Substring(0, path.Length - 1) : path;
        }

        /// <summary>
        /// Возвращает путь относительно корневого каталога.
        /// </summary>
        /// <param name="fullName">Текущий путь.</param>
        /// <param name="directory">true, если <paramref name="fullName"/> представляет путь к каталогу, и false, если к файлу.</param>
        /// <remarks>Вырезает из конца результата все символы <see cref="URI_SEPARATOR"/>.</remarks>
        string GetRelativePath(string fullName, bool directory)
        {
            var path = directory ? EnsureDirectory(fullName) : fullName;
            var relUri = rootUri.MakeRelativeUri(new Uri(EnsureDirectory(path)));
            return Uri.UnescapeDataString(relUri.OriginalString).TrimEnd(URI_SEPARATOR);
        }
    }
}