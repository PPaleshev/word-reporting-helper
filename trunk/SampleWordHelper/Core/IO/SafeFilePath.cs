using System.IO;
using SampleWordHelper.Core.Common;

namespace SampleWordHelper.Core.IO
{
    /// <summary>
    /// Обёртка для безопасного копирования файлов.
    /// </summary>
    public  class SafeFilePath :BasicDisposable
    {
        /// <summary>
        /// Флаг, равный true, если был создан временный файл, иначе false.
        /// </summary>
        readonly bool temp;
        
        /// <summary>
        /// Безопасный путь к содержимому исходного файла.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Создаёт новый экземпляр безопасного доступа к файлу.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        public SafeFilePath(string filePath)
        {
            if (filePath.Length < 248)
                FilePath = filePath;
            else
            {
                temp = true;
                FilePath = Path.GetTempFileName();
                LongPathFile.Copy(filePath, FilePath, true);
            }
        }

        protected override void DisposeUnmanaged()
        {
            if (!temp)
                return;
            try
            {
                LongPathDirectory.Delete(FilePath);
            }
            catch
            {
                throw;
            }
        }

        protected override void DisposeManaged()
        {
        }
    }
}