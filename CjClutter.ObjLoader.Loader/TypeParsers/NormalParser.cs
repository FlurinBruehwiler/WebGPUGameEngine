using System.Threading.Tasks;
using CjClutter.ObjLoader.Loader.Common;
using CjClutter.ObjLoader.Loader.Data.DataStore;
using CjClutter.ObjLoader.Loader.Data.VertexData;
using CjClutter.ObjLoader.Loader.TypeParsers.Interfaces;

namespace CjClutter.ObjLoader.Loader.TypeParsers
{
    public class NormalParser : TypeParserBase, INormalParser
    {
        private readonly INormalDataStore _normalDataStore;

        public NormalParser(INormalDataStore normalDataStore)
        {
            _normalDataStore = normalDataStore;
        }

        protected override string Keyword
        {
            get { return "vn"; }
        }

        public override Task Parse(string line)
        {
            string[] parts = line.Split(' ');

            float x = parts[0].ParseInvariantFloat();
            float y = parts[1].ParseInvariantFloat();
            float z = parts[2].ParseInvariantFloat();

            var normal = new Normal(x, y, z);
            _normalDataStore.AddNormal(normal);
            return Task.CompletedTask;
        }
    }
}