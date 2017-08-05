using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Timers;
using Kaeos.Extensions;
using Kaeos.Helpers;
using Kaeos.Models;
using Kaeos.ObservableImmutable;

namespace Kaeos.Modules
{
    public class NetworkModule : ObservableObject
    {
        public NetworkModule()
        {
            NetworkInterfaceNames = new ObservableImmutableList<NetworkInterfaceName>();
            CurrentNetworkInterfaceName = new NetworkInterfaceName();

            SetNetworkInterfaceName();
        }

        /*
         * Private fields
         */

        private NetworkInterfaceName _currentNetworkInterfaceName;
        private ObservableImmutableList<NetworkInterfaceName> _networkInterfaceNames;
        private float _networkBytesSent;
        private float _networkBytesRecieved;
        private Timer _timer;

        /*
         * Properties
         */

        public NetworkInterfaceName CurrentNetworkInterfaceName
        {
            get => _currentNetworkInterfaceName; set { if (value == _currentNetworkInterfaceName) return; _currentNetworkInterfaceName = value; OnPropertyChanged(); }
        }

        public ObservableImmutableList<NetworkInterfaceName> NetworkInterfaceNames
        {
            get => _networkInterfaceNames; set { if (value == _networkInterfaceNames) return; _networkInterfaceNames = value; OnPropertyChanged(); }
        }

        public float NetworkBytesSent
        {
            get => _networkBytesSent; set { if (Math.Abs(value - _networkBytesSent) < 0.01) return; _networkBytesSent = value; OnPropertyChanged(); }
        }

        public float NetworkBytesRecieved
        {
            get => _networkBytesRecieved; set { if (Math.Abs(value - _networkBytesRecieved) < 0.01) return; _networkBytesRecieved = value; OnPropertyChanged(); }
        }

        /*
         * Methods
         */

        private void SetNetworkInterfaceName()
        {
            // Get all network interfaces and filter out unwanted
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces().Where(
                        x =>
                            x.OperationalStatus == OperationalStatus.Up &&
                            x.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                            x.NetworkInterfaceType != NetworkInterfaceType.Tunnel);

            // Loop the results
            foreach (var networkInterface in networkInterfaces)
            {
                var nic = networkInterface.Description;

                // PerformanceCounter class uses different characters than the GetAllNetworkInterfaces class, so replace them
                nic = nic.Replace("\\", "_");
                nic = nic.Replace("/", "_");
                nic = nic.Replace("(", "[");
                nic = nic.Replace(")", "]");
                nic = nic.Replace("#", "_");

                // Add it to the collection
                NetworkInterfaceNames.Add(new NetworkInterfaceName
                {
                    Description = nic,
                    NetworkInterface = networkInterface
                });

                Debug.WriteLine($"[INFO] Network Module: Added network interface: '{nic}', of type: '{networkInterface.NetworkInterfaceType}', id: '{networkInterface.Id}'");

                if (NetworkInterfaceNames.Count > 0)
                {
                    foreach (var networkInterfaceName in NetworkInterfaceNames.Where(networkInterfaceName => networkInterfaceName.Description.Equals(CurrentNetworkInterfaceName.Description)))
                    {
                        var dnsHostEntry = Dns.GetHostEntry(Dns.GetHostName());
                        networkInterfaceName.InternalIp = dnsHostEntry.AddressList.First(x => x.AddressFamily.Equals(AddressFamily.InterNetwork)).ToString();
                    }
                }
            }

            // Set the first nic as current adapter, if any
            if (NetworkInterfaceNames.Count > 0)
            {
                CurrentNetworkInterfaceName.Description = NetworkInterfaceNames.First().Description;
                Debug.WriteLine($"[INFO] Network Module: Current network interface set to: '{CurrentNetworkInterfaceName.Description}'");
            }

            // Create polling timer
            CreatePollingTimer();
        }

        private void CreatePollingTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }
        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentNetworkInterfaceName.Description))
                return;

            SystemCounters.GetNetworkBytesSentValue(CurrentNetworkInterfaceName.Description);
            SystemCounters.GetNetworkBytesRecievedValue(CurrentNetworkInterfaceName.Description);
        }
    }
}
