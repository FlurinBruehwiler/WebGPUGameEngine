using System.IO;
using System.Threading.Tasks;

namespace CjClutter.ObjLoader.Loader.Loaders
{
    public interface IMaterialLibraryLoader
    {
        Task Load(Stream lineStream);
    }
}