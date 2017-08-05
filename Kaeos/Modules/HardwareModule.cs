using Kaeos.Extensions;
using Kaeos.Helpers;
using Kaeos.Models;
using OpenHardwareMonitor.Hardware;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;

namespace Kaeos.Modules
{
    public class HardwareModule : ObservableObject
    {
        public HardwareModule()
        {
            if (ControlModule.DesignMode)
            {
                MonitoredHardware = new MonitoredHardware();
                AddDummyContents();
                return;
            }

            MonitoredHardware = new MonitoredHardware();

            Execute_StartMonitoring();
        }

        /*
         * Private fields
         */

        private MonitoredHardware _monitoredHardware;
        private Computer _computer;
        private Timer _timer;

        /*
         * Properties
         */

        public MonitoredHardware MonitoredHardware
        {
            get => _monitoredHardware; set { if (value == _monitoredHardware) return; _monitoredHardware = value; OnPropertyChanged(); }
        }

        /*
         * Methods
         */

        private void Execute_StartMonitoring()
        {
            // Create computer instance
            _computer = new Computer
            {
                GPUEnabled = true,
                CPUEnabled = true,
                RAMEnabled = true,
                MainboardEnabled = true
            };

            Debug.WriteLine("[INFO] Hardware Module: Created computer instance with parameters: " +
                            $"Motherboard Enabled: {_computer.MainboardEnabled} " +
                            $"GPU Enabled: {_computer.GPUEnabled}, " +
                            $"CPU Enabled: {_computer.CPUEnabled}, " +
                            $"RAM Enabled: {_computer.RAMEnabled}");

            // Start computer instance
            _computer.Open();

            // Iterate all hardware
            foreach (var hardware in _computer.Hardware.Where(hardware => hardware != null))
            {
                // Get hardware values
                hardware.Update();

                // Get motherboard name
                if (hardware.HardwareType.Equals(HardwareType.Mainboard))
                {
                    var name = hardware.Name;
                    MonitoredHardware.MotherboardName = name;

                    Debug.WriteLine($"[INFO] Hardware Module: Added hardware: {hardware.HardwareType}, name: {hardware.Name}, id: {hardware.Identifier}");
                }

                // Get GPU
                if (hardware.HardwareType.Equals(HardwareType.GpuNvidia))
                {
                    var name = hardware.Name;
                    var load = GetSensorValue(hardware, SensorType.Load, "GPU Core");
                    var temperature = GetSensorValue(hardware, SensorType.Temperature, "GPU Core");
                    var fanSpeed = GetSensorValue(hardware, SensorType.Fan);

                    // Remove vendor name to shorten it
                    name = name.Replace("nvidia", "", StringComparison.OrdinalIgnoreCase).Trim();

                    MonitoredHardware.GpuNvidias.Add(new GpuNvidia
                    {
                        Name = name,
                        Temperature = temperature,
                        Load = load,
                        FanSpeed = fanSpeed,
                        Hardware = hardware
                    });

                    Debug.WriteLine($"[INFO] Hardware Module: Added hardware: {hardware.HardwareType}, name: {hardware.Name}, id: {hardware.Identifier}");
                }

                // Get CPU
                if (hardware.HardwareType.Equals(HardwareType.CPU))
                {
                    var name = hardware.Name;
                    var load = GetSensorValue(hardware, SensorType.Load, "CPU Total");
                    var temperature = GetSensorValue(hardware, SensorType.Temperature, "CPU Package");
                    var fanSpeed = GetSensorValue(hardware, SensorType.Fan);

                    // Remove vendor name to shorten it
                    name = name.Replace("intel", "", StringComparison.OrdinalIgnoreCase).Trim();

                    // Add to collection
                    MonitoredHardware.Cpus.Add(new Cpu
                    {
                        Name = name,
                        Temperature = temperature,
                        Load = load,
                        FanSpeed = fanSpeed,
                        Hardware = hardware
                    });

                    Debug.WriteLine($"[INFO] Hardware Module: Added hardware: {hardware.HardwareType}, name: {hardware.Name}, id: {hardware.Identifier}");
                }

                // Get RAM
                if (hardware.HardwareType.Equals(HardwareType.RAM))
                {
                    var name = hardware.Name;
                    var load = GetSensorValue(hardware, SensorType.Load, "Memory");

                    // If vendor name could not be resolved, try to get from wmi
                    if (name.Equals("Generic Memory"))
                    {
                        var partNumber = WmiHelper.GetValueFromWmi("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory", "PartNumber");
                        if (partNumber != null)
                        {
                            name = GetMemoryName(partNumber);

                            Debug.WriteLine($"[INFO] Hardware Module: WMI resolved memory name: {name}, from partnumber: {partNumber}");
                        }
                    }

                    // Add to collection
                    MonitoredHardware.Rams.Add(new Ram
                    {
                        Name = name,
                        Load = load,
                        Hardware = hardware
                    });

                    Debug.WriteLine($"[INFO] Hardware Module: Added hardware: {hardware.HardwareType}, name: {hardware.Name}, id: {hardware.Identifier}");
                }
            }

            // Create polling timer
            CreatePollingTimer();
        }

