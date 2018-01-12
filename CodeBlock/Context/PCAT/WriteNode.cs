using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class WriteNode : BaseNode
    {
        public override string AcceptedTypeNames => "write";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            throw new NotImplementedException();
        }
    }
}
