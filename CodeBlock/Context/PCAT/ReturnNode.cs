using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class ReturnNode : BaseNode
    {
        public override string AcceptedTypeNames => "return";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            if(Child.ContainsKey("return_value"))
            {
                Return rhs = new Return();
                yield return Child["return_value"].Execute(rhs);
                Mediator.Instance.ExecutingNameSpace.Reassign("@return", rhs.Object);
            }
            yield return NewInterrupt("return", null);
            yield break; // Will never execute to there
        }
    }
}