        private void CreatePollingTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();

            Debug.WriteLine($"[INFO] Hardware Module: Created polling timer instance with interval of {_timer.Interval} ms.", true);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            // Update values -- Gpu Nvidia
            if (MonitoredHardware.GpuNvidias != null)
                foreach (var gpu in MonitoredHardware.GpuNvidias.Where(gpu => gpu.Hardware != null))
                {
                    gpu.Hardware.Update();

                    var load = GetSensorValue(gpu.Hardware, SensorType.Load, "GPU Core");
                    var temperature = GetSensorValue(gpu.Hardware, SensorType.Temperature, "GPU Core");
                    //var fanSpeed = Settings.Default.ShowGpuFan ? GetSensorValue(gpu.Hardware, SensorType.Fan) : 0;

                    gpu.Load = load;
                    gpu.Temperature = temperature;
                    //gpu.FanSpeed = fanSpeed;
                }

            // Update values -- Cpu
            if (MonitoredHardware.Cpus != null)
                foreach (var cpu in MonitoredHardware.Cpus.Where(cpu => cpu.Hardware != null))
                {
                    cpu.Hardware.Update();

                    var load = GetSensorValue(cpu.Hardware, SensorType.Load, "CPU Total");
                    var temperature = GetSensorValue(cpu.Hardware, SensorType.Temperature, "CPU Package");
                    //var fanSpeed = Settings.Default.ShowCpuFan ? GetSensorValue(cpu.Hardware, SensorType.Fan) : 0;

                    cpu.Load = load;
                    cpu.Temperature = temperature;
                    //cpu.FanSpeed = fanSpeed;
                }

            // Update values -- Ram
            if (MonitoredHardware.Rams != null)
                foreach (var ram in MonitoredHardware.Rams.Where(ram => ram.Hardware != null))
                {
                    ram.Hardware.Update();
                    var load = GetSensorValue(ram.Hardware, SensorType.Load, "Memory");
                    ram.Load = load;
                }
        }

        private static float? GetSensorValue(IHardware hardware, SensorType sensorType, string sensorName = null)
        {
            float? results = null;

            // Check if sensor exists
            var hasValue = sensorName == null
                ? hardware.Sensors?.Any(x => x.SensorType.Equals(sensorType))
                : hardware.Sensors?.Any(x => x.SensorType.Equals(sensorType) && x.Name.Equals(sensorName));
            var b = !hasValue;
            if (b != null && (bool)b)
                return 0;

            try
            {
                results = sensorName == null
                    ? hardware.Sensors?.First(x => x.SensorType.Equals(sensorType)).Value
                    : hardware.Sensors?.First(x => x.SensorType.Equals(sensorType) && x.Name.Equals(sensorName)).Value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Hardware Module: Unable to get sensor value from hardware name: {hardware.Name}, of type: {sensorType}, sensor name: {sensorName} -- Exception: {ex.Message}");
            }

            return results ?? 0;
        }

        private void AddDummyContents()
        {
            MonitoredHardware.MotherboardName = "Some motherboard Name";

            MonitoredHardware.GpuNvidias.Add(new GpuNvidia
            {
                Name = "Some GPU Name",
                Temperature = 0,
                Load = 0,
                FanSpeed = 0
            });

            MonitoredHardware.Cpus.Add(new Cpu
            {
                Name = "Some CPU Name",
                Temperature = 0,
                Load = 0,
                FanSpeed = 0
            });

            MonitoredHardware.Rams.Add(new Ram
            {
                Name = "Some RAM Name",
                Load = 0
            });
        }

