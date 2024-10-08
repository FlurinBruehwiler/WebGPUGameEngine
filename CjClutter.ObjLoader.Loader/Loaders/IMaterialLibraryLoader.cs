using System.IO;
using System.Threading.Tasks;

namespace ObjLoader.Loader.Loaders
{
    public interface IMaterialLibraryLoader
    {
        Task Load(Stream lineStream);
    }
}