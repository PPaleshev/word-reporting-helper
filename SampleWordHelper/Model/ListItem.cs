namespace SampleWordHelper.Model
{
    public class ListItem
    {
        public string DisplayName { get; private set; }
        public string Value { get; private set; }

        public ListItem(string displayName, string value)
        {
            DisplayName = displayName;
            Value = value;
        }
    }
}