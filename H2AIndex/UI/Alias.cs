using System;
using System.Collections;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;

namespace H2AIndex.UI
{

  public class Alias : MarkupExtension
  {

    #region Properties

    public object ResourceKey { get; set; }

    #endregion

    #region Overrides

    public override object ProvideValue( IServiceProvider serviceProvider )
    {
      var provider = ( IRootObjectProvider ) serviceProvider.GetService( typeof( IRootObjectProvider ) );

      if ( provider is null )
        return null;

      var dictionary = provider.RootObject as IDictionary;
      if ( dictionary is null )
      {
        var element = provider.RootObject as FrameworkElement;
        return element?.TryFindResource( ResourceKey );
      }

      return dictionary[ ResourceKey ];
    }

    #endregion

  }

}
