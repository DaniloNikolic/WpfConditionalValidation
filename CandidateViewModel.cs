using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WpfConditionalValidation
{
    public class CandidateViewModel : ViewModelBase
    {
        private string _name;
        private int _yearsOfExperience;
        private bool _isExperienced;

        [Required(ErrorMessage = "Name is required")]
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                this._name = value;
                this.OnPropertyChanged("Name");
            }
        }

        [RangeIf(1, 100, "IsExperienced", ErrorMessage = "Years Of Experience must be greater than 0")]
        public int YearsOfExperience
        {
            get
            {
                return this._yearsOfExperience;
            }

            set
            {
                this._yearsOfExperience = value;
            }
        }

        public bool IsExperienced
        {
            get
            {
                return this._isExperienced;
            }

            set
            {
                this._isExperienced = value;
                this.OnPropertyChanged("YearsOfExperience");
            }
        }
    }
}
