

namespace AttributeEnricher.Tests
{
    using Models;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Xunit;

    public class AttributeEnricherTests
    {
        IAttributeEnricher attributeEnricher = new AttributeEnricher();

        [Fact]
        public void Enrich_PropertyWithAttribute_ShouldModify()
        {
            var model = new ObjectWithProperties
            {
                PropertyWithAttribute = "original_value"
            };

            attributeEnricher.EnrichObjectForAttribute(
                model,
                (string value, ModifyAttribute attribute) => value + ",modified"
            );

            model.PropertyWithAttribute.Should().Be("original_value,modified");
        }

        [Fact]
        public void Enrich_PropertyWithoutAttribute_ShouldNotModify()
        {
            var model = new ObjectWithProperties
            {
                Property = "original_value"
            };

            attributeEnricher.EnrichObjectForAttribute(
                model,
                (string value, ModifyAttribute attribute) => value + ",modified"
            );

            model.Property.Should().Be("original_value");
        }

        [Fact]
        public void Enrich_NestedPropertyWithAttribute_ShouldModify()
        {
            var model = new NestedObject<ObjectWithProperties>
            {
                Nested = new ObjectWithProperties
                {
                    PropertyWithAttribute = "original_value"
                }
            };

            attributeEnricher.EnrichObjectForAttribute(
                model,
                (string value, ModifyAttribute attribute) => value + ",modified"
            );

            model.Nested.PropertyWithAttribute.Should().Be("original_value,modified");
        }

        [Fact]
        public void Enrich_ForNoPropertiesMatchingType_ShouldSuceed()
        {
            var model = new ObjectWithProperties();

            attributeEnricher.EnrichObjectForAttribute(
                model,
                (Dictionary<string, string> value, ModifyAttribute attribute) => value
            );

            // Should just not fail / throw exception
        }

        [Fact]
        public void Enrich_ForNoPropertiesWithAttribute_ShouldSuceed()
        {
            var model = new object();

            attributeEnricher.EnrichObjectForAttribute(
                model,
                (Dictionary<string, string> value, ModifyAttribute attribute) => value
            );

            // Should just not fail / throw exception
        }
        
        [Fact(Skip = "Stack overflow")]
        public void Enrich_ForMultipleNestedProperties_InArray_ShouldModify()
        {
            var model = new ArrayObject<ObjectWithProperties>
            {
                Array = new[] {
                    new ObjectWithProperties { PropertyWithAttribute = "original_value1" },
                    new ObjectWithProperties { PropertyWithAttribute = "original_value2" }
                }
            };

            attributeEnricher.EnrichObjectForAttribute(
                model,
                (string value, ModifyAttribute attribute) => value + ",modified"
            );

            model.Array[0].PropertyWithAttribute.Should().Be("original_value1,modified");
            model.Array[1].PropertyWithAttribute.Should().Be("original_value2,modified");
        }

        [Fact(Skip = "Stack overflow")]
        public void Enrich_ForMultipleNestedProperties_InList_ShouldModify()
        {
            var model = new ListObject<ObjectWithProperties>
            {
                List = new List<ObjectWithProperties> {
                    new ObjectWithProperties { PropertyWithAttribute = "original_value1" },
                    new ObjectWithProperties { PropertyWithAttribute = "original_value2" }
                }
            };

            attributeEnricher.EnrichObjectForAttribute(
                model,
                (string value, ModifyAttribute attribute) => value + ",modified"
            );

            model.List[0].PropertyWithAttribute.Should().Be("original_value1,modified");
            model.List[1].PropertyWithAttribute.Should().Be("original_value2,modified");
        }

        [Fact(Skip = "Stack overflow")]
        public void Enrich_ForMultipleNestedProperties_InEnumerable_ShouldModify()
        {
            var model = new EnumerableObject<ObjectWithProperties>(
                new[] {
                    new ObjectWithProperties { PropertyWithAttribute = "original_value1" },
                    new ObjectWithProperties { PropertyWithAttribute = "original_value2" }
                });

            attributeEnricher.EnrichObjectForAttribute(
                model,
                (string value, ModifyAttribute attribute) => value + ",modified"
            );

            model.Enumerable.ElementAt(0).PropertyWithAttribute.Should().Be("original_value1,modified");
            model.Enumerable.ElementAt(1).PropertyWithAttribute.Should().Be("original_value2,modified");
        }
        
        [Fact]
        public void ExpressionTest()
        {
            Func<string, ModifyAttribute, string> modifyFunction =
                (string value, ModifyAttribute attribute) =>
                value + ",modified";

            var type = typeof(ObjectWithProperties);
            var propertyInfo = type.GetProperty(nameof(ObjectWithProperties.PropertyWithAttribute));
            var getter = propertyInfo.GetGetMethod();
            var attribute = propertyInfo.GetCustomAttribute<ModifyAttribute>();

            var attributeParameter = Expression.Constant(attribute);
            var modelParameter = Expression.Parameter(type);
            var property = Expression.Property(modelParameter, propertyInfo);
            var modifyFuncParameter = Expression.Parameter(typeof(Func<string, ModifyAttribute, string>));

            var call = Expression.Invoke(modifyFuncParameter, property, attributeParameter);

            var assignement = Expression.Assign(property, call);
            var lambda = Expression.Lambda<Action<ObjectWithProperties, Func<string, ModifyAttribute, string>>>(assignement, modelParameter, modifyFuncParameter);
            var expressionFunc = lambda.Compile();


            var model = new ObjectWithProperties
            {
                PropertyWithAttribute = "original_value"
            };

            expressionFunc(model, modifyFunction);

            model.PropertyWithAttribute.Should().Be("original_value,modified");
        }
    }
}
