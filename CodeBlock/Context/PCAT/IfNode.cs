using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBlock.Context;

namespace CodeBlock.Context.PCAT
{
    class IfNode : BaseNode
    {
        public override string AcceptedTypeNames => "if#if_else";
        
        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            Return calc = new Return();
            yield return NewPause(Child["if_condition"]);
            yield return Child["if_condition"].Execute(calc);
            if(calc.Bool)
            {
                yield return Child["if_true_part"].Execute();
            }
            else
            {
                if(Child.ContainsKey("elsif_part"))
                    yield return Child["elsif_part"].Execute(calc);
                if(!calc.Bool && Type == "if_else")
                    yield return Child["else_part"].Execute(calc);
            }
        }
    }
}
