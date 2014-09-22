﻿using System.Configuration;
using SampleWordHelper.Configuration;
using SampleWordHelper.Core;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;

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
        /// Создаёт экземпляр основного менеджера приложения.
        /// </summary>
        /// <param name="context">Контекст времени исполнения приложения.</param>
        public MainPresenter(IRuntimeContext context)
        {
            this.context = context;
            view = context.ViewFactory.CreateMainView(this);
        }

        public IRuntimeContext Context
        {
            get { return context; }
        }

        /// <summary>
        /// Начинает выполнение презентера.
        /// </summary>
        public void Start()
        {
            configurationModel = new ConfigurationModel("reportHelper");
            documentManager = new DocumentManager(context);
            var activeProvider = configurationModel.GetActiveProvider();
            if (activeProvider != null)
                activeProvider.Initialize(context);
        }

        public void OnEditSettings()
        {
            var editorModel = configurationModel.CreateEditorModel();
            using (var presenter = new ConfigurationEditorPresenter(context.ViewFactory, editorModel))
            {
                if (!presenter.Edit())
                    return;
                var result = configurationModel.Update(editorModel);
                var activeProvider = configurationModel.GetActiveProvider();
                if (result.providerChanged)
                {
                    if (result.previousProvider != null)
                        result.previousProvider.Shutdown();
                    activeProvider.Initialize(context);
                }
                activeProvider.ApplyConfiguration(editorModel.ProviderSettingsModel);
            }
        }

        protected override void DisposeManaged()
        {
            view.SafeDispose();
            documentManager.SafeDispose();
        }
    }
}