using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfConditionalValidation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    [Conditional]
    public class RangeIfAttribute : RangeAttribute
    {
        private string _dependentProperty;

        public RangeIfAttribute(double minimum, double maximum, string dependentProperty)
            : base(minimum, maximum)
        {
            this._dependentProperty = dependentProperty;
        }

        public RangeIfAttribute(int minimum, int maximum, string dependentProperty)
            : base(minimum, maximum)
        {
            this._dependentProperty = dependentProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var containerType = validationContext.ObjectInstance.GetType();
            var field = containerType.GetProperty(this._dependentProperty);
            bool dependentvalue = (bool)field.GetValue(validationContext.ObjectInstance);

            if (dependentvalue)
            {
                return base.IsValid(value, validationContext);
            }

            return ValidationResult.Success;
        }
    }
}
