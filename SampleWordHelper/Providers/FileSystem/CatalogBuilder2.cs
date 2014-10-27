using System;
using System.Collections.Generic;
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
        public CatalogBuilderResult Build()
        {
            var result = new CatalogBuilderResult(rootUri);
            foreach (var file in EnumerateFiles(rootDirectory))
                result.AddFile(file, null);
            foreach (var directory in Directory.EnumerateDirectories(rootDirectory))
                ScanDirectory(directory, null, result);
            return result;
        }

        /// <summary>
        /// Фильтрует файлы по названию.
        /// </summary>
        /// <param name="filePath">Полный путь к файлу.</param>
        /// <returns>Возвращает true, если файл может быть добавлен в каталог, иначе false.</returns>
        static bool FilterFile(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            if (!(".doc".Equals(ext, StringComparison.InvariantCultureIgnoreCase) || ".docx".Equals(ext, StringComparison.InvariantCultureIgnoreCase)))
                return false;
            var name = Path.GetFileName(filePath);
            return !name.StartsWith("~$");
        }

        /// <summary>
        /// Выполняет сканирование директории.
        /// </summary>
        /// <param name="directoryFullName">Полный путь к директории.</param>
        /// <param name="parentId">Идентификатор родительской директории.</param>
        /// <param name="result">Результат сканирования файловой системы.</param>
        /// <returns>Возвращает true, если директория или одна из её поддиректорий содержат искомые файлы, иначе false.</returns>
        bool ScanDirectory(string directoryFullName, string parentId, CatalogBuilderResult result)
        {
            var id = FileSystemUtils.GetRelativePath(rootUri, directoryFullName, true);
            var materializeThis = materializeEmptyPaths;
            foreach (var file in EnumerateFiles(directoryFullName))
                materializeThis |= result.AddFile(file, id);
            materializeThis = Directory.EnumerateDirectories(directoryFullName).Aggregate(materializeThis, (val, dir) => val | ScanDirectory(dir, id, result));
            if (materializeThis)
                result.AddGroup(directoryFullName, id, parentId);
            return materializeThis;
        }

        /// <summary>
        /// Перечисляет все требуемые файлы в директории.
        /// </summary>
        /// <param name="directoryPath">Путь к каталогу.</param>
        static IEnumerable<string> EnumerateFiles(string directoryPath)
        {
            return Directory.EnumerateFiles(directoryPath).Where(FilterFile);
        }
    }
}