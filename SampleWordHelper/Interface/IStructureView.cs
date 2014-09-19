using SampleWordHelper.Core;
using SampleWordHelper.Model;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс представления структуры каталога.
    /// </summary>
    public interface IStructureView : IView
    {
        /// <summary>
        /// Инициализирует представление данными модели.
        /// </summary>
        void Initialize(StructureModel model);
    }
}
