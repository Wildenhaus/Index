using System;

namespace Saber3D.Serializers.Scripting
{

  public abstract class ScriptingReader : IDisposable
  {

    #region Data Members

    private string _currentToken;
    private ScriptingTokenType _tokenType;

    private bool _isDisposed;

    #endregion

    #region Properties

    public string Token
    {
      get => _currentToken;
      protected set => _currentToken = value;
    }

    public ScriptingTokenType TokenType
    {
      get => _tokenType;
      protected set => _tokenType = value;
    }

    #endregion

    #region Constructor

    protected ScriptingReader()
    {
    }

    ~ScriptingReader()
    {
      Dispose( false );
    }

    #endregion

    #region Public Methods

    public abstract bool Read();

    #endregion

    #region IDisposable Methods

    public void Dispose()
      => Dispose( true );

    private void Dispose( bool disposing )
    {
      if ( _isDisposed )
        return;

      OnDisposing( disposing );

      _isDisposed = true;
    }

    protected virtual void OnDisposing( bool disposing )
    {
    }

    #endregion

  }

}
