using System.ComponentModel;
using System.IO;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Модель для редактирования настроек поставщика каталога на основе доступа к файловой системе.
    /// </summary>
    internal class FileSystemProviderSettingsModel : ISettingsModel
    {
        /// <summary>
        /// Путь к каталогу.
        /// </summary>
        [DisplayName("Путь к каталогу")]
        [Category("Основные")]
        public string RootPath { get; set; }

        public ValidationResult Validate()
        {
            if (string.IsNullOrEmpty(RootPath))
                return new ValidationResult("Путь к каталогу должен быть указан.");
            if (RootPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return new ValidationResult("Путь содержит недопустимые символы.");
            if (!Directory.Exists(RootPath))
                return new ValidationResult("Путь указывает на несуществующую директорию файловой системы.");
            return ValidationResult.CORRECT;
        }

        /// <summary>
        /// Загружает настройки из объекта.
        /// </summary>
        public void LoadFrom(FileSystemProviderSettings settings)
        {
            RootPath = settings.RootPath;
        }

        /// <summary>
        /// Сохраняет настройки в объект настроек.
        /// </summary>
        public void SaveTo(FileSystemProviderSettings settings)
        {
            settings.RootPath = RootPath;
        }
    }
}
