using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class ProgramNode : BaseNode
    {
        public override string AcceptedTypeNames => "program";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            Mediator.Instance.ExecutingNameSpace = Variable.Root;
            yield return ChildID[0].Execute();
        }
    }
}
