using System.Threading.Tasks;
using CjClutter.ObjLoader.Loader.Loaders;
using CjClutter.ObjLoader.Loader.TypeParsers.Interfaces;

namespace CjClutter.ObjLoader.Loader.TypeParsers
{
    public class MaterialLibraryParser : TypeParserBase, IMaterialLibraryParser
    {
        private readonly IMaterialLibraryLoaderFacade _libraryLoaderFacade;

        public MaterialLibraryParser(IMaterialLibraryLoaderFacade libraryLoaderFacade)
        {
            _libraryLoaderFacade = libraryLoaderFacade;
        }

        protected override string Keyword
        {
            get { return "mtllib"; }
        }

        public override Task Parse(string line)
        {
            return _libraryLoaderFacade.Load(line);
        }
    }
}