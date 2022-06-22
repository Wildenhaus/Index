using System.Windows;
using System.Windows.Controls;

namespace Index.Controls
{

  public partial class ContentHostControl : ContentControl
  {

    #region Properties

    public static DependencyProperty ModalProperty
      = DependencyProperty.Register( nameof( Modal ), typeof( Control ), typeof( ContentHostControl ) );

    public Control Modal
    {
      get => ( Control ) GetValue( ModalProperty );
      set => SetValue( ModalProperty, value );
    }

    #endregion

    #region Constructor

    public ContentHostControl()
    {
      InitializeComponent();
    }

    #endregion

  }

}
