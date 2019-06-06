using System.Collections.Generic;
using Core.Common.Items;

namespace Core.Common.Interfaces
{
    public interface IAttributeColumnConverter
    {
        IEnumerable<AttributeDescription> ConvertColumns2Attributes(string[] dataDescription);
    }
}