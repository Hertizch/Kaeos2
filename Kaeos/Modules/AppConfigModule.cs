using System;
using System.Diagnostics.CodeAnalysis;
using Kaeos.Extensions;
using Kaeos.Models;

namespace Kaeos.Modules
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class AppConfigModule : ObservableObject
    {
        public AppConfigModule()
        {
            LoadFromSettings();

            // Set default values
            if (DateTimeModule_TimeFormat == null)
                DateTimeModule_TimeFormat = new TimeFormat {Id = 1, Code = "T", FriendlyView = DateTime.Now.ToString("T")};

            if (DateTimeModule_DateFormat == null)
                DateTimeModule_DateFormat = new DateFormat {Id = 3, Code = "D", FriendlyView = DateTime.Now.ToString("D")};

            if (WeatherModule_UnitFormat == null)
                WeatherModule_UnitFormat = new UnitFormat {Unit = "metric"};
        }

        /*
         * Private fields
         */

        private TimeFormat _dateTimeModule_TimeFormat;
        private DateFormat _dateTimeModule_DateFormat;
        private string _weatherModule_Location;
        private UnitFormat _weatherModule_UnitFormat;

        /*
         * Properties
         */

        public TimeFormat DateTimeModule_TimeFormat
        {
            get => _dateTimeModule_TimeFormat; set { if (value == _dateTimeModule_TimeFormat) return; _dateTimeModule_TimeFormat = value; OnPropertyChanged(); }
        }

        public DateFormat DateTimeModule_DateFormat
        {
            get => _dateTimeModule_DateFormat; set { if (value == _dateTimeModule_DateFormat) return; _dateTimeModule_DateFormat = value; OnPropertyChanged(); }
        }

        public string WeatherModule_Location
        {
            get => _weatherModule_Location; set { if (value == _weatherModule_Location) return; _weatherModule_Location = value; OnPropertyChanged(); }
        }

        public UnitFormat WeatherModule_UnitFormat
        {
            get => _weatherModule_UnitFormat; set { if (value == _weatherModule_UnitFormat) return; _weatherModule_UnitFormat = value; OnPropertyChanged(); }
        }

        /*
         * Methods
         */

        private void LoadFromSettings()
        {
            var settings = Properties.Settings.Default;
            DateTimeModule_TimeFormat = settings.DateTimeModule_TimeFormat;
            DateTimeModule_DateFormat = settings.DateTimeModule_DateFormat;
            WeatherModule_Location = settings.WeatherModule_Location;
            WeatherModule_UnitFormat = settings.WeatherModule_UnitFormat;

        }

        public void SaveToSettings()
        {
            var settings = Properties.Settings.Default;
            settings.DateTimeModule_TimeFormat = DateTimeModule_TimeFormat;
            settings.DateTimeModule_DateFormat = DateTimeModule_DateFormat;
            settings.WeatherModule_Location = WeatherModule_Location;
            settings.WeatherModule_UnitFormat = WeatherModule_UnitFormat;

            settings.Save();
        }
    }
}
