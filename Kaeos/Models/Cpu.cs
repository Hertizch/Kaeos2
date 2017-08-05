using Kaeos.Extensions;
using OpenHardwareMonitor.Hardware;

namespace Kaeos.Models
{
    public class Cpu : ObservableObject
    {
        /*
         * Private fiels
         */

        private string _name;
        private float? _temperature;
        private float? _load;
        private float? _fanSpeed;

        /*
         * Properties
         */

        public IHardware Hardware { get; set; }

        public string Name
        {
            get => _name; set { if (value == _name) return; _name = value; OnPropertyChanged(); }
        }

        public float? Temperature
        {
            get => _temperature; set { if (value == _temperature) return; _temperature = value; OnPropertyChanged(); }
        }

        public float? Load
        {
            get => _load; set { if (value == _load) return; _load = value; OnPropertyChanged(); }
        }

        public float? FanSpeed
        {
            get => _fanSpeed; set { if (value == _fanSpeed) return; _fanSpeed = value; OnPropertyChanged(); }
        }
    }
}
