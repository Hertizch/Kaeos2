using System;
using Kaeos.Extensions;

namespace Kaeos.Models
{
    public class ActiveApp : ObservableObject
    {
        public ActiveApp()
        {
            
        }

        /*
         * Private fields
         */

        private string _mainWindowTitle;
        private int _processId;
        private string _processName;
        private string _iconPath;
        private float _volumeLevel;
        private bool _isMuted;

        /*
         * Properties
         */

        public string MainWindowTitle
        {
            get => _mainWindowTitle; set { if (value == _mainWindowTitle) return; _mainWindowTitle = value; OnPropertyChanged(); }
        }

        public int ProcessId
        {
            get => _processId; set { if (value == _processId) return; _processId = value; OnPropertyChanged(); }
        }

        public string ProcessName
        {
            get => _processName; set { if (value == _processName) return; _processName = value; OnPropertyChanged(); }
        }

        public string IconPath
        {
            get => _iconPath; set { if (value == _iconPath) return; _iconPath = value; OnPropertyChanged(); }
        }

        public float VolumeLevel
        {
            get => _volumeLevel; set { if (Math.Abs(value - _volumeLevel) < 0.01) return; _volumeLevel = value; OnPropertyChanged(); }
        }

        public bool IsMuted
        {
            get => _isMuted; set { if (value == _isMuted) return; _isMuted = value; OnPropertyChanged(); }
        }
    }
}
