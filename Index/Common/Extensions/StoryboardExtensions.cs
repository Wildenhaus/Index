using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Index.Common
{

  public static class StoryboardExtensions
  {

    public static Task BeginAsync( this Storyboard storyboard )
    {
      var tcs = new TaskCompletionSource<object>();
      storyboard.Completed += delegate
      {
        tcs.SetResult( null );
      };

      storyboard.Begin();
      return tcs.Task;
    }

  }

}
