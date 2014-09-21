using System;
using Microsoft.Office.Tools;
using SampleWordHelper.Model;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Представление для отображения структуры каталога.
    /// </summary>
    public class StructureTreeView: IStructureView
    {
        readonly StructureTreeControl control;
        readonly CustomTaskPane container;
        readonly IStructurePresenter presenter;

        public StructureTreeView(CustomTaskPane container, IStructurePresenter presenter)
        {
            control = (StructureTreeControl) container.Control;
            this.container = container;
            this.presenter = presenter;
            InitializeComponents();
        }

        public object RawObject
        {
            get { return control; }
        }

        public void Initialize(StructureModel model)
        {
        }

        public void SetVisibility(bool value)
        {
            container.Visible = value;
        }

        public void Dispose()
        {
            control.Dispose();
            container.VisibleChanged -= ContainerVisibilityChanged;
        }

        /// <summary>
        /// Выполняет инициализацию компонентов представления.
        /// </summary>
        void InitializeComponents()
        {
            container.VisibleChanged += ContainerVisibilityChanged;
            container.Width = 400;
        }

        /// <summary>
        /// Вызывается при изменении видимости панели со структурой каталога.
        /// </summary>
        void ContainerVisibilityChanged(object sender, EventArgs e)
        {
            if (!container.Visible)
                presenter.OnClosed();
        }
    }
}
