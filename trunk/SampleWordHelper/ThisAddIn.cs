using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Office.Tools.Ribbon;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Interface;
using SampleWordHelper.Presentation;

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
        IDisposable presenterObject;

        /// <summary>
        /// Вызывается при старте приложения.
        /// К моменту вызова все сервисы проинициализированы и доступны.
        /// </summary>
        void ThisAddIn_Startup(object sender, EventArgs e)
        {
            //Заглушка на случай использования Word в качестве COM сервера для других целей. В этом случае надстройка загружаться не должна.
            if (Application.Windows.Count == 0)
            {
                presenterObject = new EmptyDisposable();
                return;
            }
            var viewFactory = new ViewFactory(ribbon, CustomTaskPanes);
            var context = new RuntimeContext(Application, viewFactory, Globals.Factory);
            var presenter = new MainPresenter(context);
            presenter.Start();
            presenterObject = presenter;
        }

        /// <summary>
        /// Вызывается при завершении работы надстройки.
        /// При этом не нужно уничтожать экземпляры лент (ribbons) и панелей задач (custom task panes), они будут очищены автоматически.
        /// </summary>
        void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            presenterObject.SafeDispose();
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