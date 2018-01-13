using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class ArrayConstructionNode : BaseNode
    {
        public override string AcceptedTypeNames => "array_construction";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            string typeName = ChildID[0].GetCode();
            Variable structure = Mediator.Instance.ExecutingNameSpace.GetPrototypeOf(typeName).Clone() as Variable;
            BaseNode arrayValues = ChildID[1];
            int index = 0;
            foreach (BaseNode arrayValuePair in arrayValues.ChildID)
            {
                int repeatTimes;
                Return lhs = new Return();
                Return rhs = new Return();
                if (arrayValuePair.Type == "array_value_pair")
                {
                    yield return arrayValuePair.ChildID[0].Execute(lhs);
                    repeatTimes = lhs.Integer;
                    yield return arrayValuePair.ChildID[1].Execute(rhs);
                }
                else // arrayValuePair is expression
                {
                    repeatTimes = 1;
                    yield return arrayValuePair.Execute(rhs);
                }
                object elementValue = rhs.Object;
                for (int i = 0; i < repeatTimes; ++i)
                {
                    object elementObject = structure.CurrentLevelGet("@element");
                    structure.RegisterObject(index, elementObject);
                    if (elementValue is ICloneable)
                        elementValue = (elementValue as ICloneable).Clone();
                    structure.Reassign(index, elementValue);
                    ++index;
                }
            }
            me.Return(structure);
            yield break;
        }
    }
}
