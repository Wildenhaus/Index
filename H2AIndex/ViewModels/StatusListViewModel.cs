using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using H2AIndex.Common;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;

namespace H2AIndex.ViewModels
{

  public class StatusListViewModel : ViewModel
  {

    public string SummaryText { get; set; }

    public ICollection<StatusListEntryModel> Entries { get; }
    public Brush StatusIconBrush { get; set; }

    [OnChangedMethod( nameof( OnStatusListChanged ) )]
    public StatusList StatusList { get; set; }


    public StatusListViewModel( IServiceProvider serviceProvider )
      : base( serviceProvider )
    {
      Entries = new ObservableCollection<StatusListEntryModel>();
    }

    #region Private Methods

    private void OnStatusListChanged()
    {
      SummaryText = string.Format( "Process completed with {0}, {1}, and {2}",
        Pluralize( "error", StatusList.Errors.Count ),
        Pluralize( "warning", StatusList.Warnings.Count ),
        Pluralize( "message", StatusList.Messages.Count ) );

      if ( StatusList.HasErrors )
        StatusIconBrush = ( Brush ) App.Current.FindResource( "Brush.Error" );
      else if ( StatusList.HasWarnings )
        StatusIconBrush = ( Brush ) App.Current.FindResource( "Brush.Warning" );
      else
        StatusIconBrush = ( Brush ) App.Current.FindResource( "Brush.Message" );

      Entries.Clear();
      foreach ( var entry in StatusList )
        Entries.Add( new StatusListEntryModel( entry ) );
    }

    private string Pluralize( string name, int count )
      => $"{count} {( count == 1 ? name : name + "s" )}";

    #endregion

    #region Embedded Types

    public class StatusListEntryModel : ObservableObject
    {

      public StatusListEntryType Type { get; set; }
      public string Name { get; set; }
      public string Message { get; set; }
      public Exception Exception { get; set; }
      public ICommand ShowExceptionCommand { get; }

      public StatusListEntryModel( StatusList.Entry entry )
      {
        Type = entry.Type;
        Name = entry.Name;
        Message = entry.Message;
        Exception = entry.Exception;

        var mainViewModel = ( ( App ) App.Current ).ServiceProvider.GetRequiredService<MainViewModel>();
        ShowExceptionCommand = mainViewModel.ShowExceptionModalCommand;
      }

    }

    #endregion

  }
}
