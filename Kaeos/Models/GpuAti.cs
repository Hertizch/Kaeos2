using Kaeos.Extensions;
using OpenHardwareMonitor.Hardware;

namespace Kaeos.Models
{
    public class GpuAti : ObservableObject
    {
        public ISensor GpuAtiSensor { get; set; }
    }
}
