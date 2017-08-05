using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using CSCore.CoreAudioAPI;
using Kaeos.Extensions;
using Kaeos.Helpers;
using Kaeos.Models;
using Kaeos.ObservableImmutable;

namespace Kaeos.Modules
{
    public class VolumeMixerModule : ObservableObject
    {
        public VolumeMixerModule()
        {
            ActiveAppsCollection = new ObservableImmutableList<ActiveApp>();

            /*if (ControlModule.DesignMode)
                return;*/

            // Get master device name
            TimerHelper.BeginIntervalTimer(1000, GetMasterDeviceNameCmd);

            // Get master volume
            TimerHelper.BeginIntervalTimer(1000, SetMasterAudioVolumeCmd);

            // Get master peak
            TimerHelper.BeginIntervalTimer(100, GetMasterAudioPeakCmd);

            if (GetActiveAudioSessionsCmd.CanExecute(null))
                GetActiveAudioSessionsCmd.Execute(null);

            // Get active apps timer
            var getActiveAppsTimer = new Timer(1000);

            getActiveAppsTimer.Elapsed += (sender, args) =>
            {
                // Remove inactive processes
                foreach (var activeApp in ActiveAppsCollection.ToList().Where(x => !ProcessExtensions.ProcessExists(x.ProcessId)))
                {
                    Debug.WriteLine($"GetActiveApplications - Removing application: '{activeApp.MainWindowTitle}' Id: '{activeApp.ProcessId}'");
                    ActiveAppsCollection.Remove(activeApp);
                }

                if (GetActiveAudioSessionsCmd.CanExecute(null))
                    GetActiveAudioSessionsCmd.Execute(null);
            };

            getActiveAppsTimer.Start();
        }

        /*
         * Private fields
         */

        private float _masterAudioLevel;
        private float _masterAudioPeak;
        private bool _masterAudioIsMuted;
        private string _masterDeviceName;
        private ObservableImmutableList<ActiveApp> _activeAppsCollection;
        private RelayCommand _getMasterAudioVolumeCmd;
        private RelayCommand _setMasterAudioVolumeCmd;
        private RelayCommand _getMasterAudioPeakCmd;
        private RelayCommand _getMasterDeviceNameCmd;
        private RelayCommand _getActiveAudioSessionsCmd;
        private RelayCommand _setAudioSessionVolumeCmd;

        /*
         * Properties
         */

        public float MasterAudioLevel
        {
            get => _masterAudioLevel;
            set
            {
                if (Math.Abs(value - _masterAudioLevel) < 0.01) return; _masterAudioLevel = value; OnPropertyChanged();
                if (SetMasterAudioVolumeCmd.CanExecute(null))
                    SetMasterAudioVolumeCmd.Execute(null);
            }
        }

        public float MasterAudioPeak
        {
            get => _masterAudioPeak; set { if (Math.Abs(value - _masterAudioPeak) < 0.01) return; _masterAudioPeak = value; OnPropertyChanged(); }
        }

        public bool MasterAudioIsMuted
        {
            get => _masterAudioIsMuted; set { if (value == _masterAudioIsMuted) return; _masterAudioIsMuted = value; OnPropertyChanged(); }
        }

        public string MasterDeviceName
        {
            get => _masterDeviceName; set { if (value == _masterDeviceName) return; _masterDeviceName = value; OnPropertyChanged(); }
        }

        public ObservableImmutableList<ActiveApp> ActiveAppsCollection
        {
            get => _activeAppsCollection; set { if (value == _activeAppsCollection) return; _activeAppsCollection = value; OnPropertyChanged(); }
        }

        /*
         * Commands
         */

        public RelayCommand GetMasterAudioVolumeCmd
        {
            get { return _getMasterAudioVolumeCmd ?? (_getMasterAudioVolumeCmd = new RelayCommand(Execute_GetMasterAudioVolumeCmd, p => true)); }
        }

        public RelayCommand SetMasterAudioVolumeCmd
        {
            get { return _setMasterAudioVolumeCmd ?? (_setMasterAudioVolumeCmd = new RelayCommand(Execute_SetMasterAudioVolumeCmd, p => true)); }
        }

        public RelayCommand GetMasterAudioPeakCmd
        {
            get { return _getMasterAudioPeakCmd ?? (_getMasterAudioPeakCmd = new RelayCommand(Execute_GetMasterAudioPeakCmd, p => true)); }
        }

        public RelayCommand GetMasterDeviceNameCmd
        {
            get { return _getMasterDeviceNameCmd ?? (_getMasterDeviceNameCmd = new RelayCommand(Execute_GetMasterDeviceNameCmd, p => true)); }
        }

        public RelayCommand GetActiveAudioSessionsCmd
        {
            get { return _getActiveAudioSessionsCmd ?? (_getActiveAudioSessionsCmd = new RelayCommand(Execute_GetActiveAudioSessionsCmd, p => true)); }
        }

