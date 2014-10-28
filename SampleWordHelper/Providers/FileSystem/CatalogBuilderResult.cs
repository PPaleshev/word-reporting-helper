using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Результат сканирования файловой системы.
    /// </summary>
    public class CatalogBuilderResult
    {
        /// <summary>
        /// URI корневого каталога.
        /// </summary>
        readonly Uri rootUri;

        /// <summary>
        /// Экземпляр каталога.
        /// </summary>
        readonly Catalog catalog;

        /// <summary>
        /// Список ошибок.
        /// </summary>
        readonly List<ElementValidationInfo> errors = new List<ElementValidationInfo>();

        /// <summary>
        /// Создаёт результат построения каталога.
        /// </summary>
        /// <param name="rootUri">URI корневого каталога.</param>
        public CatalogBuilderResult(Uri rootUri)
        {
            this.rootUri = rootUri;
            catalog = new Catalog();
        }

        /// <summary>
        /// Возвращает массив ошибок, возникших при сканировании файловой системы.
        /// </summary>
        public ElementValidationInfo[] GetErrors()
        {
            return errors.ToArray();
        }

        /// <summary>
        /// Возвращает экземпляр каталога.
        /// </summary>
        public Catalog GetCatalog()
        {
            return errors.Any(info => info.IsError) ? null : catalog;
        }

        /// <summary>
        /// Добавляет файл в каталог.
        /// </summary>
        /// <param name="file">Полный путь к файлу.</param>
        /// <param name="parent">Идентификатор родителя.</param>
        /// <returns>Возвращает true, если файл был успешно добавлен, иначе false.</returns>
        public bool AddFile(string file, string parent)
        {
            var id = FileSystemUtils.GetRelativePath(rootUri, file, false);
            if (file.Length > 255)
            {
                errors.Add(new ElementValidationInfo(id, "Путь к файлу слишком длинный: " + file, false));
                return false;
            }
            try
            {
                catalog.AddItem(id, parent, Path.GetFileNameWithoutExtension(file), file);
                return true;
            }
            catch (PathTooLongException)
            {
                errors.Add(new ElementValidationInfo(id, "Путь к файлу слишком длинный: " + file, false));
                return false;
            }
        }

        /// <summary>
        /// Добавляет группу в каталог.
        /// </summary>
        /// <param name="directory">Полный путь к директории.</param>
        /// <param name="id">Идентификатор директории. </param>
        /// <param name="parent">Идентификатор родителя.</param>
        public void AddGroup(string directory, string id, string parent)
        {
            var shortName = Path.GetFileName(FileSystemUtils.EnsureFile(directory));
            catalog.AddGroup(id, parent, shortName, directory);
        }
    }
}