        private static string GetMemoryName(string partNumber)
        {
            string vendor = null;
            var productSeries = "Memory";
            var memoryAmount = 0;

            // Corsair
            if (partNumber.StartsWith("CM"))
            {
                vendor = "Corsair";
                var match = Regex.Match(partNumber, "CM([C-Z]*)(\\d*)G");
                if (match.Success)
                {
                    var productSeriesCode = match.Groups[1].Value;
                    switch (productSeriesCode)
                    {
                        // XMS series
                        case "C":
                            productSeries = "XMS classic";
                            break;
                        case "X":
                            productSeries = "XMS classic";
                            break;

                        // Vengenance series
                        case "Z":
                            productSeries = "Vengeance";
                            break;
                        case "L":
                            productSeries = "Vengeance LP";
                            break;
                        case "R":
                            productSeries = "Vengeance RGB";
                            break;
                        case "Y":
                            productSeries = "Vengeance Pro";
                            break;
                        case "K":
                            productSeries = "Vengeance LPX";
                            break;

                        // Dominator
                        case "D":
                            productSeries = "Dominator";
                            break;

                        // D
                        case "P":
                            productSeries = "D";
                            break;

                        // GT series
                        case "G":
                            productSeries = "GT Airflow II";
                            break;
                        case "GS":
                            productSeries = "GT";
                            break;

                        // G
                        case "T":
                            productSeries = "G";
                            break;

                        // ECC
                        case "E":
                            productSeries = "Non-Registered ECC";
                            break;
                        case "S":
                            productSeries = "Registered ECC";
                            break;
                        
                        // SO-DIMM
                        case "SO":
                            productSeries = "SoDIMM";
                            break;
                        case "SX":
                            productSeries = "SoDIMM Vengeance";
                            break;

                        // Value
                        case "V":
                            productSeries = "Value Select";
                            break;

                        default:
                            productSeries = "Memory";
                            break;
                    }

                    // Get memory amount
                    memoryAmount = int.Parse(match.Groups[2].Value);
                }
            }

            // G.skill
            else if (partNumber.StartsWith("F1") | partNumber.StartsWith("F2") | partNumber.StartsWith("F3") | partNumber.StartsWith("F4") | partNumber.StartsWith("FA"))
            {
                vendor = "G.Skill";
                var match = Regex.Match(partNumber, "F.-\\d*CL\\d.*-(\\d)*GB(.*)");
                if (match.Success)
                {
                    var productSeriesCode = match.Groups[2].Value;
                    switch (productSeriesCode)
                    {
                        // Ripjaws V
                        case "VR":
                            productSeries = "Ripjaws V";
                            break;
                        case "VB":
                            productSeries = "Ripjaws V";
                            break;
                        case "VK":
                            productSeries = "Ripjaws V";
                            break;
                        case "VS":
                            productSeries = "Ripjaws V";
                            break;
                        case "VG":
                            productSeries = "Ripjaws V";
                            break;

                        // Ripjaws 4
                        case "RR":
                            productSeries = "Ripjaws 4";
                            break;
                        case "RB":
                            productSeries = "Ripjaws 4";
                            break;
                        case "RK":
                            productSeries = "Ripjaws 4";
                            break;

                        // RipjawsX
                        case "XL":
                            productSeries = "RipjawsX";
                            break;
                        case "XM":
                            productSeries = "RipjawsX";
                            break;
                        case "XH":
                            productSeries = "RipjawsX";
                            break;

                        // RipjawsZ
                        case "ZL":
                            productSeries = "RipjawsZ";
                            break;
                        case "ZM":
                            productSeries = "RipjawsZ";
                            break;
                        case "ZH":
                            productSeries = "RipjawsZ";
                            break;

                        // Ripjaws
                        case "RL":
                            productSeries = "Ripjaws";
                            break;
                        case "RM":
                            productSeries = "Ripjaws";
                            break;
                        case "RH":
                            productSeries = "Ripjaws";
                            break;

                        // Trident series
                        case "TZ":
                            productSeries = "Trident Z";
                            break;
                        case "TZSW":
                            productSeries = "Trident Z";
                            break;
                        case "TZSK":
                            productSeries = "Trident Z";
                            break;
                        case "TZKW":
                            productSeries = "Trident Z";
                            break;
                        case "TZKO":
                            productSeries = "Trident Z";
                            break;
                        case "TZKY":
                            productSeries = "Trident Z";
                            break;
                        case "TX":
                            productSeries = "TridentX";
                            break;
                        case "TD":
                            productSeries = "Trident";
                            break;

                        // ARES
                        case "AB":
                            productSeries = "ARES";
                            break;
                        case "AO":
                            productSeries = "ARES";
                            break;
                        case "AR":
                            productSeries = "ARES";
                            break;

                        // ECO
                        case "ECO":
                            productSeries = "ECO";
                            break;

                        // Sniper
                        case "SR":
                            productSeries = "Sniper";
                            break;

                        // Aegis
                        case "IS":
                            productSeries = "Aegis";
                            break;
                        case "ISL":
                            productSeries = "Aegis";
                            break;

                        // Performance
                        case "NQ":
                            productSeries = "Performance";
                            break;
                        case "PK":
                            productSeries = "Performance";
                            break;
                        case "HK":
                            productSeries = "Performance";
                            break;

                        // Value RAM
                        case "NT":
                            productSeries = "Value RAM";
                            break;
                        case "NS":
                            productSeries = "Value RAM";
                            break;

                        // SO-DIMM
                        case "SQ":
                            productSeries = "SO-DIMM";
                            break;
                        case "SK":
                            productSeries = "SO-DIMM";
                            break;

                        default:
                            productSeries = "Memory";
                            break;
                    }

                    // Get memory amount
                    memoryAmount = int.Parse(match.Groups[1].Value);
                }
            }

            return $"{vendor} {productSeries}";
        }
    }
}
