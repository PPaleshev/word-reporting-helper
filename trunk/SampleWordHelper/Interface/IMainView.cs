namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс основного представления.
    /// </summary>
    public interface IMainView
    {
        /// <summary>
        /// Делает панель структуры видимой или скрывает её в зависимости от флага.
        /// </summary>
        void SetStructureVisibility(bool value);
    }
}