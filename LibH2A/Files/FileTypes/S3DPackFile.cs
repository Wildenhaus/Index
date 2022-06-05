using System.Collections.Generic;
using System.IO;
using System.Linq;
using Saber3D.Common;
using static Saber3D.Assertions;

namespace Saber3D.Files
{

  public class S3DPackFile : S3DFile, IS3DArchiveFile
  {

    #region Constants

    private const int SIGNATURE_PAK = 0x006B6170;

    #endregion

    #region Data Members

    private object _readLock;
    private Dictionary<string, IS3DArchiveFileEntry> _entries;

    #endregion

    #region Properties

    public IReadOnlyDictionary<string, IS3DArchiveFileEntry> Entries
    {
      get => _entries;
    }

    #endregion

    #region Constructor

    private S3DPackFile( string name, H2ADecompressionStream stream, bool keepStreamOpen )
      : base( name, stream, keepStreamOpen )
    {
      _readLock = new object();
    }

    public static S3DPackFile FromStream( string name, H2ADecompressionStream stream, bool keepStreamOpen = false )
    {
      var file = new S3DPackFile( name, stream, keepStreamOpen );
      file.Initialize();

      return file;
    }

    public static S3DPackFile FromFile( string filePath, bool keepStreamOpen = false )
    {
      var name = Path.GetFileName( filePath );
      return FromStream( name, H2ADecompressionStream.FromFile( filePath ), keepStreamOpen );
    }

    #endregion

    #region Public Methods

    public Stream GetEntryStream( IS3DArchiveFileEntry entry )
    {
      lock ( _entries )
        if ( !_entries.ContainsKey( entry.Name ) )
          return FailReturn<Stream>( $"Entry not found: {entry.Name}" );

      lock ( _readLock )
      {
        BaseStream.Position = entry.Offset;

        /* I'm not sure what future use cases this library will have, but I'm
         * trying to plan ahead for multithreading.
         * 
         * Right now, I'm just allocating a new stream for consumption. 
         * We could make a multithreaded read stream where it saves/loads positions,
         * but I'm not sure what performance implications that will have.
         */

        // Create a stream for the entry
        var entryData = new byte[ entry.SizeInBytes ];
        BaseStream.Read( entryData, 0, entry.SizeInBytes );

        return new MemoryStream( entryData );
      }
    }

    #endregion

    #region Overrides

    protected override void OnInitialize()
    {
      BaseStream.Position = 0;
      CheckSignature( SIGNATURE_PAK );

      ReadHeader();

      var entries = _entries = new Dictionary<string, IS3DArchiveFileEntry>();
      foreach ( var entry in ReadEntries() )
        entries[ entry.Name ] = entry; // TODO: this is gonna overwrite duplicates
    }

    #endregion

    #region Private Methods

    private void ReadHeader()
    {
      // TODO: Read this
      BaseStream.Position = 0x45;
    }

    private IEnumerable<IS3DArchiveFileEntry> ReadEntries()
    {
      var entryCount = Reader.ReadInt32();
      _ = Reader.ReadInt32(); // TODO: Unk
      _ = Reader.ReadByte(); // Delimiter

      // Read Entry Names
      var names = new string[ entryCount ];
      for ( var i = 0; i < entryCount; i++ )
        names[ i ] = Reader.ReadPascalString32();

      _ = Reader.ReadByte(); // Delimiter

      // Read Entry Offsets
      var offsets = new long[ entryCount ];
      for ( var i = 0; i < entryCount; i++ )
        offsets[ i ] = Reader.ReadInt64();

      _ = Reader.ReadByte(); // Delimiter

      // Read Entry Sizes
      var sizes = new int[ entryCount ];
      for ( var i = 0; i < entryCount; i++ )
        sizes[ i ] = Reader.ReadInt32();

      // Create Entries
      var entries = new List<IS3DArchiveFileEntry>();
      for ( var i = 0; i < entryCount; i++ )
        entries.Add( new S3DArchiveEntryFile( this, names[ i ], offsets[ i ], sizes[ i ] ) );

      var finalEntries = new List<IS3DArchiveFileEntry>();
      foreach ( var entry in entries )
      {
        if ( entry.SizeInBytes == 0 )
          continue;

        var subEntries = ReadSubEntries( entry );
        if ( subEntries.Any() )
          finalEntries.AddRange( subEntries );
        else
          finalEntries.Add( entry );
      }

      return finalEntries;
    }

    private IEnumerable<IS3DArchiveFileEntry> ReadSubEntries( IS3DArchiveFileEntry parentEntry )
    {
      /* A lot of times, 1SERpak files contain 1SERpak files. There's definitely a better
       * way to do this, but for now I'm just reading 1 level deep and only including non-reference
       * files (files that actually have data instead of referencing other files).
       */

      BaseStream.Position = parentEntry.Offset;
      var subEntryOffset = BaseStream.Position;

      if ( Reader.ReadInt32() != SIGNATURE_SER || Reader.ReadInt32() != SIGNATURE_PAK )
        return Enumerable.Empty<IS3DArchiveFileEntry>();


      BaseStream.Position += 0x45 - 8;

      var entryCount = Reader.ReadInt32();
      _ = Reader.ReadInt32(); // TODO: Unk
      _ = Reader.ReadByte(); // Delimiter

      // Read Entry Names
      var names = new string[ entryCount ];
      for ( var i = 0; i < entryCount; i++ )
        names[ i ] = Reader.ReadPascalString32();

      _ = Reader.ReadByte(); // Delimiter

      // Read Entry Offsets
      var offsets = new long[ entryCount ];
      for ( var i = 0; i < entryCount; i++ )
        offsets[ i ] = Reader.ReadInt64() + subEntryOffset;

      _ = Reader.ReadByte(); // Delimiter

      // Read Entry Sizes
      var sizes = new int[ entryCount ];
      for ( var i = 0; i < entryCount; i++ )
        sizes[ i ] = Reader.ReadInt32();

      // Create Entries
      var entries = new List<IS3DArchiveFileEntry>();
      for ( var i = 0; i < entryCount; i++ )
        if ( sizes[ i ] > 0 )
          entries.Add( new S3DArchiveEntryFile( this, names[ i ], offsets[ i ], sizes[ i ] ) );

      var finalEntries = new List<IS3DArchiveFileEntry>();
      foreach ( var entry in entries )
      {
        if ( entry.SizeInBytes == 0 )
          continue;

        var subEntries = ReadSubEntries( entry );
        if ( subEntries.Any() )
          finalEntries.AddRange( subEntries );
        else
          finalEntries.Add( entry );
      }

      return finalEntries;
    }

    #endregion

  }

}
