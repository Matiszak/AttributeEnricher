using System.Collections.Generic;

namespace AttributeEnricher.Tests.Models
{
    class EnumerableObject<T>
    {
        public IEnumerable<T> Enumerable { get; set; }

        public EnumerableObject(IEnumerable<T> enumerable)
        {
            Enumerable = CreateEnumerable(enumerable);
        }

        private IEnumerable<T> CreateEnumerable(IEnumerable<T> enumerable)
        {
            foreach(var item in enumerable)
            {
                yield return item;
            }
        }
    }
}
