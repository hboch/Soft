using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// Abstract base class implementing INotifyPropertyChanged
/// </summary>
public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Fires PropertyChanged event for calling property or property name
    /// </summary>
    /// <param name="propertyName">Property name to fire event for</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}