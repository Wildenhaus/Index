using System;
using System.Collections.Generic;
using System.Linq;
using H2AIndex.Common;
using H2AIndex.ViewModels;
using Saber3D.Files;

namespace H2AIndex.Services
{

  public class FileTypeService : IFileTypeService
  {

    #region Data Members

    // Dictionary<IS3DFileType, ViewModelType>
    private readonly Dictionary<Type, Type> _viewModelLookup;

    private readonly HashSet<string> _extensionsWithEditorSupport;

    #endregion

    #region Properties

    public IReadOnlySet<string> ExtensionsWithEditorSupport
    {
      get => _extensionsWithEditorSupport;
    }

    #endregion

    #region Constructor

    public FileTypeService()
    {
      _extensionsWithEditorSupport = BuildEditorExtensionsSet();
      _viewModelLookup = BuildViewModelLookup();
    }

    #endregion

    #region Public Methods

    public Type GetViewModelType( Type fileType )
    {
      _viewModelLookup.TryGetValue( fileType, out var viewModelType );
      return viewModelType;
    }

    #endregion

    #region Private Methods

    private static HashSet<string> BuildEditorExtensionsSet()
    {
      var set = new HashSet<string>();

      foreach ( var viewModelType in GetViewModelTypes() )
      {
        var attributes = viewModelType
          .GetCustomAttributes( typeof( AcceptsFileTypeAttribute ), true )
          .Cast<AcceptsFileTypeAttribute>();

        foreach ( var attribute in attributes )
        {
          var fileType = attribute.FileType;

          var extAttributes = fileType
            .GetCustomAttributes( typeof( FileExtensionAttribute ), false )
            .Cast<FileExtensionAttribute>();

          foreach ( var extAttribute in extAttributes )
            set.Add( extAttribute.FileExtension );
        }
      }

      return set;
    }

    private static Dictionary<Type, Type> BuildViewModelLookup()
    {
      var lookup = new Dictionary<Type, Type>();

      var viewModelTypes = GetViewModelTypes();
      foreach ( var viewModelType in viewModelTypes )
      {
        var attributes = viewModelType
          .GetCustomAttributes( typeof( AcceptsFileTypeAttribute ), true )
          .Cast<AcceptsFileTypeAttribute>();

        foreach ( var attribute in attributes )
          lookup.Add( attribute.FileType, viewModelType );
      }

      return lookup;
    }

    private static IEnumerable<Type> GetViewModelTypes()
    {
      return typeof( FileTypeService ).Assembly.GetTypes()
        .Where( x =>
        {
          if ( !x.IsClass || x.IsAbstract )
            return false;

          if ( !typeof( IViewModel ).IsAssignableFrom( x ) )
            return false;

          return true;
        } );
    }

    #endregion

  }

}
