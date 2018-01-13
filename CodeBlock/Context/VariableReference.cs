using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context
{
    public class VariableReference
    {
        public Variable Base { get; set; }
        public object Offset { get; set; }
        public object Dereference()
        {
            return Base.Get(Offset);
        }
    }
}
