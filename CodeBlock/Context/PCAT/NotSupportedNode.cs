using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class NotSupportedNode : BaseNode
    {
        public override string AcceptedTypeNames => "*";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            yield return NewInterrupt("Unsupported node type : " + Type,this);
            throw new NotImplementedException();
        }
    }
}