        public RelayCommand SetAudioSessionVolumeCmd
        {
            get { return _setAudioSessionVolumeCmd ?? (_setAudioSessionVolumeCmd = new RelayCommand(p => Execute_SetAudioSessionVolumeCmd(p as ActiveApp), p => p is ActiveApp)); }
        }

        /*
         * Methods
         */

        private void Execute_GetMasterAudioVolumeCmd(object obj)
        {
            using (var device = AudioManager.GetDefaultRenderDevice())
            {
                using (var endpointVolume = AudioEndpointVolume.FromDevice(device))
                {
                    var volume = endpointVolume.GetMasterVolumeLevelScalar();
                    MasterAudioLevel = volume;

                    var isMuted = endpointVolume.GetMute();
                    MasterAudioIsMuted = isMuted;
                }
            }
        }

        private void Execute_SetMasterAudioVolumeCmd(object obj)
        {
            try
            {
                using (var device = AudioManager.GetDefaultRenderDevice())
                {
                    using (var endpointVolume = AudioEndpointVolume.FromDevice(device))
                    {
                        endpointVolume.SetMasterVolumeLevelScalar(MasterAudioLevel, Guid.Empty);

                        if (obj == null) return;

                        var isMuted = Convert.ToBoolean(obj);

                        if (endpointVolume.GetMute() && isMuted) return;
                        if (!endpointVolume.GetMute() && !isMuted) return;

                        endpointVolume.SetMuteNative(isMuted, Guid.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void Execute_GetMasterAudioPeakCmd(object obj)
        {
            using (var device = AudioManager.GetDefaultRenderDevice())
            using (var meter = AudioMeterInformation.FromDevice(device))
                MasterAudioPeak = meter.PeakValue * 100;
        }

        private void Execute_GetMasterDeviceNameCmd(object obj)
        {
            using (var device = AudioManager.GetDefaultRenderDevice())
                MasterDeviceName = device.FriendlyName;
        }

        private void Execute_GetActiveAudioSessionsCmd(object obj = null)
        {
            try
            {
                using (var sessionManager = AudioManager.GetDefaultAudioSessionManager2(DataFlow.Render))
                {
                    using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                    {
                        foreach (var session in sessionEnumerator)
                        {
                            using (var sessionControl = session.QueryInterface<AudioSessionControl2>())
                            {
                                // Get process main window title
                                var mainWindowTitle = sessionControl.Process?.MainWindowTitle;
                                if (string.IsNullOrEmpty(mainWindowTitle)) continue;

                                // Get process name
                                var processName = sessionControl.Process?.ProcessName;
                                if (string.IsNullOrEmpty(processName)) continue;

                                // Get process id
                                var processId = sessionControl.ProcessID;
                                if (processId.Equals(0)) continue;

                                // Get process icon
                                var processIcon = sessionControl.IconPath;

                                // Get volume level and is muted
                                float volume;
                                bool isMuted;
                                using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                                {
                                    volume = simpleVolume.MasterVolume;
                                    isMuted = simpleVolume.IsMuted;
                                }

                                // Set new main window title if process id exists and main window title does not match
                                if (ActiveAppsCollection.Any(x => x.ProcessId.Equals(processId) && !x.MainWindowTitle.Equals(mainWindowTitle)))
                                {
                                    Debug.WriteLine($"GetActiveApplications - Renaming application: '{mainWindowTitle}' Id: '{processId}'");

                                    var activeApps = ActiveAppsCollection.Where(x => x.ProcessId.Equals(processId) && !x.MainWindowTitle.Equals(mainWindowTitle));
                                    foreach (var activeApp in activeApps)
                                        activeApp.MainWindowTitle = mainWindowTitle;
                                }

                                // If the application does not exist in the collection, add it
                                if (ActiveAppsCollection.Any(x => x.MainWindowTitle.Equals(mainWindowTitle))) continue;

                                Debug.WriteLine($"GetActiveApplications - Adding application: '{mainWindowTitle}' Id: '{processId}' Volume: '{volume}'");

                                ActiveAppsCollection.Add(new ActiveApp
                                {
                                    MainWindowTitle = mainWindowTitle,
                                    ProcessName = processName,
                                    ProcessId = processId,
                                    IconPath = processIcon,
                                    VolumeLevel = volume,
                                    IsMuted = isMuted
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static void Execute_SetAudioSessionVolumeCmd(ActiveApp activeApp)
        {
            using (var sessionManager = AudioManager.GetDefaultAudioSessionManager2(DataFlow.Render))
            {
                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {
                    foreach (var session in sessionEnumerator)
                    {
                        using (var session2 = session.QueryInterface<AudioSessionControl2>())
                        {
                            if (!session2.ProcessID.Equals(activeApp.ProcessId)) continue;

                            using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                            {
                                simpleVolume.MasterVolume = activeApp.VolumeLevel;
                                simpleVolume.IsMuted = activeApp.IsMuted;
                            }
                        }
                    }
                }
            }
        }
    }
}
