namespace SampleWordHelper.Model
{
    public class Item<T>
    {
        public string DisplayName { get; private set; }
        public T Object { get; private set; }

        public Item(string displayName, T o)
        {
            DisplayName = displayName;
            Object = o;
        }
    }
}