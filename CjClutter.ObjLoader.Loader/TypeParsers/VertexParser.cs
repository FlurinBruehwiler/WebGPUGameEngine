using System;
using System.Threading.Tasks;
using CjClutter.ObjLoader.Loader.Common;
using CjClutter.ObjLoader.Loader.Data.DataStore;
using CjClutter.ObjLoader.Loader.Data.VertexData;
using CjClutter.ObjLoader.Loader.TypeParsers.Interfaces;

namespace CjClutter.ObjLoader.Loader.TypeParsers
{
    public class VertexParser : TypeParserBase, IVertexParser
    {
        private readonly IVertexDataStore _vertexDataStore;

        public VertexParser(IVertexDataStore vertexDataStore)
        {
            _vertexDataStore = vertexDataStore;
        }

        protected override string Keyword
        {
            get { return "v"; }
        }

        public override Task Parse(string line)
        {
            string[] parts = line.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);

            var x = parts[0].ParseInvariantFloat();
            var y = parts[1].ParseInvariantFloat();
            var z = parts[2].ParseInvariantFloat();

            var vertex = new Vertex(x, y, z);
            _vertexDataStore.AddVertex(vertex);
            return Task.CompletedTask;
        }
    }
}