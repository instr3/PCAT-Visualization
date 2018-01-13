using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class LoopNode : BaseNode
    {
        public override string AcceptedTypeNames => "loop";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            // Will only break on ExitNode
            while (true)
            {
                yield return Child["loop_statements"].Execute();
            }
        }
    }
}
