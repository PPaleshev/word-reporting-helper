﻿using Microsoft.Office.Tools;
using SampleWordHelper.Interface;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Core
{
    /// <summary>
    /// Экземпляр фабрики представлений.
    /// </summary>
    public class ViewFactory: IViewFactory
    {
        /// <summary>
        /// Экземпляр ленты, которая делится между всеми экземплярами окон.
        /// </summary>
        readonly MainRibbon ribbon;

        /// <summary>
        /// Фабрика контейнеров для панелей задач.
        /// </summary>
        readonly CustomTaskPaneCollection paneFactory;

        /// <summary>
        /// Создаёт экземпляр фабрики представлений.
        /// </summary>
        /// <param name="ribbon">Экземпляр ленты с элементами управления.</param>
        /// <param name="paneFactory">Фабрика контейнеров для панелей задач.</param>
        public ViewFactory(MainRibbon ribbon, CustomTaskPaneCollection paneFactory)
        {
            this.ribbon = ribbon;
            this.paneFactory = paneFactory;
        }

        public IRibbonView CreateRibbonView(IRibbonPresenter presenter)
        {
            return new RibbonView(ribbon, presenter);
        }

        public IStructureView CreateStructureView(IStructurePresenter presenter, string title)
        {
            var control = new StructureTreeControl();
            var container = paneFactory.Add(control, title);
            return new StructureTreeView(container, presenter);
        }

        public ISettingsEditorView CreateSettingsView(ISettingsEditorPresenter presenter)
        {
            return new SettingsForm(presenter);
        }
    }
}
