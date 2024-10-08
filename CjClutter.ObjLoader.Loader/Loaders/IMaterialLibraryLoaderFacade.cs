using System.Threading.Tasks;

namespace ObjLoader.Loader.Loaders
{
    public interface IMaterialLibraryLoaderFacade
    {
        Task Load(string materialFileName);
    }
}