using System.IO;
using Saber3D.Common;

namespace Saber3D.Files
{

  public abstract class S3DContainerFile : S3DFile
  {

    #region Constructor

    protected S3DContainerFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

    #region Overrides

    protected override void OnInitialize()
    {
      BaseStream.Position = 0;

      ReadHeader();
      ReadChildren();
    }

    #endregion

    #region Private Methods

    protected virtual IS3DFile CreateChildFile( string name, long offset, long size )
    {
      var dataStartOffset = CalculateTrueChildOffset( offset );
      var dataEndOffset = dataStartOffset + size;

      return S3DFileFactory.CreateFile( name, BaseStream, dataStartOffset, dataEndOffset, this );
    }

    protected long CalculateTrueChildOffset( long offset )
      => DataStartOffset + offset;

    private void ReadHeader()
    {
      // TODO
      // Skipping this for now. We might want to read it though.
      BaseStream.Position = DataStartOffset + 0x45;
    }

    private void ReadChildren()
    {
      var entryCount = Reader.ReadInt32();
      _ = Reader.ReadInt32(); // TODO: Unk

      var d = Reader.ReadByte(); // Delimiter

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

      // Create entries
      for ( var i = 0; i < entryCount; i++ )
      {
        var name = names[ i ];
        var offset = offsets[ i ];
        var size = sizes[ i ];

        /* If a file is 0 bytes long, it means the file is a dependency,
         * but it isn't present in this current Pck.
         * In this case, we just want to ignore it.
         */
        if ( size == 0 )
          continue;

        var childFile = CreateChildFile( name, offset, size );
        if ( childFile is null )
          continue;

        AddChild( childFile );
      }

    }

    protected override string SanitizeName( string fileName )
    {
      fileName = base.SanitizeName( fileName );

      /* A lot of times, files will be encapsulated in a Pck file, but will still
       * use the extension of the file they contain. We don't want these Pck containers
       * to use their child file's extension. The files within these containers have
       * offsets to data that do not account for the Pck header to be present.
       * 
       * In the UI, we'll want to hide Pck files from the FileTree to avoid confusion.
       */

      var ext = Path.GetExtension( fileName );
      if ( ext != ".pck" )
        fileName = Path.ChangeExtension( fileName, ".pck" );

      return fileName;
    }

    #endregion

  }

}
