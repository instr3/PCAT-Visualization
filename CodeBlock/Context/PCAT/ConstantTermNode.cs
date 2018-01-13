using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class ConstantTermNode : BaseNode
    {
        public override string AcceptedTypeNames => "TRUE#FALSE";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            if (Type == "TRUE")
                me.Return(true);
            if (Type == "FALSE")
                me.Return(false);
            yield break;
        }
    }
}
