using System.IO;
using System.Threading.Tasks;

namespace CjClutter.ObjLoader.Loader.Loaders
{
    public class MaterialStreamProvider : IMaterialStreamProvider
    {
        public Task<Stream> Open(string materialFilePath)
        {
            return Task.FromResult((Stream)File.Open(materialFilePath, FileMode.Open, FileAccess.Read));
        }
    }

    public class MaterialNullStreamProvider : IMaterialStreamProvider
    {
        public Task<Stream> Open(string materialFilePath)
        {
            return null;
        }
    }
}