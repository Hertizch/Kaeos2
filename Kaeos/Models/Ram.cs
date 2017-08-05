using Kaeos.Extensions;
using OpenHardwareMonitor.Hardware;

namespace Kaeos.Models
{
    public class Ram : ObservableObject
    {
        private string _name;
        private float? _load;

        public IHardware Hardware { get; set; }

        public string Name
        {
            get => _name; set { if (value == _name) return; _name = value; OnPropertyChanged(); }
        }

        public float? Load
        {
            get => _load; set { if (value == _load) return; _load = value; OnPropertyChanged(); }
        }
    }
}
