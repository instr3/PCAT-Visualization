using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class ElementDerefNode : BaseNode
    {
        public override string AcceptedTypeNames => "element_deref";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            BaseNode leftValue = ChildID[0];
            BaseNode rightValue = ChildID[1];
            VariableReference reference = new VariableReference();
            Return rhs = new Return();
            yield return rightValue.Execute(rhs);
            reference.Offset = rhs.Integer;
            if (leftValue.Type == "ID")
            {
                string idName = leftValue.GetCode();
                if (!Mediator.Instance.ExecutingNameSpace.CanFind(idName))
                {
                    throw new Exception("Not declared variable: " + idName);
                }
                reference.Base = Mediator.Instance.ExecutingNameSpace.Get(idName) as Variable;
            }
            else
            {
                Return lhs = new Return();
                yield return leftValue.Execute(lhs);
                reference.Base = lhs.Reference.Dereference() as Variable;
            }
            me.Return(reference);
        }
    }
}
