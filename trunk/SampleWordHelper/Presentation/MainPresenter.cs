using System;
using System.Windows.Forms;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Менеджер главного представления.
    /// </summary>
    public class MainPresenter : BasicDisposable, IMainPresenter
    {
        /// <summary>
        /// Контекст времени выполнения приложения.
        /// </summary>
        readonly IRuntimeContext context;

        /// <summary>
        /// Основное представление приложения.
        /// </summary>
        readonly IMainView view;

        /// <summary>
        /// Объект для отслеживания состояния документов.
        /// </summary>
        DocumentManager documentManager;

        /// <summary>
        /// Модель конфигурации приложения.
        /// </summary>
        ConfigurationModel configurationModel;

        /// <summary>
        /// Модель каталога.
        /// </summary>
        ICatalog catalog;

        /// <summary>
        /// Экземпляр провайдера.
        /// </summary>
        Provider provider;

        /// <summary>
        /// Создаёт экземпляр основного менеджера приложения.
        /// </summary>
        /// <param name="context">Контекст времени исполнения приложения.</param>
        public MainPresenter(IRuntimeContext context)
        {
            this.context = context;
            view = context.ViewFactory.CreateMainView(this);
        }

        /// <summary>
        /// Начинает выполнение презентера.
        /// </summary>
        public void Start()
        {
            configurationModel = new ConfigurationModel("reportHelper");
            documentManager = new DocumentManager(context);
            provider = new Provider(configurationModel.GetConfiguredProviderStrategy());
            if (InitializeAndValidate())
                UpdateCatalog();
        }

        public void OnEditSettings()
        {
            var editorModel = configurationModel.CreateEditorModel();
            using (var presenter = new ConfigurationEditorPresenter(context.ViewFactory, editorModel))
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

        /// <summary>
        /// Проверяет состояние провайдера.
        /// Если он успешно инициализировался, делает доступными все элементы управления надстройкой, в противном случае требует повторной настройки.
        /// </summary>
        bool InitializeAndValidate()
        {
            string message = null;
            if (!configurationModel.HasConfiguredProvider)
                message = "Требуется выбор поставщика каталога.";
            else if (!provider.Initialize(context))
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
            try
            {
                catalog = provider.LoadCatalog();
                documentManager.UpdateCatalog(catalog);
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось обновить данные каталога.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //TODO PP: Log error
            }
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
        }
    }
}