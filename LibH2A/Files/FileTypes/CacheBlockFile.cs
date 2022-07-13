using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERcache_block" )]
  [FileExtension( ".cache_block" )]
  public class CacheBlockFile : S3DContainerFile
  {

    #region Properties

    public override string FileTypeDisplay => "Cache Block (.cache_block)";

    #endregion

    #region Constructor

    public CacheBlockFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

    #region Overrides

    protected override IS3DFile CreateChildFile( string name, long offset, long size )
    {
      /* CacheBlock files are containers for scripts. They contain scripting, shader code,
       * material settings, and a lot more.
       * 
       * Since they are all plaintext files, we can avoid the performance overhead of 
       * S3DFileFactory by just treating them all as generic text files unless defined here.
       */

      var dataStartOffset = CalculateTrueChildOffset( offset );
      var dataEndOffset = dataStartOffset + size;

      switch ( Path.GetExtension( name ) )
      {
        case ".td":
          return new TextureDefinitionFile( name, BaseStream, dataStartOffset, dataEndOffset, this );

        default:
          return new GenericTextFile( name, BaseStream, dataStartOffset, dataEndOffset, this );
      }

    }

    #endregion

  }

}
