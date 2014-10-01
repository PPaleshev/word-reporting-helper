using System.Runtime.InteropServices;
using SampleWordHelper.Core;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Фильтр для событий ленты.
    /// Вызывает события оборачиваемого менеджера только в том случае, если управляемый этим менеджером документ является активным в данный момент.
    /// </summary>
    public class RibbonEventFilter : IRibbonEventListener
    {
        /// <summary>
        /// Внутренний презентер, которому делегируются события.
        /// </summary>
        readonly IRibbonEventListener innerPresenter;

        /// <summary>
        /// Ключ документа, которым управляет менеджер.
        /// </summary>
        readonly object key;

        /// <summary>
        /// Контекст исполнения надстройки.
        /// </summary>
        readonly IRuntimeContext context;

        /// <summary>
        /// Создаёт новый экземпляр фильтра событий.
        /// </summary>
        /// <param name="context">Контекст времени исполнения надстройки.</param>
        /// <param name="innerPresenter">Менеджер ленты.</param>
        /// <param name="documentKey">Уникальный ключ документа, которым управляет менеджер.</param>
        public RibbonEventFilter(IRuntimeContext context, IRibbonEventListener innerPresenter, object documentKey)
        {
            key = documentKey;
            this.context = context;
            this.innerPresenter = innerPresenter;
        }

        public void OnToggleStructureVisibility()
        {
            if (IsFilterActive)
                innerPresenter.OnToggleStructureVisibility();
        }

        /// <summary>
        /// Возвращает true, если возникающие события могут быть делегированы <see cref="innerPresenter"/>, иначе false.
        /// </summary>
        bool IsFilterActive
        {
            get
            {
                try
                {
                    var doc = context.Application.ActiveDocument;
                    if (doc == null)
                        return false;
                    var vsto = context.ApplicationFactory.GetVstoObject(doc);
                    return key.Equals(vsto.GetKey());
                }
                catch (COMException)
                {
                    return false;
                }
            }
        }
    }
}
