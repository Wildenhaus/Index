using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using H2AIndex.ViewModels;

namespace H2AIndex.Views
{

  public partial class ModelView : View<ModelViewModel>
  {

    #region Constructor

    public ModelView()
    {
      InitializeComponent();
      RemoveDirectionalViewKeyBindings();
      DataContextChanged += OnDataContextChanged;
    }

    #endregion

    #region Overrides

    protected override void OnDisposing()
    {
      DataContextChanged -= OnDataContextChanged;
      base.OnDisposing();
    }

    #endregion

    #region Private Methods

    private void RemoveDirectionalViewKeyBindings()
    {
      var toRemove = new List<KeyBinding>();
      foreach ( var binding in Viewport.InputBindings )
        if ( binding is KeyBinding keyBinding )
          toRemove.Add( keyBinding );

      foreach ( var binding in toRemove )
        Viewport.InputBindings.Remove( binding );
    }

    #endregion

    #region Event Handlers

    private void OnContextMenuLoaded( object sender, RoutedEventArgs e )
    {
      ( sender as ContextMenu ).DataContext = this.DataContext;
    }

    private void OnDataContextChanged( object sender, DependencyPropertyChangedEventArgs e )
    {
      if ( DataContext is ModelViewModel viewModel )
        viewModel.Viewport = Viewport;
    }

    #endregion

  }

}
