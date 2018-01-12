using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class ElsifNode : BaseNode
    {
        public override string AcceptedTypeNames => "elsif_sentence";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            Return calc = new Return();
            yield return NewPause(Child["elsif_condition"]);
            yield return Child["elsif_condition"].Execute(calc);
            if (calc.Bool)
            {
                yield return Child["elsif_true_part"].Execute();
            }
            me.Return(calc.Bool);
        }
    }
}
