using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class RecordConstructionNode : BaseNode
    {
        public override string AcceptedTypeNames => "record_construction";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            string typeName = ChildID[0].GetCode();
            Variable structure = Mediator.Instance.ExecutingNameSpace.GetPrototypeOf(typeName).Clone() as Variable;
            BaseNode compValues=ChildID[1];
            foreach(BaseNode compValuePair in compValues.ChildID)
            {
                string idName = compValuePair.ChildID[0].GetCode();
                Return rhs = new Return();
                yield return compValuePair.ChildID[1].Execute(rhs);
                structure.Reassign(idName, rhs.Object);
            }
            me.Return(structure);
            yield break;
        }
    }
}
