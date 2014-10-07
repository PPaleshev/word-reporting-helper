using System;
using System.Diagnostics;
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
    public class PreviewPresenter : BasicDisposable, IPreviewPresenter
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
            view.ShowLoading(Path.GetFileName(unsafeFilePath));
            var data = PreviewHandlerRegistry.LoadHandlers();
            var ext = Path.GetExtension(unsafeFilePath);
            var handlerInfo = data.Extensions.Where(info => string.Equals(info.extension, ext, StringComparison.InvariantCultureIgnoreCase)).Select(info => info.handler).FirstOrDefault();
            if (handlerInfo == null)
            {
                view.CompleteLoading(false, "Не удалось инициировать предварительный просмотр.");
                return;
            }
            try
            {
                InitializeHandler(handlerInfo.id);
                view.CompleteLoading(true, null);
            }
            catch (Exception)
            {
                view.CompleteLoading(false, "Предварительный просмотр недоступен из-за возникшей ошибки");
            }
        }

        void IPreviewPresenter.OnSizeChanged()
        {
            var rect = new RECT(view.PreviewArea);
            handler.SetRect(ref rect);
        }

        void IPreviewPresenter.OnClose()
        {
            UnloadHandler();
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
            handler.SetWindow(view.Handle, new RECT(view.PreviewArea));
            handler.DoPreview();
            var rect = new RECT(view.PreviewArea);
            handler.SetRect(ref rect);
        }

        void UnloadHandler()
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

        protected override void DisposeManaged()
        {
            UnloadHandler();
            view.SafeDispose();
        }
    }
}
