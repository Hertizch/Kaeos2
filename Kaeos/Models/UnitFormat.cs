using Kaeos.Extensions;

namespace Kaeos.Models
{
    public class UnitFormat : ObservableObject
    {
        /*
         * Private fields
         */

        private string _unit;
        private string _symbol;

        /*
         * Properties
         */

        public string Unit
        {
            get => _unit; set { if (value == _unit) return; _unit = value; OnPropertyChanged(); }
        }

        public string Symbol
        {
            get => _symbol; set { if (value == _symbol) return; _symbol = value; OnPropertyChanged(); }
        }
    }
}
