using System;

namespace ListsTestTask.Models
{
    [Serializable]
    public class OptionField : IOption
    {
        public string Name { get; set; }

        public OptionField(string name)
        {
            Name = name;
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

