using System;
using System.ComponentModel;

namespace Soft.Ui.Tests.Extensions
{
    /// <summary>
    /// Helper class to test PropertyChanged calls
    /// </summary>
    public static class NotifyPropertyChangedExtensions
    {
        public static bool IsPropertyChangedFired(this INotifyPropertyChanged notifyPropertyChanged, Action action, string propertyName)
        {
            var fired = false;
            notifyPropertyChanged.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == propertyName)
                {
                    fired = true;
                }
            };

            action();

            return fired;
        }
    }
}
