using AttributeEnricher.Tests.Models;
using FluentAssertions;
using System;
using Xunit;

namespace AttributeEnricher.Tests
{
    public class AttributeEnricherTests
    {
        IAttributeEnricher attributeEnricher = new AttributeEnricher();

        [Fact]
        public void Enrich_ShouldModifyPropertyWithAttribute()
        {
            var model = new ObjectWithProperties
            {
                PropertyWithAttribute = "prop1"
            };

            attributeEnricher.EnrichObjectForAttribute<ModifyAttribute, string>(
                (value, attribute) => value + ",modified"
            );

            model.PropertyWithAttribute.Should().Be("prop1,modified");
        }
    }
}
