using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AttributeEnricher
{
    public class AttributeEnricher : IAttributeEnricher
    {
        public void EnrichObjectForAttribute<T, TAttribute, TProperty>( Func<TProperty, TAttribute, TProperty> modifyFunc)
            where TAttribute : Attribute
        {
            var properties = typeof(TProperty)
                .GetProperties()
                .Select(property =>
                {
                    var attribute = property.GetCustomAttribute<TAttribute>();
                    return (Property: property, Attribute: attribute);
                })
                .Where(tuple => tuple.Attribute != null)
                .ToArray();

            foreach(var tuple in properties)
            {
                var value = tuple.Property.GetValue()
            }
        }
    }
}
