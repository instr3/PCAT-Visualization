using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class ForNode : BaseNode
    {
        public override string AcceptedTypeNames => "for";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            string idName = Child["loop_variable"].GetCode();
            int initValue, endValue, stepValue;
            Return init = new Return();
            yield return NewPause(Child["loop_init_value"]);
            yield return Child["loop_init_value"].Execute(init);
            initValue = init.Integer;
            Return end = new Return();
            yield return Child["loop_end_value"].Execute(end);
            endValue = end.Integer;
            if (Child.ContainsKey("loop_step_size"))
            {
                Return step = new Return();
                yield return Child["loop_step_size"].Execute(step);
                stepValue = step.Integer;
            }
            else stepValue = 1;
            Mediator.Instance.ExecutingNameSpace.Reassign(idName, init.Integer);
            yield return NewPause(Child["loop_end_value"]);
            while (ContinueLoop((int)Mediator.Instance.ExecutingNameSpace.Get(idName),
                    endValue, stepValue))
            {
                   yield return Child["loop_statements"].Execute();
                if (Child.ContainsKey("loop_step_size"))
                    yield return NewPause(Child["loop_step_size"]);
                Mediator.Instance.ExecutingNameSpace.Reassign(idName,
                    (int)Mediator.Instance.ExecutingNameSpace.Get(idName) + stepValue);
                yield return NewPause(Child["loop_end_value"]);
            }
        }

        private bool ContinueLoop(int currentValue,int endValue,int step)
        {
            if (step >= 0)
                return currentValue <= endValue;
            else
                return currentValue >= endValue;
        }
    }
}
