using System;
using System.Runtime.InteropServices;

namespace Index.Common
{

  public class TickProfiler
  {

    [DllImport( "kernel32.dll" )]
    static extern ulong GetTickCount64();

    private int _index;
    private int _sampleCount;
    private ulong[] _samples;

    private Action<double> _callback;

    public TickProfiler( int sampleCount, Action<double> callback )
    {
      _sampleCount = sampleCount;
      _samples = new ulong[ sampleCount ];
      _callback = callback;
    }

    public void Register()
    {
      var samples = _samples;
      var sampleCount = _sampleCount;

      samples[ _index++ ] = GetTickCount64();
      if ( _index < sampleCount )
        return;

      double sum = 0;
      for ( var i = 1; i < sampleCount; i++ )
        sum += samples[ i ] - samples[ i - 1 ];

      sum /= sampleCount - 1;
      _callback( sum );

      _index = 0;
    }

  }

}
