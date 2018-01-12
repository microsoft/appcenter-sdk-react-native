namespace Contoso.Forms.Puppet
{
    public class Property
    {
        public string Name { get; private set; }

        public string Value { get; private set; }

        public Property(string propertyName, string propertyValue)
        {
            Name = propertyName;
            Value = propertyValue;
        }
    }
}