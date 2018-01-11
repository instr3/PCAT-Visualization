using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class BinaryOpNode : BaseNode
    {
        public override string AcceptedTypeNames => "logical_or#logical_and#" +
            "less_than#greater_than#less_equal#greater_equal#equal#not_equal#" +
            "add#subtract#multiply#divide#div#mod";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            Return lhs = new Return();
            Return rhs = new Return();
            switch (Type)
            {
                case "logical_or": // short-circuit
                    yield return ChildID[0].Execute(lhs);
                    if (lhs.Bool == true)
                    {
                        me.Return(true);
                    }
                    else
                    {
                        yield return ChildID[1].Execute(rhs);
                        me.Return(rhs.Bool);
                    }
                    break;
                case "logical_and":
                    yield return ChildID[0].Execute(lhs);
                    if (lhs.Bool == false)
                    {
                        me.Return(false);
                    }
                    else
                    {
                        yield return ChildID[1].Execute(rhs);
                        me.Return(rhs.Bool);
                    }
                    break;
                default:
                    break;
            }
            yield return ChildID[0].Execute(lhs);
            yield return ChildID[1].Execute(rhs);
            switch (Type)
            {
                case "add":
                    if (lhs.IsReal() || rhs.IsReal())
                        me.Return(lhs.Real + rhs.Real);
                    else
                        me.Return(lhs.Integer + rhs.Integer);
                    break;
                case "subtract":
                    if (lhs.IsReal() || rhs.IsReal())
                        me.Return(lhs.Real - rhs.Real);
                    else
                        me.Return(lhs.Integer - rhs.Integer);
                    break;
                case "multiply":
                    if (lhs.IsReal() || rhs.IsReal())
                        me.Return(lhs.Real * rhs.Real);
                    else
                        me.Return(lhs.Integer * rhs.Integer);
                    break;
                case "divide":
                    me.Return(lhs.Real / rhs.Real);
                    break;
                case "mod":
                    me.Return(lhs.Integer % rhs.Integer);
                    break;
                case "div":
                    me.Return(lhs.Integer / rhs.Integer);
                    break;
                case "less_than":
                    if (lhs.IsReal() || rhs.IsReal())
                        me.Return(lhs.Real < rhs.Real);
                    else
                        me.Return(lhs.Integer < rhs.Integer);
                    break;
                case "greater_than":
                    if (lhs.IsReal() || rhs.IsReal())
                        me.Return(lhs.Real > rhs.Real);
                    else
                        me.Return(lhs.Integer > rhs.Integer);
                    break;
                case "less_equal":
                    if (lhs.IsReal() || rhs.IsReal())
                        me.Return(lhs.Real <= rhs.Real);
                    else
                        me.Return(lhs.Integer <= rhs.Integer);
                    break;
                case "greater_equal":
                    if (lhs.IsReal() || rhs.IsReal())
                        me.Return(lhs.Real >= rhs.Real);
                    else
                        me.Return(lhs.Integer >= rhs.Integer);
                    break;
                case "equal":
                    if (lhs.IsReal() || rhs.IsReal())
                        me.Return(lhs.Real == rhs.Real);
                    else if (lhs.IsInteger() && rhs.IsInteger())
                        me.Return(lhs.Integer == rhs.Integer);
                    else if (lhs.IsBool() && rhs.IsBool())
                        me.Return(lhs.Bool == rhs.Bool);
                    else
                        me.Return(lhs.Structure == rhs.Structure);
                    break;
                case "not_equal":
                    if (lhs.IsReal() || rhs.IsReal())
                        me.Return(lhs.Real != rhs.Real);
                    else if (lhs.IsInteger() && rhs.IsInteger())
                        me.Return(lhs.Integer != rhs.Integer);
                    else if (lhs.IsBool() && rhs.IsBool())
                        me.Return(lhs.Bool != rhs.Bool);
                    else
                        me.Return(lhs.Structure != rhs.Structure);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
