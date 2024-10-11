using System.Threading.Tasks;

namespace CjClutter.ObjLoader.Loader.TypeParsers.Interfaces
{
    public interface ITypeParser
    {
        bool CanParse(string keyword);
        Task Parse(string line);
    }
}