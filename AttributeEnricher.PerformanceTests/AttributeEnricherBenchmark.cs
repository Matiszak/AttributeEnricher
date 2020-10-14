using AttributeEnricher.PerformanceTests.Models;
using AttributeEnricher.Tests.Models;
using BenchmarkDotNet.Attributes;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AttributeEnricher.PerformanceTests
{
    [MemoryDiagnoser]
    public class AttributeEnricherBenchmark
    {
        static IAttributeEnricher enricher;
        static Func<string, ModifyAttribute, string> modifyFunction;
        static Action<ObjectWithProperties, Func<string, ModifyAttribute, string>> expressionFunc;

        static AttributeEnricherBenchmark()
        {
            enricher = new AttributeEnricher();
            modifyFunction = (value, attribute) => value + ",modified";

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
            expressionFunc = lambda.Compile();
        }

        [Benchmark(Baseline = true)]
        public void NormalGetAndSet()
        {
            var model = new ObjectWithProperties()
            {
                PropertyWithAttribute = "original_value"
            };
            model.PropertyWithAttribute = model.PropertyWithAttribute + ",modified";
        }
        
        [Benchmark]
        public void Enricher()
        {
            var model = new ObjectWithProperties()
            {
                PropertyWithAttribute = "original_value"
            };
            enricher.EnrichObjectForAttribute(model, modifyFunction);
        }

        [Benchmark]
        public void CompiledExpression()
        {
            var model = new ObjectWithProperties()
            {
                PropertyWithAttribute = "original_value"
            };
            expressionFunc(model, modifyFunction);
        }

        [Benchmark]
        public void Nested_NormalGetAndSet()
        {
            var model = new NestedObject<ObjectWithProperties>
            {
                Nested = new ObjectWithProperties()
                {
                    PropertyWithAttribute = "original_value"
                }
            };
            model.Nested.PropertyWithAttribute = model.Nested.PropertyWithAttribute + ",modified";
        }

        [Benchmark]
        public void Nested_Enricher()
        {
            var model = new NestedObject<ObjectWithProperties>
            {
                Nested = new ObjectWithProperties()
                {
                    PropertyWithAttribute = "original_value"
                }
            };
            enricher.EnrichObjectForAttribute(model, modifyFunction);
        }
    }
}
