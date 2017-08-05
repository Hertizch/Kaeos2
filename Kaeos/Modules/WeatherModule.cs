using System;
using System.Collections.Specialized;
using System.Device.Location;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using Kaeos.Extensions;
using Kaeos.Models;
using Kaeos.ObservableImmutable;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kaeos.Modules
{
    public class WeatherModule : ObservableObject
    {
        public WeatherModule()
        {
            UnitFormatCollection = new ObservableImmutableList<UnitFormat>
            {
                new UnitFormat{Unit = "metric", Symbol = "°C" },
                new UnitFormat{ Unit = "imperial", Symbol = "°F" }
            };

            if (ControlModule.DesignMode)
                return;

            GetApiData();
            CreatePollingTimer();
        }

        /*
         * Private fields
         */

        private double _temp;
        private string _description;
        private string _locationName;
        private const string ApiKey = "d1394ff1f3c4f5617fc841cebe4e184a";
        private ObservableImmutableList<UnitFormat> _unitFormatCollection;
        private GeoCoordinateWatcher _geoCoordinateWatcher;
        private string _geoLocation;
        private Timer _timer;
        private RelayCommand _getGeoLocationCmd;

        /*
         * Properties
         */

        public double Temp
        {
            get => _temp; set { if (Math.Abs(value - _temp) < 0.01) return; _temp = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description; set { if (value == _description) return; _description = value; OnPropertyChanged(); }
        }

        public string LocationName
        {
            get => _locationName; set { if (value == _locationName) return; _locationName = value; OnPropertyChanged(); }
        }

        public ObservableImmutableList<UnitFormat> UnitFormatCollection
        {
            get => _unitFormatCollection; set { if (value == _unitFormatCollection) return; _unitFormatCollection = value; OnPropertyChanged(); }
        }

        public string GeoLocation
        {
            get => _geoLocation; set { if (value == _geoLocation) return; _geoLocation = value; OnPropertyChanged(); }
        }

        /*
         * Commands
         */

        public RelayCommand GetGeoLocationCmd => _getGeoLocationCmd ?? (_getGeoLocationCmd = new RelayCommand(Execute_GetGeoLocationCmd, p => true));

        /*
         * Methods
         */

        private async void GetApiData()
        {
            if (string.IsNullOrWhiteSpace(ControlModule.AppConfigModule.WeatherModule_Location))
                return;

            // Get the json
            var json = await GetJson($"http://api.openweathermap.org/data/2.5/weather?lat=59.95&lon=10.75&mode=json&units={ControlModule.AppConfigModule.WeatherModule_UnitFormat.Unit}&APPID={ApiKey}");

            // Deserialize the json
            if (json != null)
            {
                dynamic jsonResult = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());

                // Get the result values
                Temp = jsonResult.main.temp;
                Description = jsonResult.weather[0].description;
                LocationName = jsonResult.name;

                Debug.WriteLine($"[INFO] Weather Module: Weather data updated -- Temp: {Temp}, Description: {Description}, Location name: {LocationName}");
            }
            else
            {
                Debug.WriteLine("[ERROR] Weather Module: Weather data failed to update -- Json string is null");
            }
        }

        private static async Task<string> GetJson(string url)
        {
            Debug.WriteLine($"[INFO] Weather Module: Creating web client with request to url: {url}");

            // Create a web client.
            using (var client = new WebClient())
            {
                // Get the response string from the URL.
                string json = null;

                try
                {
                    json = await client.DownloadStringTaskAsync(url);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ERROR] Weather Module: Web client request failed -- Exception message: {ex.Message}");
                }

                // Return the result.
                return json;
            }
        }

        private async void Execute_GetGeoLocationCmd(object obj)
        {
            Debug.WriteLine(await GetLocationByIpAddress());
        }

        private async Task<string> GetExternalIpAddress()
        {
            string ipAddress;

            using (var webClient = new WebClient())
            {
                ipAddress = await webClient.DownloadStringTaskAsync("http://icanhazip.com");
            }

            return ipAddress;
        }

        private async Task<string> GetLocationByIpAddress()
        {
            string json = null;

            string ipAddress = await GetExternalIpAddress();

            Debug.WriteLine($"[INFO] Weather Module: Resolved external IP address to: {ipAddress}");

            using (var webClient = new WebClient())
            {
                json = await webClient.DownloadStringTaskAsync($"http://geoip.nekudo.com/api/{ipAddress}");
            }

            return json;
        }

        private void GetGeoLocation()
        {
            _geoCoordinateWatcher = new GeoCoordinateWatcher();

            _geoCoordinateWatcher.StatusChanged += (sender, args) =>
            {
                Debug.WriteLine($"[INFO] Weather Module: GeoPosition status changed: {args.Status}");
            };

            _geoCoordinateWatcher.PositionChanged += (sender, args) =>
            {
                if (args.Position.Location.IsUnknown)
                {
                    Debug.WriteLine($"[ERROR] Weather Module: GeoPosition returned unknown");
                }

                Debug.WriteLine($"[INFO] Weather Module: GeoPosition status changed: Lat {args.Position.Location.Latitude}, Lon {args.Position.Location.Latitude}");
            };

            _geoCoordinateWatcher.Start(true);
        }

        private void CreatePollingTimer()
        {
            _timer = new Timer(3600000);
            _timer.Elapsed += _timer_Elapsed; ;
            _timer.Start();

            Debug.WriteLine($"[INFO] Weather Module: Created polling timer instance with interval of {_timer.Interval} ms.");
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            GetApiData();
        }
    }
}
