using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Core.IO;
using SampleWordHelper.Core.Native;
using SampleWordHelper.Interface;

namespace SampleWordHelper.Presentation
{
    public class PreviewPresenter : BasicDisposable
    {
        readonly string unsafeFilePath;
        readonly IPreviewView view;

        SafeFilePath safeFilePath;
        IPreviewHandler handler;
        Stream stream;
        

        public PreviewPresenter(IRuntimeContext context, string filePath)
        {
            unsafeFilePath = filePath;
            view = context.ViewFactory.CreatePreviewView(this);
        }

        public void Run()
        {
            view.Show();
            var data = PreviewHandlerRegistry.LoadHandlers();
            var ext = Path.GetExtension(unsafeFilePath);
            var handlerInfo = data.Extensions.Where(info => string.Equals(info.extension, ext, StringComparison.InvariantCultureIgnoreCase)).Select(info => info.handler).FirstOrDefault();
            if (handlerInfo == null)
            {
                view.SetErrorMessage("Не удалось инициировать предварительный просмотр.");
                return;
            }
            try
            {
                InitializeHandler(handlerInfo.id);
            }
            catch (Exception)
            {
                view.SetErrorMessage("Предварительный просмотр недоступен из-за возникшей ошибки");
            }
        }

        void InitializeHandler(string id)
        {
            var handlerType = Type.GetTypeFromCLSID(new Guid(id));
            handler = (IPreviewHandler) Activator.CreateInstance(handlerType);
            if (handler is IInitializeWithFile)
            {
                safeFilePath = new SafeFilePath(unsafeFilePath);
                ((IInitializeWithFile) handler).Initialize(safeFilePath.FilePath, 0);
            }
            else if (handler is IInitializeWithStream)
            {
                stream = LongPathFile.Open(unsafeFilePath, FileMode.Open, FileAccess.Read);
                ((IInitializeWithStream) handler).Initialize(new StreamAdapter(stream), 0);
            }
            handler.SetWindow(view.Handle, new RECT(view.Bounds));
            handler.DoPreview();
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
            view.SafeDispose();
        }
    }
}
