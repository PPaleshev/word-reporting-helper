﻿using System;
using Microsoft.Office.Interop.Word;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Presentation;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Model
{
    public class MainPresenterState: BasicDisposable
    {
        /// <summary>
        /// Слушатель событий окружения.
        /// </summary>
        readonly ApplicationEventsListener eventListener;

        /// <summary>
        /// Конфигурация приложения.
        /// </summary>
        readonly ConfigurationModel configuration;
        
        /// <summary>
        /// Контекст выполнения приложения.
        /// </summary>
        readonly ApplicationContext context;

        /// <summary>
        /// Объект для отслеживания состояния документов.
        /// </summary>
        DocumentManager DocumentManager { get; set; }

        /// <summary>
        /// Экземпляр активного поставщика каталога.
        /// </summary>
        Provider Provider { get; set; }


        public bool IsValid { get; private set; }
        public string Message { get; private set; }

        public MainPresenterState(ApplicationContext context, ICatalogPaneCallback callback)
        {
            this.context = context;
            DocumentManager = new DocumentManager(context, callback);
            eventListener = new ApplicationEventsListener(context.Environment, DocumentManager);
            configuration = new ConfigurationModel("reportHelper");
            Provider = new Provider(new NullProviderStrategy());
            InitializeAndValidate();
            eventListener.StartListen();
        }

        /// <summary>
        /// Проверяет состояние провайдера.
        /// Если он успешно инициализировался, делает доступными все элементы управления надстройкой, в противном случае требует повторной настройки.
        /// </summary>
        bool InitializeAndValidate()
        {
            if (!configuration.HasConfiguredProvider)
                return SetInvalid("Требуется выбор поставщика каталога.");
            Provider = new Provider(configuration.GetConfiguredProviderStrategy());
            if (!Provider.Initialize(context.Environment))
                return SetInvalid("Требуется настройка текущего поставщика каталога.");
            Message = null;
            return IsValid = true;
        }

        public ConfigurationEditorModel CreateEditorModel()
        {
            return configuration.CreateEditorModel();
        }

        public void UpdateConfiguraion(ConfigurationEditorModel model)
        {
            Provider.Shutdown();
            configuration.Update(model);
            Provider = new Provider(configuration.GetConfiguredProviderStrategy());
            InitializeAndValidate();
        }

        public void UpdateCatalog()
        {
            context.Catalog = Provider.LoadCatalog();
            DocumentManager.UpdateCatalog();
        }

        public void ShowCatalog(bool visible)
        {
            DocumentManager.UpdateCatalogVisibility(visible);
        }

        bool SetInvalid(string message)
        {
            IsValid = false;
            Message = message;
            return false;
        }

        protected override void DisposeManaged()
        {
            eventListener.SafeDispose();
            DocumentManager.SafeDispose();
            Provider.Shutdown();
        }

        public IDisposable SuspendEvents()
        {
            return eventListener.SuspendEvents();
        }
    }
}