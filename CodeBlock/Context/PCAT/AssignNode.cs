using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class AssignNode : BaseNode
    {
        public override string AcceptedTypeNames => "assign";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            // Return lhs = new Return();
            Return rhs = new Return();
            yield return Child["r_value"].Execute(rhs);
            // yield return Child["l_value"].Execute(lhs);
            if (Child["l_value"].Type=="ID")
            {
                string idName = Child["l_value"].GetCode();
                if(!Mediator.Instance.ExecutingNameSpace.ContainsKey(idName))
                {
                    throw new Exception("Not decleared variable: " + idName);
                }
                Mediator.Instance.ExecutingNameSpace.Reassign(idName, rhs.Object);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
