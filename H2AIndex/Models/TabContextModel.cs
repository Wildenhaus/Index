using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using H2AIndex.Common;

namespace H2AIndex.Models
{

  public class TabContextModel : ObservableObject
  {

    #region Properties

    public ICollection<ITab> Tabs { get; }
    public ITab CurrentTab { get; set; }

    #endregion

    #region Constructor

    public TabContextModel()
    {
      Tabs = new ObservableCollection<ITab>();
    }

    #endregion

    #region Public Methods

    public void AddTab( ITab tab )
    {
      tab.CloseRequested += OnCloseTab;
      Tabs.Add( tab );
      CurrentTab = tab;
    }

    #endregion

    #region Event Handlers

    private void OnCloseTab( object sender, EventArgs e )
    {
      var tab = sender as ITab;
      if ( tab is null )
        return;

      Tabs.Remove( tab );
      tab.CloseRequested -= OnCloseTab;
      tab.Dispose();
    }

    #endregion

  }

}
