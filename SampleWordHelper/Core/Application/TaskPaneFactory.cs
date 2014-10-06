using System.Windows.Forms;
using Microsoft.Office.Tools;

namespace SampleWordHelper.Core.Application
{
    /// <summary>
    /// Фабрика панелей задач.
    /// </summary>
    public class TaskPaneFactory
    {
        /// <summary>
        /// Коллекция панелей задач, принадлежащих надстройке.
        /// </summary>
        readonly CustomTaskPaneCollection collection;

        /// <summary>
        /// Создаёт новый экземпляр фабрики.
        /// </summary>
        public TaskPaneFactory(CustomTaskPaneCollection collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// Создаёт панель задач.
        /// </summary>
        /// <param name="control">Элемент управления, отображаемый на панели.</param>
        /// <param name="title">Заголовок панели задач.</param>
        public ManagedTaskPane Create(UserControl control, string title)
        {
            var pane = collection.Add(control, title);
            return new ManagedTaskPane(collection, pane);
        }
    }
}