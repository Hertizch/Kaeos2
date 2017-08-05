using Kaeos.Extensions;

namespace Kaeos.Models
{
    public class TimeFormat : ObservableObject
    {
        /*
         * Private fields
         */

        private int _id;
        private string _code;
        private string _friendlyView;

        /*
         * Properties
         */

        public int Id
        {
            get => _id; set { if (value == _id) return; _id = value; OnPropertyChanged(); }
        }

        public string Code
        {
            get => _code; set { if (value == _code) return; _code = value; OnPropertyChanged(); }
        }

        public string FriendlyView
        {
            get => _friendlyView; set { if (value == _friendlyView) return; _friendlyView = value; OnPropertyChanged(); }
        }
    }
}
