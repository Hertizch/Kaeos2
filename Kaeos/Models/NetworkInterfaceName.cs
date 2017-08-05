using System.Net.NetworkInformation;
using Kaeos.Extensions;

namespace Kaeos.Models
{
    public class NetworkInterfaceName : ObservableObject
    {
        /*
         * Private fields
         */

        private string _description;
        private string _internalIp;
        private NetworkInterface _networkInterface;

        /*
         * Properties
         */

        public string Description
        {
            get => _description; set { if (value == _description) return; _description = value; OnPropertyChanged(); }
        }

        public string InternalIp
        {
            get => _internalIp; set { if (value == _internalIp) return; _internalIp = value; OnPropertyChanged(); }
        }

        public NetworkInterface NetworkInterface
        {
            get => _networkInterface; set { if (value == _networkInterface) return; _networkInterface = value; OnPropertyChanged(); }
        }
    }
}
