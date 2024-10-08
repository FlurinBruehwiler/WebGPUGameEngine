using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ObjLoader.Loader.Data.DataStore;
using ObjLoader.Loader.TypeParsers.Interfaces;

namespace ObjLoader.Loader.Loaders
{
    public class ObjLoader : LoaderBase, IObjLoader
    {
        private readonly IDataStore _dataStore;
        private readonly List<ITypeParser> _typeParsers = new List<ITypeParser>();

        private readonly List<string> _unrecognizedLines = new List<string>();

        public ObjLoader(
            IDataStore dataStore, 
            IFaceParser faceParser, 
            IGroupParser groupParser,
            INormalParser normalParser, 
            ITextureParser textureParser, 
            IVertexParser vertexParser,
            IMaterialLibraryParser materialLibraryParser, 
            IUseMaterialParser useMaterialParser)
        {
            _dataStore = dataStore;
            SetupTypeParsers(
                vertexParser,
                faceParser,
                normalParser,
                textureParser,
                groupParser,
                materialLibraryParser,
                useMaterialParser);
        }

        private void SetupTypeParsers(params ITypeParser[] parsers)
        {
            foreach (var parser in parsers)
            {
                _typeParsers.Add(parser);
            }
        }

        protected override Task ParseLine(string keyword, string data)
        {
            foreach (var typeParser in _typeParsers)
            {
                if (typeParser.CanParse(keyword))
                {
                    return typeParser.Parse(data);
                }
            }

            _unrecognizedLines.Add(keyword + " " + data);
            return Task.CompletedTask;
        }

        public async Task<LoadResult> Load(Stream lineStream)
        {
            await StartLoad(lineStream);

            return CreateResult();
        }

        private LoadResult CreateResult()
        {
            var result = new LoadResult
                             {
                                 Vertices = _dataStore.Vertices,
                                 Textures = _dataStore.Textures,
                                 Normals = _dataStore.Normals,
                                 Groups = _dataStore.Groups,
                                 Materials = _dataStore.Materials
                             };
            return result;
        }
    }
}