using System.Threading.Tasks;
using CjClutter.ObjLoader.Loader.Common;
using CjClutter.ObjLoader.Loader.TypeParsers.Interfaces;

namespace CjClutter.ObjLoader.Loader.TypeParsers
{
    public abstract class TypeParserBase : ITypeParser
    {
        protected abstract string Keyword { get; }

        public bool CanParse(string keyword)
        {
            return keyword.EqualsOrdinalIgnoreCase(Keyword);
        }

        public abstract Task Parse(string line);
    }
}