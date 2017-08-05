using System.Timers;
using Kaeos.Extensions;

namespace Kaeos.Helpers
{
    public static class TimerHelper
    {
        public static void BeginIntervalTimer(int interval, RelayCommand command, object commandParameter = null, bool executeOnStart = true)
        {
            // Start command on start
            if (executeOnStart)
                if (command.CanExecute(commandParameter))
                    command.Execute(commandParameter);

            var timer = new Timer(interval);

            timer.Elapsed += (sender, args) =>
            {
                if (command.CanExecute(commandParameter))
                    command.Execute(commandParameter);
            };

            timer.Start();
        }
    }
}
