using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AttributeEnricher
{
    public class AttributeEnricher : IAttributeEnricher
    {
        public void EnrichObjectForAttribute<TAttribute, TProperty>(object model, Func<TProperty, TAttribute, TProperty> modifyFunc)
            where TAttribute : Attribute
        {
            var properties = model
                .GetType()
                .GetProperties();

            ModifyPropertiesWithAttribute(model, modifyFunc, properties);
            RecurseDeeperIntoComplexTypes(model, modifyFunc, properties);
        }

        private void RecurseDeeperIntoComplexTypes<TAttribute, TProperty>(object model, Func<TProperty, TAttribute, TProperty> modifyFunc, PropertyInfo[] properties) where TAttribute : Attribute
        {
            var propertiesToRecurse = properties
                            .Where(property => property.PropertyType != typeof(TProperty));

            foreach (var property in propertiesToRecurse)
            {
                var value = property.GetValue(model);

                if (value == null)
                {
                    continue;
                }

                EnrichObjectForAttribute(value, modifyFunc);
            }
        }

        private static void ModifyPropertiesWithAttribute<TAttribute, TProperty>(object model, Func<TProperty, TAttribute, TProperty> modifyFunc, PropertyInfo[] properties) where TAttribute : Attribute
        {
            var propertiesToCheck = properties
                            .Where(property => property.PropertyType == typeof(TProperty))
                            .Select(property =>
                            {
                                var attribute = property.GetCustomAttribute<TAttribute>();
                                return (Property: property, Attribute: attribute);
                            })
                            .Where(tuple => tuple.Attribute != null);

            foreach (var (property, attribute) in propertiesToCheck)
            {
                var value = (TProperty)property.GetValue(model);
                var modifiedValue = modifyFunc(value, attribute);
                property.SetValue(model, modifiedValue);
            }
        }
    }
}
