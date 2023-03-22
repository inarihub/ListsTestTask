namespace ListsTestTask.Models
{
    public class OptionField
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public OptionField(string name, bool isSelected)
        {
            Name = name;
            IsSelected = isSelected;
        }
        public override bool Equals(object? obj)
        {
            return obj is OptionField field && this.Name == field.Name;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}

