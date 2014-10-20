using NLog;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Indexation;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;
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
        MainPresenterState model;

        /// <summary>
        /// Механизм поиска.
        /// </summary>
        readonly SearchEngine searchEngine;

        /// <summary>
        /// True, если приложение активно в данный момент, иначе false.
        /// </summary>
        bool isActive;

        /// <summary>
        /// Создаёт экземпляр основного менеджера приложения.
        /// </summary>
        /// <param name="runtimeContext">Контекст времени исполнения приложения.</param>
        public MainPresenter(IRuntimeContext runtimeContext)
        {
            searchEngine = new SearchEngine(new WordDocumentContentProvider(runtimeContext.Application));
            context = new ApplicationContext(runtimeContext, new EmptyCatalog(), searchEngine);
            view = runtimeContext.ViewFactory.CreateMainView(this);
        }

        /// <summary>
        /// Начинает выполнение презентера.
        /// </summary>
        public void Start()
        {
            isActive = false;
            view.EnableAddinFeatures(isActive, false, "");
        }

        public void OnEnabledChanged(bool enabled)
        {
            if (isActive == enabled)
                return;
            if (enabled)
                Activate();
            else
                Deactivate();
        }

        public void OnEditSettings()
        {
            if(!isActive)
                return;
            var editorModel = model.CreateEditorModel();
            using (var presenter = new ConfigurationEditorPresenter(context.Environment.ViewFactory, editorModel))
            {
                if (!presenter.Edit())
                    return;
                LOG.Info("Updating settings: {0}", editorModel.SelectedProviderName);
                model.UpdateConfiguraion(editorModel);
                view.EnableAddinFeatures(isActive, model.IsValid, model.Message);
                if (model.IsValid)
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
            model.UpdateCatalog();
            LOG.Info("Updating catalog");
            model.UpdateCatalog();
            LOG.Info("Indexing catalog");
            using (model.SuspendUpdates())
            using (var presenter = new SearchIndexPresenter(context.Environment))
                presenter.Run(context.Catalog, searchEngine);
        }

        /// <summary>
        /// Выполняет активацию аддина.
        /// </summary>
        void Activate()
        {
            LOG.Info("Activating presenter");
            isActive = true;
            model = new MainPresenterState(context, this);
            LOG.Info("MainPresenterState: Valid={0}; Message={1}; ShowWindowsInTaskBar={2}", model.IsValid, model.Message, context.Environment.Application.ShowWindowsInTaskbar);
            view.EnableAddinFeatures(isActive, model.IsValid, model.Message);
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
            isActive = false;
            model.SafeDispose();
            view.EnableAddinFeatures(false, false, "");
        }

        protected override void DisposeManaged()
        {
            view.SafeDispose();
            model.SafeDispose();
            searchEngine.SafeDispose();
        }
    }
}