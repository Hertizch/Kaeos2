using Kaeos.Extensions;
using System.ComponentModel;
using System.Windows;

namespace Kaeos.Modules
{
    public class ControlModule : ObservableObject
    {
        public ControlModule()
        {
            AppConfigModule = new AppConfigModule();
            DateTimeModule = new DateTimeModule();
            HardwareModule = new HardwareModule();
            WeatherModule = new WeatherModule();
            NetworkModule = new NetworkModule();
            VolumeMixerModule = new VolumeMixerModule();
        }

        /*
         * Properties
         */

        public static bool DesignMode { get; set; } = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public static AppConfigModule AppConfigModule { get; private set; }

        public static DateTimeModule DateTimeModule { get; private set; }

        public static HardwareModule HardwareModule { get; private set; }

        public static WeatherModule WeatherModule { get; private set; }

        public static NetworkModule NetworkModule { get; private set; }

        public static VolumeMixerModule VolumeMixerModule { get; private set; }
    }
}
