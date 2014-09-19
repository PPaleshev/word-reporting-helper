using Microsoft.Office.Tools.Word;

namespace SampleWordHelper.Core
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
    }
}
