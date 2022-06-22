using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Index.Common
{

  public abstract class ObservableObject : INotifyPropertyChanged
  {

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Private Methods

    protected void NotifyPropertyChanged( [CallerMemberName] string propertyName = null )
    {
      PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    #endregion

  }

}
