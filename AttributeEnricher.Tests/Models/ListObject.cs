using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeEnricher.Tests.Models
{
    class ListObject<T>
    {
        public IList<T> List { get; set; }
    }
}
