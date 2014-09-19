namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс менеджера, управляющего экземпляром панели задач.
    /// </summary>
    public interface ITaskPanePresenter
    {
        /// <summary>
        /// Вызывается при закрытии пользователем панели задач.
        /// </summary>
        void OnPaneClosed();
    }
}
