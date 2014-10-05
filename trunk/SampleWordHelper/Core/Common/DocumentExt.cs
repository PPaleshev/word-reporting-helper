using Microsoft.Office.Tools.Word;
using SampleWordHelper.Core.Application;

namespace SampleWordHelper.Core.Common
{
    /// <summary>
    /// Вспомогательные методы.
    /// </summary>
    public static class DocumentExt
    {
        /// <summary>
        /// Возвращает ключ, уникально идентифицирующий документ.
        /// </summary>
        public static object GetKey(this Document document)
        {
            return document.DocID;
        }

        /// <summary>
        /// Возвращает идентификатор активного в данный момент документа.
        /// </summary>
        public static object GetActiveDocumentKey(this IRuntimeContext context)
        {
            var vsto = context.ApplicationFactory.GetVstoObject(context.Application.ActiveDocument);
            return vsto.GetKey();
        }
    }
}
