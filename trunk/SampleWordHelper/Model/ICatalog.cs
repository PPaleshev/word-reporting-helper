using System.Collections.Generic;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Интерфейс доступа к иерархическому каталогу.
    /// </summary>
    public interface ICatalog
    {
        /// <summary>
        /// Возвращает идентификаторы корневых элементов каталога.
        /// </summary>
        IEnumerable<string> GetRootElements();

        /// <summary>
        /// Возвращает идентификаторы дочерних элементов.
        /// </summary>
        /// <param name="parentId">Идентификатор корневого элемента.</param>
        IEnumerable<string> GetChildElements(string parentId);

        /// <summary>
        /// Возвращает перечисление всех элементов каталога.
        /// </summary>
        IEnumerable<string> All();

        /// <summary>
        /// Возвращает название элемента по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор элемента.</param>
        string GetName(string id);

        /// <summary>
        /// Возвращает описание элемента по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор элемента.</param>
        string GetDescription(string id);

        /// <summary>
        /// Возвращает расположение элемента в файловой системе.
        /// </summary>
        /// <param name="id">Идентификатор элемента.</param>
        string GetLocation(string id);

        /// <summary>
        /// Возвращает true, в случае элемент с идентификатором представляет группу, в противном случае false.
        /// </summary>
        /// <param name="id">Идентификатор элемента.</param>
        bool IsGroup(string id);

        /// <summary>
        /// Возвращает true, если элемент с указанным идентификатором содержится в каталоге, иначе false.
        /// </summary>
        /// <param name="id">Идентификатор элемента.</param>
        bool Contains(string id);
    }
}
