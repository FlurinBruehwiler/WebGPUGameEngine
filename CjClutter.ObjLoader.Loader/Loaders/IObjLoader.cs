using System.IO;
using System.Threading.Tasks;

namespace ObjLoader.Loader.Loaders
{
    public interface IObjLoader
    {
        Task<LoadResult> Load(Stream lineStream);
    }
}