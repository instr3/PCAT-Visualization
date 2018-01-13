using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class NodeListNotPause : BaseNode
    {
        public override string AcceptedTypeNames => "procedure_decl_list#declaration_list";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            foreach (BaseNode node in ChildID)
            {
                yield return node.Execute();
            }
        }
    }
}
