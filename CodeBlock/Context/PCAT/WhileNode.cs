﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class WhileNode : BaseNode
    {
        public override string AcceptedTypeNames => "while";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            Return calc = new Return();
            yield return NewPause(Child["while_condition"]);
            while(true)
            {
                yield return Child["while_condition"].Execute(calc);
                if (!calc.Bool)
                    break;
                Child["loop_statements"].Execute();
            }
        }
    }
}
