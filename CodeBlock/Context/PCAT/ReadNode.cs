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
            foreach (BaseNode readExpr in thingsToRead.ChildID)
            {
                string idStr = readExpr.GetCode();
                string hint = "";
                while(true)
                {
                    string inputValue = Mediator.Instance.GetUserInput(hint + "Please input " + idStr);
                    if (inputValue == "")
                        continue;
                    VariableReference reference = null;
                    if (readExpr.Type != "ID")
                    {
                        Return lhs = new Return();
                        yield return readExpr.Execute(lhs);
                        reference = lhs.Reference;
                    }
                    try
                    {
                        if (reference is null)
                            Mediator.Instance.ExecutingNameSpace.Reassign(idStr, inputValue);
                        else
                            reference.Base.Reassign(reference.Offset, inputValue);
                    }
                    catch
                    {
                        hint = "Format error! ";
                        continue;
                    }
                    break;
                }
            }
            yield break;
        }
    }
}
