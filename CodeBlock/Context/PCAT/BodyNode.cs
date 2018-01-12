using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class bodynode : BaseNode
    {
        public override string AcceptedTypeNames => "body";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            yield return Child["declarations"].Execute();
            yield return Child["process"].Execute();

        }
    }
}
