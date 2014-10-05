namespace SampleWordHelper.Indexation
{
    /// <summary>
    /// Интерфейс для индикации прогресса выполнения операции. 
    /// </summary>
    public interface IProgressMonitor
    {
        /// <summary>
        /// Обновляет прогресс операции.
        /// </summary>
        /// <param name="action">Выполняемое действие.</param>
        /// <param name="maxValue">Максимальное значение прогресса операции.</param>
        /// <param name="currentValue">Текущий прогресс операции.</param>
        void UpdateProgress(string action, int maxValue, int currentValue);
    }
}
