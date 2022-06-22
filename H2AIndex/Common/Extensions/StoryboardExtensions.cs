using System;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace H2AIndex.Common.Extensions
{

  public static class StoryboardExtensions
  {

    public static Task BeginAsync( this Storyboard storyboard )
    {
      var tcs = new TaskCompletionSource<object>();

      void OnCompleted( object sender, EventArgs e )
      {
        tcs.SetResult( null );
        storyboard.Completed -= OnCompleted;
      }

      storyboard.Completed += OnCompleted;

      storyboard.Begin();
      return tcs.Task;
    }

  }

}
