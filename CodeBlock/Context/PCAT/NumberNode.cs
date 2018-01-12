using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class NumberNode : BaseNode
    {
        public override string AcceptedTypeNames => "NUMBER";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            string code = GetCode();
            if (code.Contains('.'))
                me.Return(float.Parse(code));
            else
                me.Return(int.Parse(code));
            yield break;
        }
    }
}
