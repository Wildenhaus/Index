using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
      tab.CloseAllRequested += OnCloseAllTabs;
      tab.CloseAllButThisRequested += OnCloseAllButSender;
      Tabs.Add( tab );
      CurrentTab = tab;
    }

    public void CloseTab( ITab tab )
    {
      Tabs.Remove( tab );
      tab.CloseRequested -= OnCloseTab;
      tab.CloseAllRequested -= OnCloseAllTabs;
      tab.CloseAllButThisRequested -= OnCloseAllButSender;

      tab.Dispose();
    }

    #endregion

    #region Event Handlers

    private void OnCloseTab( object sender, EventArgs e )
    {
      var tab = sender as ITab;
      if ( tab is null )
        return;

      CloseTab( tab );
    }

    private void OnCloseAllTabs( object sender, EventArgs e )
    {
      var tabs = Tabs.ToArray();
      foreach ( var tab in tabs )
        CloseTab( tab );
    }

    private void OnCloseAllButSender( object sender, EventArgs e )
    {
      var senderTab = sender as ITab;
      if ( senderTab is null )
        return;

      var tabs = Tabs.ToArray();
      foreach ( var tab in tabs )
        if ( tab != senderTab )
          CloseTab( tab );
    }

    #endregion

  }

}
