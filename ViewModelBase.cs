#region Using Directives
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
#endregion

namespace WpfConditionalValidation
{
    public class ViewModelBase : IDataErrorInfo, INotifyPropertyChanged
    {
        private readonly Dictionary<string, Func<ViewModelBase, object>> propertyGetters;
        private readonly Dictionary<string, ValidationAttribute[]> validators;

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase()
        {
            this.validators = this.GetType()
                .GetProperties()
                .Where(p => this.GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name, p => this.GetValidations(p));

            this.propertyGetters = this.GetType()
                .GetProperties()
                .Where(p => this.GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name, p => this.GetValueGetter(p));
        }

        public string Error
        {
            get
            {
                var errors = from validator in this.validators
                             from attribute in validator.Value
                             where !attribute.IsValid(this.propertyGetters[validator.Key](this))
                             select attribute.ErrorMessage;

                return string.Join(Environment.NewLine, errors.ToArray());
            }
        }

        public string this[string propertyName]
        {
            get
            {
                if (this.propertyGetters.ContainsKey(propertyName))
                {
                    var errorMessages = this.validators[propertyName]
                        .Where(attribute => !this.Validate(attribute, propertyName))
                        .Select(attribute => attribute.ErrorMessage).ToList();

                    return string.Join(Environment.NewLine, errorMessages);
                }

                return string.Empty;
            }
        }

        private ValidationAttribute[] GetValidations(PropertyInfo property)
        {
            return (ValidationAttribute[])property.GetCustomAttributes(typeof(ValidationAttribute), true);
        }

        private Func<ViewModelBase, object> GetValueGetter(PropertyInfo property)
        {
            return new Func<ViewModelBase, object>(viewmodel => property.GetValue(viewmodel, null));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }            
        }

        private bool Validate(ValidationAttribute validationAttribute, string propertyName)
        {
            var propertyValue = this.propertyGetters[propertyName](this);

            if (IsConditionalValidationAttribute(validationAttribute))
            {
                return validationAttribute.GetValidationResult(propertyValue, new ValidationContext(this)) == ValidationResult.Success;
            }

            return validationAttribute.IsValid(propertyValue);
        }

        private bool IsConditionalValidationAttribute(ValidationAttribute validationAttribute)
        {
            return validationAttribute.GetType().GetCustomAttributes().Any(x => x.GetType() == typeof(ConditionalAttribute));
        }
    }
}
