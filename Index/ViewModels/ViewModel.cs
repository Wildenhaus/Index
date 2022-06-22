using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Index.ViewModels
{

  public abstract class ViewModel : INotifyPropertyChanged
  {

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Private Methods

    protected void RaisePropertyChanged( [CallerMemberName] string propertyName = null )
    {
      PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    protected void SetProperty<T>( ref T propertyField, T value, [CallerMemberName] string propertyName = null )
    {
      if ( propertyField?.Equals( value ) ?? false )
        return;

      propertyField = value;
      RaisePropertyChanged( propertyName );
    }

    #endregion

  }

}
