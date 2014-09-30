using System;
using System.Windows.Forms;
using SampleWordHelper.Core;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Presentation
{
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
        /// Экземпляр поставщика данных.
        /// </summary>
        Provider provider;

        /// <summary>
        /// Модель конфигурации приложения.
        /// </summary>
        ConfigurationModel configurationModel;

        /// <summary>
        /// Модель каталога.
        /// </summary>
        CatalogModel catalog;

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
            provider = new Provider(configurationModel.GetCurrentProviderStrategy());
            if (!ValidateProviderState())
                return;
            catalog = provider.LoadCatalog(CatalogLoadMode.PARTIAL);
            documentManager.SetCatalog(catalog);
        }

        public void OnEditSettings()
        {
            var editorModel = configurationModel.CreateEditorModel();
            using (var presenter = new ConfigurationEditorPresenter(context.ViewFactory, editorModel))
            {
                if (!presenter.Edit())
                    return;
                var result = configurationModel.Update(editorModel);
                if (result.providerChanged)
                {
                    provider.Shutdown();
                    provider = new Provider(configurationModel.GetCurrentProviderStrategy());
                    provider.Initialize(context);
                }
                provider.ApplyConfiguration(editorModel.ProviderSettingsModel);
                ValidateProviderState();
            }
        }

        public void OnUpdateCatalog()
        {
            try
            {
                catalog = provider.LoadCatalog(CatalogLoadMode.FULL);
                documentManager.SetCatalog(catalog);
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось обновить данные каталога.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //TODO PP: Log error
            }
        }

        /// <summary>
        /// Проверяет состояние провайдера.
        /// Если он успешно инициализировался, делает доступными все элементы управления надстройкой, в противном случае требует повторной настройки.
        /// </summary>
        bool ValidateProviderState()
        {
            string message = null;
            if (string.IsNullOrWhiteSpace(configurationModel.CurrentProviderName))
                message = "Требуется выбор поставщика каталога.";
            else if (!provider.Initialize(context))
                message = "Требуется настройка текущего поставщика каталога.";
            var isSuccess = string.IsNullOrWhiteSpace(message);
            view.EnableAddinFeatures(isSuccess, message);
            return isSuccess;
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