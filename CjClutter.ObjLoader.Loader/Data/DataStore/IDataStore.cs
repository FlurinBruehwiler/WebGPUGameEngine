using System.Collections.Generic;
using CjClutter.ObjLoader.Loader.Data.Elements;
using CjClutter.ObjLoader.Loader.Data.VertexData;

namespace CjClutter.ObjLoader.Loader.Data.DataStore
{
    public interface IDataStore 
    {
        IList<Vertex> Vertices { get; }
        IList<Texture> Textures { get; }
        IList<Normal> Normals { get; }
        IList<Material> Materials { get; }
        IList<Group> Groups { get; }
    }
}