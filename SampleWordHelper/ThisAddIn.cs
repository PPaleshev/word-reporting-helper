using System;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Ribbon;
using SampleWordHelper.Core;
using SampleWordHelper.Interface;
using SampleWordHelper.Presentation;
using Office = Microsoft.Office.Core;

namespace SampleWordHelper
{
    public partial class ThisAddIn
    {
        /// <summary>
        /// Экземпляр ленты. 
        /// Лента создаётся до вызова <see cref="ThisAddIn_Startup"/> в методе <see cref="CreateRibbonObjects"/> и существует одна на всё приложение.
        /// </summary>
        MainRibbon ribbon;

        /// <summary>
        /// Экземпляр основного менеджера приложения.
        /// </summary>
        MainPresenter presenter;

        /// <summary>
        /// Вызывается при старте приложения.
        /// К моменту вызова все сервисы проинициализированы и доступны.
        /// </summary>
        void ThisAddIn_Startup(object sender, EventArgs e)
        {
            var viewFactory = new ViewFactory(ribbon, CustomTaskPanes);
            var context = new RuntimeContext(Application, viewFactory, Globals.Factory);
            presenter = new MainPresenter(context);
            presenter.Start();
        }

        /// <summary>
        /// Вызывается при завершении работы надстройки.
        /// При этом не нужно уничтожать экземпляры лент (ribbons) и панелей задач (custom task panes), они будут очищены автоматически.
        /// </summary>
        void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            presenter.SafeDispose();
        }

        /// <summary>
        /// Вызывается после активации окна.
        /// </summary>
        void OnWindowActivated(Document doc, Window wn)
        {
            MessageBox.Show("OnWindowActivated: " + wn.Caption);
        }

        /// <summary>
        /// Вызывается после деактивации окна.
        /// </summary>
        void OnWindowDeactivated(Document doc, Window wn)
        {
            MessageBox.Show("OnWindowDeactivated: " + wn.Caption);
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InternalStartup()
        {
            Startup += ThisAddIn_Startup;
            Shutdown += ThisAddIn_Shutdown;
        }

        protected override IRibbonExtension[] CreateRibbonObjects()
        {
            return new IRibbonExtension[] {ribbon = new MainRibbon()};
        }

        #endregion
    }
}