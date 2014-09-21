using System.ComponentModel;
using System.IO;
using SampleWordHelper.Configuration;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Providers.FileSystem
{
    internal class Settings : ISettingsModel
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

        public void LoadFrom(FileSystemProviderConfigurationSection section)
        {
            RootPath = section.RootPath;
        }

        public void SaveTo(FileSystemProviderConfigurationSection section)
        {
            section.RootPath = RootPath;
        }
    }
}
