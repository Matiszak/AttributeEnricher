using System;

namespace AttributeEnricher
{
    public interface IAttributeEnricher
    {
        void EnrichObjectForAttribute<TAttribute, TProperty>(object model, Func<TProperty, TAttribute, TProperty> modifyFunc)
            where TAttribute : Attribute;
    }
}
