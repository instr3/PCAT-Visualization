using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class StatementListNode : BaseNode
    {
        public override string AcceptedTypeNames => "statement_list";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            foreach (BaseNode node in ChildID)
            {
                if(!(node.Type=="while"||node.Type=="if"||node.Type=="if_else"||
                    node.Type=="loop"||node.Type=="for"))
                    yield return NewPause(node);
                yield return node.Execute();
            }
        }
    }
}
