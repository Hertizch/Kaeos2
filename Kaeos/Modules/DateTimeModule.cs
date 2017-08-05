using Kaeos.Extensions;
using System;
using System.Timers;
using Kaeos.Models;
using Kaeos.ObservableImmutable;

namespace Kaeos.Modules
{
    public class DateTimeModule : ObservableObject
    {
        public DateTimeModule()
        {
            TimeFormatCollection = new ObservableImmutableList<TimeFormat>
            {
                new TimeFormat{ Id = 1, Code = "T", FriendlyView = DateTime.Now.ToString("T")},
                new TimeFormat{ Id = 2, Code = "t", FriendlyView = DateTime.Now.ToString("t")}
            };

            DateFormatCollection = new ObservableImmutableList<DateFormat>
            {
                new DateFormat{ Id = 3, Code = "D", FriendlyView = DateTime.Now.ToString("D")},
                new DateFormat{ Id = 4, Code = "m", FriendlyView = DateTime.Now.ToString("m")},
                new DateFormat{ Id = 5, Code = "y", FriendlyView = DateTime.Now.ToString("y")}
            };

            if (ControlModule.DesignMode)
                return;

            var timerNow = new Timer { Interval = 100 };
            timerNow.Elapsed += TimerNow_Elapsed; ;
            timerNow.Start();
        }

        /*
         * Private fields
         */

        private DateTime _currentDateTime;
        private ObservableImmutableList<TimeFormat> _timeFormatCollection;
        private ObservableImmutableList<DateFormat> _dateFormatCollection;

        /*
         * Properties
         */

        public DateTime CurrentDateTime
        {
            get => _currentDateTime; set { if (value == _currentDateTime) return; _currentDateTime = value; OnPropertyChanged(); }
        }

        public ObservableImmutableList<TimeFormat> TimeFormatCollection
        {
            get => _timeFormatCollection; set { if (value == _timeFormatCollection) return; _timeFormatCollection = value; OnPropertyChanged(); }
        }

        public ObservableImmutableList<DateFormat> DateFormatCollection
        {
            get => _dateFormatCollection; set { if (value == _dateFormatCollection) return; _dateFormatCollection = value; OnPropertyChanged(); }
        }

        /*
         * Methods
         */

        private void TimerNow_Elapsed(object sender, ElapsedEventArgs e)
        {
            CurrentDateTime = DateTime.Now;
        }
    }
}
