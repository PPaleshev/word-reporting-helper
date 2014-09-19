﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SampleWordHelper.Core;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Класс для отслеживания состояния открываемых и создаваемых документов.
    /// </summary>
    public class DocumentManager: IDisposable
    {
        /// <summary>
        /// Контекст работы плагина.
        /// </summary>
        readonly IRuntimeContext context;

        /// <summary>
        /// Отображение из ключа документа в управляющий им менеджер.
        /// </summary>
        readonly Dictionary<object, DocumentPresenter> documents = new Dictionary<object, DocumentPresenter>();

        /// <summary>
        /// Создаёт новый экземпляр менеджера документов.
        /// </summary>
        public DocumentManager(IRuntimeContext context)
        {
            this.context = context;
            var application = context.Application;
            ((ApplicationEvents4_Event) application).NewDocument += OnNewDocument;
            application.DocumentOpen += OnDocumentOpened;
            application.DocumentChange += OnDocumentChanged;
        }

        /// <summary>
        /// Добавляет документ в список активных.
        /// </summary>
        void RegisterDocument(Document document)
        {
            var toolDoc = context.ApplicationFactory.GetVstoObject(document);
            var key = toolDoc.GetKey();
            toolDoc.Shutdown += (sender, args) => OnDocumentShutdown(key);
            var presenter = new DocumentPresenter(context, toolDoc);
            documents.Add(key, presenter);
            presenter.Run();
        }

        /// <summary>
        /// Пытается получить презентер для указанного документа.
        /// </summary>
        bool TryGetPresenter(Document document, out DocumentPresenter presenter)
        {
            var vsto = context.ApplicationFactory.GetVstoObject(document);
            return documents.TryGetValue(vsto.GetKey(), out presenter);
        }

        /// <summary>
        /// Вызывается при открытии нового документа.
        /// </summary>
        /// <remarks>Для нового документа признак сохранения устанавливается в <c>false</c> для предотвращения его "перезаписывания" при открытии существующего документа.</remarks>
        void OnNewDocument(Document newDocument)
        {
            newDocument.Saved = false;
            RegisterDocument(newDocument);
        }

        /// <summary>
        /// Вызывается после открытия нового документа.
        /// </summary>
        /// <param name="doc">Открытый документ.</param>
        void OnDocumentOpened(Document doc)
        {
            RegisterDocument(doc);
        }

        /// <summary>
        /// Вызывается после окончательного закрытия документа.
        /// </summary>
        void OnDocumentShutdown(object documentKey)
        {
            DocumentPresenter presenter;
            if (!documents.TryGetValue(documentKey, out presenter))
            {
                MessageBox.Show("OnDocumentClosing: no presenter found for this document (" + documentKey + ")");
                return;
            }
            documents.Remove(documentKey);
            presenter.Dispose();
        }

        /// <summary>
        /// Вызывается при изменении активного документа.
        /// </summary>
        void OnDocumentChanged()
        {
            try
            {
                if (context.Application.Documents.Count == 0)
                    return;
                var activeDocument = context.Application.ActiveDocument;
                if (activeDocument == null)
                    return;
                DocumentPresenter presenter;
                if (TryGetPresenter(activeDocument, out presenter))
                    presenter.Activate();
            }
            catch (COMException e)
            {
                MessageBox.Show("OnDocumentChanged: " + e.StackTrace);
            }
        }

        public void Dispose()
        {
            var application = context.Application;
            if (application == null)
                return;
            ((ApplicationEvents4_Event) application).NewDocument -= OnNewDocument;
            application.DocumentOpen -= OnDocumentOpened;
            application.DocumentChange -= OnDocumentChanged;
        }
    }
}