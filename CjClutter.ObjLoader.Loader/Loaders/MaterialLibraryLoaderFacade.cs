using System.Threading.Tasks;

namespace CjClutter.ObjLoader.Loader.Loaders
{
    public class MaterialLibraryLoaderFacade : IMaterialLibraryLoaderFacade
    {
        private readonly IMaterialLibraryLoader _loader;
        private readonly IMaterialStreamProvider _materialStreamProvider;

        public MaterialLibraryLoaderFacade(IMaterialLibraryLoader loader, IMaterialStreamProvider materialStreamProvider)
        {
            _loader = loader;
            _materialStreamProvider = materialStreamProvider;
        }

        public async Task Load(string materialFileName)
        {
            using (var stream = await _materialStreamProvider.Open(materialFileName))
            {
                if (stream != null)
                {
                    await _loader.Load(stream);
                }
            }
        }
    }
}