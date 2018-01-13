using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class WriteNode : BaseNode
    {
        public override string AcceptedTypeNames => "write";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            BaseNode thingsToWrite = ChildID[0];
            string text = "";
            foreach (BaseNode writeExpr in thingsToWrite.ChildID)
            {
                if(writeExpr.Type == "STRING")
                {
                    string result = writeExpr.GetCode();
                    text += result.Substring(1, result.Length - 2);
                }
                else //writeExpr is expression
                {
                    Return rhs = new Return();
                    yield return writeExpr.Execute(rhs);
                    text += rhs.Object.ToString();
                }
            }
            Variable writeVar = Variable.Root.Get("$OUTPUT$") as Variable;
            writeVar.RegisterObject(writeVar.ChildCount(), text);
            yield break;
        }
    }
}
