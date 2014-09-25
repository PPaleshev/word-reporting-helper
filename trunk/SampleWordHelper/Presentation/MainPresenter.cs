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
            var activeProvider = GetActiveProvider();
            if (!ValidateProviderState(activeProvider))
                return;
            catalog = activeProvider.LoadCatalog(CatalogLoadMode.PARTIAL);
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
                var activeProvider = GetActiveProvider();
                if (result.providerChanged)
                {
                    if (result.previousProvider != null)
                        result.previousProvider.Shutdown();
                    activeProvider.Initialize(context);
                }
                activeProvider.ApplyConfiguration(editorModel.ProviderSettingsModel);
                ValidateProviderState(activeProvider);
            }
        }

        public void OnUpdateCatalog()
        {
            var activeProvider = GetActiveProvider();
            try
            {
                catalog = activeProvider.LoadCatalog(CatalogLoadMode.FULL);
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
        bool ValidateProviderState(ICatalogProvider activeProvider)
        {
            string message = null;
            if (activeProvider == null)
                message = "Требуется выбор поставщика каталога.";
            else if (!activeProvider.Initialize(context))
                message = "Требуется настройка текущего поставщика каталога.";
            var isSuccess = string.IsNullOrWhiteSpace(message);
            view.EnableAddinFeatures(isSuccess, message);
            return isSuccess;
        }

        /// <summary>
        /// Возвращает экземпляр активного поставщика каталога.
        /// </summary>
        ICatalogProvider GetActiveProvider()
        {
            return configurationModel.GetActiveProvider();
        }

        protected override void DisposeManaged()
        {
            try
            {
                var activeProvider = GetActiveProvider();
                if (activeProvider != null)
                    activeProvider.Shutdown();
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