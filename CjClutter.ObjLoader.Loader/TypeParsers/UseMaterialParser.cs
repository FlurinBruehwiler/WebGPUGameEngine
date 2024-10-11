using System.Threading.Tasks;
using CjClutter.ObjLoader.Loader.Data.DataStore;
using CjClutter.ObjLoader.Loader.TypeParsers.Interfaces;

namespace CjClutter.ObjLoader.Loader.TypeParsers
{
    public class UseMaterialParser : TypeParserBase, IUseMaterialParser
    {
        private readonly IElementGroup _elementGroup;

        public UseMaterialParser(IElementGroup elementGroup)
        {
            _elementGroup = elementGroup;
        }

        protected override string Keyword
        {
            get { return "usemtl"; }
        }

        public override Task Parse(string line)
        {
            _elementGroup.SetMaterial(line);
            return Task.CompletedTask;
        }
    }
}