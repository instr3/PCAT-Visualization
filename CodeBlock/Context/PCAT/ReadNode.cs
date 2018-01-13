using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class ReadNode : BaseNode
    {
        public override string AcceptedTypeNames => "read";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            BaseNode thingsToRead = ChildID[0];
            foreach (BaseNode writeExpr in thingsToRead.ChildID)
            {
                string idName = writeExpr.GetCode();
                while(true)
                {
                    string inputValue = Mediator.Instance.GetUserInput("Please input " + idName);
                    try
                    {
                        Mediator.Instance.ExecutingNameSpace.Reassign(idName, inputValue);
                    }
                    catch
                    {
                        continue;
                    }
                    break;
                }
            }
            yield break;
        }
    }
}
