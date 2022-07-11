using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Saber3D.Common;
using static Saber3D.Assertions;

namespace Saber3D.Files
{

  public static class S3DFileFactory
  {

    #region Delegates

    private delegate IS3DFile CreateFileDelegate( string name, Stream stream, IS3DFile parent = null );

    #endregion

    #region Data Members

    public static readonly IReadOnlyCollection<string> RecognizedExtensions;

    private static Dictionary<string, Type> _extensionLookup;
    private static Dictionary<string, Type> _signatureLookup;

    private static Dictionary<Type, CreateFileDelegate> _constructorLookup;

    #endregion

    #region Constructor

    static S3DFileFactory()
    {
      _extensionLookup = BuildExtensionLookup();
      _signatureLookup = BuildSignatureLookup();
      _constructorLookup = BuildConstructorLookup();

      RecognizedExtensions = new HashSet<string>( _extensionLookup.Keys );
    }

    #endregion

    #region Public Methods

    public static IS3DFile CreateFile( string name, Stream stream, IS3DFile parent = null )
    {
      stream.Position = 0;
      var ext = Path.GetExtension( name );

      if ( !_extensionLookup.TryGetValue( ext, out var fileType ) )
      {
        var signature = ReadSignature( stream );
        if ( !_signatureLookup.TryGetValue( signature, out fileType ) )
          return FailReturn<IS3DFile>( $"Could not determine a FileType for '{name}'." );
      }

      if ( !_constructorLookup.TryGetValue( fileType, out var ctorDelegate ) )
        return FailReturn<IS3DFile>( $"FileType '{fileType.Name}' does not have a constructor delegate!" );

      return ctorDelegate( name, stream, parent );
    }

    #endregion

    #region Private Methods

    private static string ReadSignature( Stream stream )
    {
      stream.Position = 0;
      var reader = new BinaryReader( stream, System.Text.Encoding.UTF8, true );

      var signature = reader.ReadStringNullTerminated( maxLength: 32 );
      reader.BaseStream.Position = 0;

      return signature;
    }

    private static CreateFileDelegate BuildConstructorDelegate( Type fileType )
    {
      const BindingFlags BINDING_FLAGS =
        BindingFlags.CreateInstance |
        BindingFlags.Instance |
        BindingFlags.Public |
        BindingFlags.NonPublic;

      // Get the constructor
      var ctorArgTypes = new Type[] { typeof( string ), typeof( Stream ), typeof( IS3DFile ) };
      var ctorMethod = fileType.GetConstructor( BINDING_FLAGS, null, ctorArgTypes, new ParameterModifier[ 0 ] );
      Assert( ctorMethod != null, $"Could not find constructor for {fileType.Name}" );

      // Get the initialize method
      var initializeMethod = fileType.GetMethod( "Initialize", BINDING_FLAGS );
      Assert( ctorMethod != null, $"Could not find initialize method for {fileType.Name}" );

      // Initialize the call arguments
      var nameParameter = Expression.Parameter( typeof( string ), "name" );
      var streamParameter = Expression.Parameter( typeof( Stream ), "stream" );
      var parentParameter = Expression.Parameter( typeof( IS3DFile ), "parent" );
      var parameters = new[] { nameParameter, streamParameter, parentParameter };

      // Initialize the local variables
      var instanceVariable = Expression.Variable( fileType, "instance" );

      var expressionBlock = Expression.Block(
        variables: new[] { instanceVariable },
        new Expression[]
        {
          // Call the constructor and set the instance variable
          Expression.Assign( instanceVariable, Expression.New( ctorMethod, parameters ) ),

          // Call the initialize method
          Expression.Call( instanceVariable, initializeMethod ),

          // Push the instance to the stack for return
          instanceVariable
        } );

      return Expression.Lambda<CreateFileDelegate>( expressionBlock, parameters ).Compile();
    }

    private static IEnumerable<Type> GetDefinedFileTypes()
    {
      return typeof( S3DFile ).Assembly.GetTypes()
        .Where( x => !x.IsAbstract && typeof( IS3DFile ).IsAssignableFrom( x ) );
    }

    private static Dictionary<Type, CreateFileDelegate> BuildConstructorLookup()
    {
      var ctorLookup = new Dictionary<Type, CreateFileDelegate>();

      foreach ( var fileType in GetDefinedFileTypes() )
        ctorLookup.Add( fileType, BuildConstructorDelegate( fileType ) );

      return ctorLookup;
    }

    private static Dictionary<string, Type> BuildExtensionLookup()
    {
      var extLookup = new Dictionary<string, Type>();

      foreach ( var fileType in GetDefinedFileTypes() )
      {
        var extAttributes = fileType.GetCustomAttributes( typeof( FileExtensionAttribute ), false )
          .Cast<FileExtensionAttribute>();

        foreach ( var extAttribute in extAttributes )
        {
          var extension = extAttribute.FileExtension;
          extLookup.Add( extension, fileType );
        }
      }

      return extLookup;
    }

    private static Dictionary<string, Type> BuildSignatureLookup()
    {
      var sigLookup = new Dictionary<string, Type>();

      foreach ( var fileType in GetDefinedFileTypes() )
      {
        var signatureAttribute = fileType.GetCustomAttributes( typeof( FileSignatureAttribute ), false ).FirstOrDefault() as FileSignatureAttribute;
        if ( signatureAttribute is null )
          continue;

        sigLookup.Add( signatureAttribute.Signature, fileType );
      }

      return sigLookup;
    }

    #endregion

  }

}
