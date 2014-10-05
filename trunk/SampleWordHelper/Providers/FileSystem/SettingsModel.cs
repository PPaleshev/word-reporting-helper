using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using SampleWordHelper.Core.Design;
using SampleWordHelper.Core.IO;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Модель для редактирования настроек поставщика каталога на основе доступа к файловой системе.
    /// </summary>
    internal class SettingsModel : ISettingsModel
    {
        /// <summary>
        /// Внутренний объект, которому делегируются свойства.
        /// </summary>
        readonly ProviderSettings settings;

        /// <summary>
        /// Путь к каталогу.
        /// </summary>
        [DisplayName("Путь к каталогу")]
        [Category("Основные")]
        [Editor(typeof (FileSystemPathEditor), typeof (UITypeEditor))]
        public string RootPath
        {
            get { return settings.RootDirectory; }
            set { settings.RootDirectory = value; }
        }

//        /// <summary>
//        /// Путь к каталогу, в котором будет храниться кэш с файлами.
//        /// </summary>
//        [DisplayName("Путь к кэшу")]
//        [Category("Кэширование")]
//        [Editor(typeof (FileSystemPathEditor), typeof (UITypeEditor))]
//        public string CacheDirectory
//        {
//            get { return settings.CacheDirectory; }
//            set { settings.CacheDirectory = value; }
//        }
//
//        /// <summary>
//        /// Флаг, равный true, если должно быть использовано локальное кэширование, иначе false.
//        /// </summary>
//        [DisplayName("Использовать кэширование")]
//        [Description("Кэширование может быть использовано во избежание ошибок доступа к файлам с длинными именами.")]
//        [Category("Кэширование")]
//        public bool UseLocalCache
//        {
//            get { return settings.UseLocalCache; }
//            set { settings.UseLocalCache = value; }
//        }

        /// <summary>
        /// Флаг, равный true, если при обходе каталога необходимо сохранять ветви, не содержащие файлов, иначе false.
        /// </summary>
        [Category("Дополнительно")]
        [DisplayName("Отображать пустые группы")]
        public bool MaterializeEmptyPaths
        {
            get { return settings.MaterializeEmptyPaths; }
            set { settings.MaterializeEmptyPaths = value; }
        }

        public SettingsModel(ProviderSettings settings)
        {
            this.settings = settings;
        }

        public SettingsModel() : this(new ProviderSettings())
        {
        }

        public ValidationResult Validate()
        {
            if (string.IsNullOrEmpty(RootPath))
                return new ValidationResult("Путь к каталогу должен быть указан.");
            if (RootPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return new ValidationResult("Путь к каталогу содержит недопустимые символы.");
            if (!LongPathDirectory.Exists(RootPath))
                return new ValidationResult("Путь к каталогу указывает на несуществующую директорию файловой системы.");
//            if (UseLocalCache)
//            {
//                if (string.IsNullOrWhiteSpace(CacheDirectory))
//                    return new ValidationResult("Путь к локальному кэшу должен быть указан.");
//                if (CacheDirectory.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
//                    return new ValidationResult("Путь к локальному кэшу содержит недопустимые символы.");
//                if (!LongPathDirectory.Exists(CacheDirectory))
//                    return new ValidationResult("Путь к кэшу указывает на несуществующую директорию файловой системы.");
//            }
            return ValidationResult.CORRECT;
        }

        /// <summary>
        /// Создаёт объект с настройками.
        /// </summary>
        public ProviderSettings CreateSettingsObject()
        {
            return settings;
        }
    }
}
