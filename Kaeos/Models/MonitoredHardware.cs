using Kaeos.Extensions;
using Kaeos.ObservableImmutable;

namespace Kaeos.Models
{
    public class MonitoredHardware : ObservableObject
    {
        public MonitoredHardware()
        {
            GpuNvidias = new ObservableImmutableList<GpuNvidia>();
            GpuAtis = new ObservableImmutableList<GpuAti>();
            Cpus = new ObservableImmutableList<Cpu>();
            Rams = new ObservableImmutableList<Ram>();
        }

        /*
         * Private fields
         */

        private string _motherboardName;
        private ObservableImmutableList<GpuNvidia> _gpuNvidias;
        private ObservableImmutableList<GpuAti> _gpuAtis;
        private ObservableImmutableList<Cpu> _cpus;
        private ObservableImmutableList<Ram> _rams;

        /*
         * Properties
         */

        public string MotherboardName
        {
            get => _motherboardName; set { if (value == _motherboardName) return; _motherboardName = value; OnPropertyChanged(); }
        }

        public ObservableImmutableList<GpuNvidia> GpuNvidias
        {
            get => _gpuNvidias; set { if (value == _gpuNvidias) return; _gpuNvidias = value; OnPropertyChanged(); }
        }

        public ObservableImmutableList<GpuAti> GpuAtis
        {
            get => _gpuAtis; set { if (value == _gpuAtis) return; _gpuAtis = value; OnPropertyChanged(); }
        }

        public ObservableImmutableList<Cpu> Cpus
        {
            get => _cpus; set { if (value == _cpus) return; _cpus = value; OnPropertyChanged(); }
        }

        public ObservableImmutableList<Ram> Rams
        {
            get => _rams; set { if (value == _rams) return; _rams = value; OnPropertyChanged(); }
        }
    }
}
