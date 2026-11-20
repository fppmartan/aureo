using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Aureo.ViewModels
{
    /// <summary>
    /// Provides property change notification support for view models.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged; // Event that is raised when a property value changes, allowing the UI to update accordingly.
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Raises the PropertyChanged event, notifying any subscribers (typically the UI) that a property value has changed, so they can update accordingly.
        }
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) 
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        } // This method sets the value of a property and raises the PropertyChanged event if the value has changed. It returns true if the value was changed, otherwise false.
    }
}
