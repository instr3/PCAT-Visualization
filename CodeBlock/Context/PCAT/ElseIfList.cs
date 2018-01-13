using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class ElseIfList : BaseNode
    {
        public override string AcceptedTypeNames => "elsif_sentence_list";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            Return result = new Return();
            bool firstTry = true;
            foreach (BaseNode elseif in ChildID)
            {
                if(firstTry || result.Bool==false)
                {
                    firstTry = false;
                    yield return elseif.Execute(result);
                }
            }
            me.Return(result.Bool);
        }
    }
}
