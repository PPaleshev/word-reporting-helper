using System.ComponentModel;
using System.IO;
using System.Security.AccessControl;
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
        /// Внутренний объект, которому делегируются свойства.
        /// </summary>
        readonly FileSystemProviderSettings settings;

        /// <summary>
        /// Путь к каталогу.
        /// </summary>
        [DisplayName("Путь к каталогу")]
        [Category("Основные")]
        public string RootPath
        {
            get { return settings.RootPath; }
            set { settings.RootPath = value; }
        }

        public FileSystemProviderSettingsModel(FileSystemProviderSettings settings)
        {
            this.settings = settings;
        }

        public FileSystemProviderSettingsModel() : this(new FileSystemProviderSettings())
        {
        }

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
        /// Создаёт объект с настройками.
        /// </summary>
        public FileSystemProviderSettings CreateSettingsObject()
        {
            return settings;
        }
    }
}
