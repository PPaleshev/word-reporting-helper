namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Менеджер предварительного просмотра.
    /// </summary>
    public interface IPreviewPresenter
    {
        /// <summary>
        /// Вызывается при изменении размеров представления.
        /// </summary>
        void OnSizeChanged();

        /// <summary>
        /// Вызывается при запросе вставки просматриваемого документа.
        /// </summary>
        void OnPaste();

        /// <summary>
        /// Вызывается при закрытии представления.
        /// </summary>
        void OnClose();
    }
}
