using System.Threading.Tasks;

namespace ObjLoader.Loader.TypeParsers.Interfaces
{
    public interface ITypeParser
    {
        bool CanParse(string keyword);
        Task Parse(string line);
    }
}