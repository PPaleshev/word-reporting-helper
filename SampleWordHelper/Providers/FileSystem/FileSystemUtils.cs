using System;
using System.IO;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Вспомогательные методы для работы с файловой системой и путями, длина которых превышает 260 символов.
    /// </summary>
    public static class FileSystemUtils
    {
        /// <summary>
        /// Разделитель элементов URI.
        /// </summary>
        const char URI_SEPARATOR = '/';

        /// <summary>
        /// Возвращает путь <paramref name="fullName"/> относительно <paramref name="root"/>.
        /// </summary>
        /// <param name="root">Каталог, относительно которого вычисляется путь.</param>
        /// <param name="fullName">Путь к файлу.</param>
        /// <param name="directory">True, если путь вычисляется для директории, и false, если для файла.</param>
        public static string GetRelativePath(string root, string fullName, bool directory)
        {
            var rootUri = new Uri(EnsureDirectory(root));
            return GetRelativePath(rootUri, fullName, directory);
        }

        /// <summary>
        /// Возвращает путь <paramref name="fullName"/> относительно uri корневого каталога <paramref name="root"/>.
        /// </summary>
        /// <param name="root">Uri каталога, относительно которого вычисляется путь.</param>
        /// <param name="fullName">Путь к файлу.</param>
        /// <param name="directory">True, если путь вычисляется для директории, и false, если для файла.</param>
        public static string GetRelativePath(Uri root, string fullName, bool directory)
        {
            var relUri = root.MakeRelativeUri(new Uri(directory ? EnsureDirectory(fullName) : EnsureFile(fullName)));
            return Uri.UnescapeDataString(relUri.OriginalString).TrimEnd(URI_SEPARATOR);
        }

        /// <summary>
        /// Возвращает полный путь к каталогу с добавлением <see cref="Path.DirectorySeparatorChar"/> в конец при его отсутствии.
        /// </summary>
        public static string EnsureDirectory(string path)
        {
            if (path[path.Length - 1] != Path.DirectorySeparatorChar)
                path += Path.DirectorySeparatorChar;
            return path;
        }

        /// <summary>
        /// Возвращает полный путь к файлу с удалением символа <see cref="Path.DirectorySeparatorChar"/> из конца при его наличии.
        /// </summary>
        public static string EnsureFile(string path)
        {
            return path[path.Length - 1] == Path.DirectorySeparatorChar ? path.Substring(0, path.Length - 1) : path;
        }
    }
}