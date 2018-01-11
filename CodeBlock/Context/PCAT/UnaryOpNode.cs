using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class UnaryOpNode : BaseNode
    {
        public override string AcceptedTypeNames => "brackets#positive#negative#logical_not";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            Return lhs = new Return();
            yield return ChildID[0].Execute(lhs);
            switch (Type)
            {
                case "brackets":
                    me.Return(lhs.Object);
                    break;
                case "positive":
                    if(lhs.IsReal())
                        me.Return(lhs.Real);
                    else if(lhs.IsInteger())
                        me.Return(lhs.Integer);
                    break;
                case "negative":
                    if (lhs.IsReal())
                        me.Return(-lhs.Real);
                    else if (lhs.IsInteger())
                        me.Return(-lhs.Integer);
                    break;
                case "logical_not":
                    me.Return(!lhs.Bool);
                    break;
            }
        }
    }
}
