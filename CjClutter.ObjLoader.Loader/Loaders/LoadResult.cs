using System.Collections.Generic;
using CjClutter.ObjLoader.Loader.Data;
using CjClutter.ObjLoader.Loader.Data.Elements;
using CjClutter.ObjLoader.Loader.Data.VertexData;

namespace CjClutter.ObjLoader.Loader.Loaders
{
    public class LoadResult  
    {
        public IList<Vertex> Vertices { get; set; }
        public IList<Texture> Textures { get; set; }
        public IList<Normal> Normals { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<Material> Materials { get; set; }
    }
}