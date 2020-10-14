using System;

namespace AttributeEnricher.Tests
{
    class ExpressionAttributeEnricher : IAttributeEnricher
    {
        public void EnrichObjectForAttribute<TAttribute, TProperty>(object model, Func<TProperty, TAttribute, TProperty> modifyFunc) where TAttribute : Attribute
        {
            throw new NotImplementedException();
        }
    }
}
