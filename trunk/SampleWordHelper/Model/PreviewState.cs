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
    public class PreviewState: BasicDisposable
    {
        IPreviewHandler handler;
        SafeFilePath safeFilePath;
        Stream stream;

        public string FileName { get; private set; }
        public string ErrorMessage { get; private set; }
        public bool IsValid { get; private set; }

        /// <summary>
        /// Ссылка на представление.
        /// </summary>
        public IPreviewView View { get; set; }

        public PreviewState(string fileName)
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

        public void ShowPreview(IntPtr window, Rectangle area)
        {
            if (Disposed)
                return;
            if (!IsValid )
                throw new InvalidOperationException();
            handler.SetWindow(window, new RECT(area));
            handler.DoPreview();
            handler.SetFocus();
        }

        public void UpdateSize(Rectangle area)
        {
            if (Disposed)
                return;
            handler.SetRect(new RECT(area));
        }

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