using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Interfaces;

namespace BusinessLogicLayer.DataValidation
{
    public class ValidateService  :IValidateService
    {
        public bool ValueRangeValidation<T>(T value, double min, double max, out ICollection<string> validationErrors) 
        {            
            validationErrors = new Collection<string>();

            if (!(value != null && Convert.ToDouble(value) >= min && Convert.ToDouble(value) <= max))
            {                
                validationErrors.Add($"Podaj liczbę większą od {min} do {max}.");
            }

            return !validationErrors.Any();
        }

        
    }
}
