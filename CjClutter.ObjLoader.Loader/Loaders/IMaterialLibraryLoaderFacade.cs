using System.Threading.Tasks;

namespace CjClutter.ObjLoader.Loader.Loaders
{
    public interface IMaterialLibraryLoaderFacade
    {
        Task Load(string materialFileName);
    }
}