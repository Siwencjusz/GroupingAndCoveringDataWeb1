using System.Collections.Generic;

namespace Core.Common.Interfaces
{
    public interface IValidateService
    {
        bool ValueRangeValidation<T>(T value, double min, double max, out ICollection<string> validationErrors);  
    }
}
