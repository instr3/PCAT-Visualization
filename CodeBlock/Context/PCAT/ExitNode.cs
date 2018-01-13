using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class ExitNode : BaseNode
    {
        public override string AcceptedTypeNames => "exit";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            yield return NewInterrupt("exit", null);
            yield break;
        }
    }
}
