using System;
using System.Diagnostics;
using Microsoft.Office.Interop.Word;
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
        /// Контекст времени выполнения приложения.
        /// </summary>
        readonly ApplicationContext context;

        /// <summary>
        /// Основное представление приложения.
        /// </summary>
        readonly IMainView view;

        /// <summary>
        /// Слушатель событий окружения.
        /// </summary>
        readonly ApplicationEventsListener eventListener;

        /// <summary>
        /// Объект для отслеживания состояния документов.
        /// </summary>
        readonly DocumentManager documentManager;

        /// <summary>
        /// Механизм поиска.
        /// </summary>
        SearchEngine searchEngine;

        /// <summary>
        /// Модель конфигурации приложения.
        /// </summary>
        ConfigurationModel configurationModel;

        /// <summary>
        /// Экземпляр провайдера.
        /// </summary>
        Provider provider;

        /// <summary>
        /// Создаёт экземпляр основного менеджера приложения.
        /// </summary>
        /// <param name="runtimeContext">Контекст времени исполнения приложения.</param>
        public MainPresenter(IRuntimeContext runtimeContext)
        {
            searchEngine = new SearchEngine(new WordDocumentContentProvider(runtimeContext.Application));
            context = new ApplicationContext(runtimeContext, new EmptyCatalog(), searchEngine);
            view = runtimeContext.ViewFactory.CreateMainView(this);
            documentManager = new DocumentManager(context, this);
            eventListener = new ApplicationEventsListener(runtimeContext, documentManager);
        }

        /// <summary>
        /// Начинает выполнение презентера.
        /// </summary>
        public void Start()
        {
            configurationModel = new ConfigurationModel("reportHelper");
            provider = new Provider(configurationModel.GetConfiguredProviderStrategy());
            if (InitializeAndValidate())
                UpdateCatalog();
            eventListener.Listen();
        }

        public void OnEditSettings()
        {
            var editorModel = configurationModel.CreateEditorModel();
            using (var presenter = new ConfigurationEditorPresenter(context.Environment.ViewFactory, editorModel))
            {
                if (!presenter.Edit())
                    return;
                provider.Shutdown();
                configurationModel.Update(editorModel);
                provider = new Provider(configurationModel.GetConfiguredProviderStrategy());
                if (InitializeAndValidate())
                    UpdateCatalog();
            }
        }

        public void OnUpdateCatalog()
        {
            UpdateCatalog();
        }

        public void OnUpdateCatalogVisibility(bool visible)
        {
            documentManager.UpdateCatalogVisibility(visible);
        }

        public void OnVisibilityChanged(bool visible)
        {
            view.SetCatalogButtonPressed(visible);
        }

        /// <summary>
        /// Проверяет состояние провайдера.
        /// Если он успешно инициализировался, делает доступными все элементы управления надстройкой, в противном случае требует повторной настройки.
        /// </summary>
        bool InitializeAndValidate()
        {
            string message = null;
            if (!configurationModel.HasConfiguredProvider)
                message = "Требуется выбор поставщика каталога.";
            else if (!provider.Initialize(context.Environment))
                message = "Требуется настройка текущего поставщика каталога.";
            var isSuccess = string.IsNullOrWhiteSpace(message);
            view.EnableAddinFeatures(isSuccess, message);
            return isSuccess;
        }

        /// <summary>
        /// Перезагружает текущий каталог и обновляет его данные.
        /// </summary>
        void UpdateCatalog()
        {
            context.Catalog = provider.LoadCatalog();
            documentManager.UpdateCatalog();
            Debug.WriteLine("Update catalog");
            using (var presenter = new SearchIndexPresenter(context.Environment))
            using (eventListener.SuspendEvents())
                presenter.Run(context.Catalog, searchEngine);
        }

        protected override void DisposeManaged()
        {
            try
            {
                provider.Shutdown();
            }
            catch
            {
                //TODO log it
            }
            view.SafeDispose();
            documentManager.SafeDispose();
            searchEngine.SafeDispose();
        }
    }
}