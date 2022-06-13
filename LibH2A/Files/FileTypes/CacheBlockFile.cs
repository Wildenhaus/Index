using System.IO;
using Saber3D.Common;

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

    public CacheBlockFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
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
       * S3DFileFactory by just treating them all as generic text files.
       * 
       * If we want to convert material settings when we're exporting models, we'll need to
       * deserialize the .td files.
       */

      var stream = new StreamSegment( BaseStream, offset, size );
      return new GenericTextFile( name, stream, this );
    }

    #endregion

  }

}
