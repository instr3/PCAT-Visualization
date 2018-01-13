using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class NodeListFullPause : BaseNode
    {
        public override string AcceptedTypeNames => "var_decl_list#type_decl_list";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            foreach (BaseNode node in ChildID)
            {
                yield return NewPause(node);
                yield return node.Execute();
            }
        }
    }
}
