using System;
using System.Windows.Forms;
using NLog;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Indexation;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.Core;
using ApplicationContext = SampleWordHelper.Core.Application.ApplicationContext;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Менеджер главного представления.
    /// </summary>
    public class MainPresenter : BasicDisposable, IMainPresenter, ICatalogPaneCallback
    {
        /// <summary>
        /// Поддержка логирования.
        /// </summary>
        static Logger LOG = LogManager.GetCurrentClassLogger(); 

        /// <summary>
        /// Контекст времени выполнения приложения.
        /// </summary>
        readonly ApplicationContext context;

        /// <summary>
        /// Основное представление приложения.
        /// </summary>
        readonly IMainView view;

        /// <summary>
        /// Состояние.
        /// </summary>
        readonly MainModel model;

        /// <summary>
        /// Механизм поиска.
        /// </summary>
        readonly SearchEngine searchEngine;

        /// <summary>
        /// Создаёт экземпляр основного менеджера приложения.
        /// </summary>
        /// <param name="runtimeContext">Контекст времени исполнения приложения.</param>
        public MainPresenter(IRuntimeContext runtimeContext)
        {
            searchEngine = new SearchEngine(new WordDocumentContentProvider(runtimeContext.Application));
            context = new ApplicationContext(runtimeContext, new EmptyCatalog(), searchEngine);
            view = runtimeContext.ViewFactory.CreateMainView(this);
            model = new MainModel(context, this);
        }

        /// <summary>
        /// Начинает выполнение презентера.
        /// </summary>
        public void Start()
        {
            view.EnableAddinFeatures(model.IsActive, model.IsValid, model.Message);
        }

        public void OnEnabledChanged(bool enabled)
        {
            if (model.IsActive == enabled)
                return;
            if (enabled)
                Activate();
            else
                Deactivate();
        }

        public void OnEditSettings()
        {
            var editorModel = model.CreateEditorModel();
            using (var presenter = new ConfigurationEditorPresenter(context.Environment.ViewFactory, editorModel))
            {
                if (!presenter.Edit())
                    return;
                LOG.Info("Updating settings: {0}", editorModel.SelectedProviderName);
                model.UpdateConfiguraion(editorModel);
                view.EnableAddinFeatures(model.IsActive, model.IsValid, model.Message);
                if (model.IsActive && model.IsValid)
                    UpdateCatalog();
            }
        }

        public void OnUpdateCatalog()
        {
            UpdateCatalog();
        }

        public void OnUpdateCatalogVisibility(bool visible)
        {
            model.ShowCatalogPane(visible);
        }

        public void OnVisibilityChanged(bool visible)
        {
            view.SetCatalogButtonPressed(visible);
        }

        /// <summary>
        /// Перезагружает текущий каталог и перестраивает поисковой индекс.
        /// </summary>
        void UpdateCatalog()
        {
            try
            {
                LOG.Info("Updating catalog");
                model.UpdateCatalog();
                LOG.Info("Indexing catalog");
                using (model.SuspendUpdates())
                using (var presenter = new SearchIndexPresenter(context.Environment))
                    presenter.Run(context.Catalog, searchEngine);
            }
            catch (CatalogLoadException cle)
            {
                LOG.Error("Error loading catalog", (Exception) cle);
                MessageBox.Show(string.Format("При загрузке каталога возникла ошибка: {0}", cle.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Выполняет активацию аддина.
        /// </summary>
        void Activate()
        {
            LOG.Info("Activating");
            model.Activate();
            LOG.Info("MainPresenterState: Valid={0}; Message={1}; ShowWindowsInTaskBar={2}", model.IsValid, model.Message, context.Environment.Application.ShowWindowsInTaskbar);
            view.EnableAddinFeatures(model.IsActive, model.IsValid, model.Message);
            model.ShowCatalogPane(model.IsValid);
            if (!model.IsValid)
                return;
            UpdateCatalog();
        }

        /// <summary>
        /// Выполянет деактивацию аддина.
        /// </summary>
        void Deactivate()
        {
            LOG.Info("Deactivating presenter");
            model.Shutdown();
            view.EnableAddinFeatures(model.IsActive, model.IsValid, "");
        }

        protected override void DisposeManaged()
        {
            view.SafeDispose();
            model.SafeDispose();
            searchEngine.SafeDispose();
        }
    }
}