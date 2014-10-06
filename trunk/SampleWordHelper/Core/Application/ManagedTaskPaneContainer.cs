using System;
using System.Windows.Forms;
using Microsoft.Office.Tools;
using SampleWordHelper.Core.Common;

namespace SampleWordHelper.Core.Application
{
    /// <summary>
    /// Обёртка над <see cref="Microsoft.Office.Tools.CustomTaskPane"/>.
    /// Обеспечивает гарантированное и безопасное удаление панели при вызове <see cref="Dispose"/>.
    /// </summary>
    public sealed class ManagedTaskPaneContainer : BasicDisposable
    {
        /// <summary>
        /// Экземпляр панели задач.
        /// </summary>
        readonly CustomTaskPane pane;

        /// <summary>
        /// Коллекция, которой принадлежит панель задач.
        /// </summary>
        readonly CustomTaskPaneCollection parentCollection;

        public ManagedTaskPaneContainer(CustomTaskPaneCollection parentCollection, CustomTaskPane pane)
        {
            this.parentCollection = parentCollection;
            this.pane = pane;
        }

        /// <summary>
        /// Возвращает или устанавливает ширину панели.
        /// </summary>
        public int Width
        {
            get { return pane.Width; }
            set { pane.Width = value; }
        }

        /// <summary>
        /// Возвращает или устанавливает видимость панели.
        /// </summary>
        public bool Visible
        {
            get { return pane.Visible; }
            set { pane.Visible = value; }
        }

        /// <summary>
        /// Событие, возникающее при изменении видимости контейнера.
        /// </summary>
        public event EventHandler VisibleChanged
        {
            add { pane.VisibleChanged += value; }
            remove { pane.VisibleChanged -= value; }
        }

        /// <summary>
        /// Возвращает элемент управления, содержащийся в контейнере.
        /// </summary>
        public UserControl Control
        {
            get { return pane.Control; }
        }

        protected override void DisposeManaged()
        {
            parentCollection.Remove(pane);
        }
    }
}