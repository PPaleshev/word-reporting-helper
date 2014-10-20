using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Core.IO;
using SampleWordHelper.Core.Native;
using SampleWordHelper.Interface;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Класс, описывающий состояние предварительного просмотра.
    /// </summary>
    public class PreviewModel : BasicDisposable
    {
        /// <summary>
        /// Ссылка на объект, реализующий предварительный просмотр.
        /// </summary>
        IPreviewHandler handler;

        /// <summary>
        /// Безопасный путь к файлу, просмотр которого необходимо обеспечить.
        /// </summary>
        SafeFilePath safeFilePath;

        /// <summary>
        /// Экземпляр потока, которым может инициализироваться обработчик превью.
        /// </summary>
        Stream stream;

        /// <summary>
        /// Путь к файлу для предварительного просмотра.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Сообщение об ошибке инициализации.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// True, если модель успешно инициализирована, иначе false.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Создаёт новый экземпляр модели.
        /// </summary>
        /// <param name="fileName"></param>
        public PreviewModel(string fileName)
        {
            FileName = fileName;
            var data = PreviewHandlerRegistry.LoadHandlers();
            var extension = Path.GetExtension(fileName);
            var handlerInfo = data.Extensions.Where(info => string.Equals(info.extension, extension, StringComparison.InvariantCultureIgnoreCase)).
                Select(info => info.handler).FirstOrDefault();
            if (handlerInfo == null)
            {
                IsValid = false;
                ErrorMessage = "Не удалось инициировать предварительный просмотр.";
                return;
            }
            try
            {
                InitializeHandler(handlerInfo.id);
                IsValid = true;
            }
            catch
            {
                IsValid = false;
                ErrorMessage = "Предварительный просмотр недоступен из-за возникшей ошибки.";
            }
        }

        /// <summary>
        /// Выполняет отображение превью в области окна с указанным дескриптором.
        /// </summary>
        /// <param name="window">Дескриптор окна.</param>
        /// <param name="area">Область для отображения.</param>
        public void GeneratePreview(IntPtr window, Rectangle area)
        {
            if (Disposed)
                return;
            if (!IsValid)
                throw new InvalidOperationException();
            handler.SetWindow(window, new RECT(area));
            handler.DoPreview();
            handler.SetFocus();
        }

        /// <summary>
        /// Вызывается для обновления диапазона отобржажения превью.
        /// </summary>
        /// <param name="area">Прямоугольник, описывающий область отображения.</param>
        public void UpdateSize(Rectangle area)
        {
            if (Disposed)
                return;
            handler.SetRect(new RECT(area));
        }

        /// <summary>
        /// Выполняет инициализацию обработчика превью.
        /// </summary>
        /// <param name="clsid">Идентификатор класса обработчика предварительного просмотра.</param>
        void InitializeHandler(string clsid)
        {
            var handlerType = Type.GetTypeFromCLSID(new Guid(clsid));
            handler = (IPreviewHandler) Activator.CreateInstance(handlerType);
            if (handler is IInitializeWithFile)
            {
                safeFilePath = new SafeFilePath(FileName);
                ((IInitializeWithFile) handler).Initialize(safeFilePath.FilePath, 0);
            }
            else if (handler is IInitializeWithStream)
            {
                stream = LongPathFile.Open(FileName, FileMode.Open, FileAccess.Read);
                ((IInitializeWithStream) handler).Initialize(new StreamAdapter(stream), 0);
            }
        }

        protected override void DisposeManaged()
        {
            if (handler != null)
            {
                handler.Unload();
                Marshal.FinalReleaseComObject(handler);
            }
            if (stream != null)
                stream.SafeDispose();
            if (safeFilePath != null)
                safeFilePath.SafeDispose();
        }
    }
}