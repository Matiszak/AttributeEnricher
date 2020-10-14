namespace AttributeEnricher.PerformanceTests.Models
{
    class ObjectWithProperties
    {
        [Modify]
        public string PropertyWithAttribute { get; set; }
    }
}
