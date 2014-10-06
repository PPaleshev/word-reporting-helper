using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Win32;

namespace SampleWordHelper.Core.Native
{
    /// <summary>
    /// Class used to traverse the registry to read all the file previewer registrations that exist on the system
    /// The majority of this code and logic comes from the Preview Handler Association Editor that Stephen Toub wrote
    /// and posted about on his blog.  http://blogs.msdn.com/toub/archive/2006/12/14/preview-handler-association-editor.aspx
    /// We made a few minor tweaks for our purposes, but the core of the logic is his.  Thanks to Stephen for sharing this code.
    /// </summary>
    class PreviewHandlerRegistry
    {
        /// <summary>
        /// Ключ реестра, содержащий описания обработчиков предварительного просмотра.
        /// </summary>
        const string BaseRegistryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\PreviewHandlers";

        /// <summary>
        /// Ключ реестра, указывающий на свойство обработчика предварительного просмотра.
        /// </summary>
        const string BaseClsIDKey = @"HKEY_CLASSES_ROOT\{0}\shellex\{{8895b1c6-b41f-4c1c-a562-0d564250836f}}";

        /// <summary>
        /// Ключ реестра, указывающий на свойство обработчика предварительного просмотра.
        /// </summary>
        const string BaseClsIdKey2 = @"HKEY_CLASSES_ROOT\SystemFileAssociations\{0}\shellex\{{8895b1c6-b41f-4c1c-a562-0d564250836f}}";


        /// <summary>
        /// Читает записи реестра и извлекает информацию о том, каким расширениям файлов какой CLSID обработчика предварительного просмотра соответствует.
        /// </summary>
        internal static HandlersInfo LoadHandlers()
        {
            var handlers = new List<PreviewHandlerInfo>();
            using (var handlersKey = Registry.LocalMachine.OpenSubKey(BaseRegistryKey))
                handlers.AddRange(handlersKey.GetValueNames().Select(id => new PreviewHandlerInfo(id, handlersKey.GetValue(id, null) as string)));
            handlers.Sort((first, second) =>
                              {
                                  if (first.name == null)
                                      return 1;
                                  if (second.name == null)
                                      return -1;
                                  return first.name.CompareTo(second.name);
                              });
            var handlerMap = handlers.ToDictionary(h => h.id);
            var extensions = Registry.ClassesRoot.GetSubKeyNames();
            var extensionInfos = new List<ExtensionInfo>(extensions.Length);
            foreach (var extension in extensions.Where(e => e.StartsWith(".")))
            {
                var id = Registry.GetValue(string.Format(BaseClsIDKey, extension), null, null) as string ??
                         Registry.GetValue(string.Format(BaseClsIdKey2, extension), null, null) as string;
                PreviewHandlerInfo mappedHandler;
                if (id != null && handlerMap.TryGetValue(id, out mappedHandler))
                    extensionInfos.Add(new ExtensionInfo(extension, mappedHandler));
            }
            return new HandlersInfo(handlers, extensionInfos);
        }
    }

    class HandlersInfo
    {
        public ReadOnlyCollection<PreviewHandlerInfo> Handlers { get; private set; }
        public ReadOnlyCollection<ExtensionInfo> Extensions { get; private set; }

        public HandlersInfo(IList<PreviewHandlerInfo> handlers, IList<ExtensionInfo> extensions)
        {
            Handlers = new ReadOnlyCollection<PreviewHandlerInfo>(handlers);
            Extensions = new ReadOnlyCollection<ExtensionInfo>(extensions);
        }
    }

    internal class PreviewHandlerInfo
    {
        public readonly string name;
        public readonly string id;

        public PreviewHandlerInfo(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    class ExtensionInfo
    {
        public readonly string extension;
        public readonly PreviewHandlerInfo handler;

        public ExtensionInfo(string extension, PreviewHandlerInfo handler)
        {
            this.extension = extension;
            this.handler = handler;
        }
    }
}