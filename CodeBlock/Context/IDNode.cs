using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context
{
    class IDNode : BaseNode
    {
        public override string AcceptedTypeNames => "ID";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            string idName = GetCode();
            if (!Mediator.Instance.ExecutingNameSpace.ContainsKey(idName))
            {
                throw new Exception("Not decleared variable: " + idName);
            }
            me.Return(Mediator.Instance.ExecutingNameSpace[idName]);
            yield break;
        }
    }
}
