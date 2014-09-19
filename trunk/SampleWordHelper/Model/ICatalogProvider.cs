namespace SampleWordHelper.Model
{
    /// <summary>
    /// Интерфейс компонента, предоставляющего доступ.
    /// </summary>
    public interface ICatalogProvider
    {
        CatalogModel LoadCatalog();
    }
}
