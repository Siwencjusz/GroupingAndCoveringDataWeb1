using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Core.Common.Interfaces;

namespace Models.Base
{
    public class BaseNotifyError : BaseVm,INotifyDataErrorInfo
    {
        protected BaseNotifyError()
        {

        }
        protected BaseNotifyError(IValidateService validateService)
        {
            _validateService = validateService;
        }
        private readonly IValidateService _validateService;
        protected readonly Dictionary<string, ICollection<string>> ValidationErrors = new Dictionary<string, ICollection<string>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
                || !ValidationErrors.ContainsKey(propertyName))
                return null;

            return ValidationErrors[propertyName];
        }

        public bool HasErrors => ValidationErrors.Count > 0;

        protected async void ValueRangeValidation<T>(T value, int min, int max, string propertyKey)
        {
            ICollection<string> validationErrors = null;
            /* Call service asynchronously */
            bool isValid = await Task.Run(() => _validateService.ValueRangeValidation(value, min, max, out validationErrors))
                .ConfigureAwait(false);

            if (!isValid)
            {
                /* Update the collection in the dictionary returned by the GetErrors method */
                ValidationErrors[propertyKey] = validationErrors;
                /* Raise event to tell WPF to execute the GetErrors method */
                RaiseErrorsChanged(propertyKey);
            }
            else if (ValidationErrors.ContainsKey(propertyKey))
            {
                /* Remove all errors for this property */
                ValidationErrors.Remove(propertyKey);
                /* Raise event to tell WPF to execute the GetErrors method */
                RaiseErrorsChanged(propertyKey);
            }
        }
    }
}
