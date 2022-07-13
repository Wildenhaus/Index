using System.Collections.Generic;

namespace Saber3D.Common
{

  public class LruCache<TKey, TValue>
  {

    #region Data Members

    private int _capacity;
    private LinkedList<Entry> _nodes;
    private Dictionary<TKey, LinkedListNode<Entry>> _keys;

    #endregion

    #region Constructor

    public LruCache( int capacity )
    {
      _capacity = capacity;
      _nodes = new LinkedList<Entry>();
      _keys = new Dictionary<TKey, LinkedListNode<Entry>>();
    }

    #endregion

    #region Public Methods

    public bool TryGet( TKey key, out TValue value )
    {
      value = default;

      if ( !_keys.TryGetValue( key, out var node ) )
        return false;

      _nodes.Remove( node );
      _nodes.AddLast( node );

      value = node.Value.Value;
      return true;
    }

    public void Put( TKey key, TValue value )
    {
      if ( _keys.TryGetValue( key, out var node ) )
        _nodes.Remove( node );
      else if ( _nodes.Count >= _capacity )
      {
        var toRemove = _nodes.First;
        _nodes.RemoveFirst();
        _keys.Remove( toRemove.Value.Key );
      }

      var entry = new Entry( key, value );
      node = new LinkedListNode<Entry>( entry );
      _nodes.AddLast( node );
      _keys[ key ] = node;
    }

    #endregion

    #region Embedded Types

    private struct Entry
    {
      public TKey Key;
      public TValue Value;

      public Entry( TKey key, TValue value )
      {
        Key = key;
        Value = value;
      }

    }

    #endregion

  }

}
