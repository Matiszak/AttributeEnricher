namespace AttributeEnricher.Tests.Models
{
    class ObjectWithProperties
    {
        [Modify]
        public string PropertyWithAttribute { get; set; }
        public string Property { get; set; }
    }
}
