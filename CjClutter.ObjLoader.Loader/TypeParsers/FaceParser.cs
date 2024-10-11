using System;
using System.Threading.Tasks;
using CjClutter.ObjLoader.Loader.Common;
using CjClutter.ObjLoader.Loader.Data.DataStore;
using CjClutter.ObjLoader.Loader.Data.Elements;
using CjClutter.ObjLoader.Loader.TypeParsers.Interfaces;

namespace CjClutter.ObjLoader.Loader.TypeParsers
{
    public class FaceParser : TypeParserBase, IFaceParser
    {
        private readonly IFaceGroup _faceGroup;

        public FaceParser(IFaceGroup faceGroup)
        {
            _faceGroup = faceGroup;
        }

        protected override string Keyword
        {
            get { return "f"; }
        }

        public override Task Parse(string line)
        {
            var vertices = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var face = new Face();

            foreach (var vertexString in vertices)
            {
                var faceVertex = ParseFaceVertex(vertexString);
                face.AddVertex(faceVertex);
            }

            _faceGroup.AddFace(face);

            return Task.CompletedTask;
        }

        private FaceVertex ParseFaceVertex(string vertexString)
        {
            var fields = vertexString.Split(new[]{'/'}, StringSplitOptions.None);

            var vertexIndex = fields[0].ParseInvariantInt();
            var faceVertex = new FaceVertex(vertexIndex, 0, 0);

            if(fields.Length > 1)
            {
                var textureIndex = fields[1].Length == 0 ? 0 : fields[1].ParseInvariantInt();
                faceVertex.TextureIndex = textureIndex;
            }

            if(fields.Length > 2)
            {
                var normalIndex = fields.Length > 2 && fields[2].Length == 0 ? 0 : fields[2].ParseInvariantInt();
                faceVertex.NormalIndex = normalIndex;
            }

            return faceVertex;
        }
    }
}