using System.Threading.Tasks;
using CjClutter.ObjLoader.Loader.Data.DataStore;
using CjClutter.ObjLoader.Loader.TypeParsers.Interfaces;

namespace CjClutter.ObjLoader.Loader.TypeParsers
{
    public class GroupParser : TypeParserBase, IGroupParser
    {
        private readonly IGroupDataStore _groupDataStore;

        public GroupParser(IGroupDataStore groupDataStore)
        {
            _groupDataStore = groupDataStore;
        }

        protected override string Keyword
        {
            get { return "g"; }
        }

        public override Task Parse(string line)
        {
            _groupDataStore.PushGroup(line);
            return Task.CompletedTask;
        }
    }
}