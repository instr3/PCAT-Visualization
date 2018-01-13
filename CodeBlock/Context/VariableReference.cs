using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context
{
    public class VariableReference
    {
        public Variable Base { get; private set; }
        public object Offset { get; private set; }
        public object Dereference()
        {
            return Base.Get(Offset);
        }
        public void Assign(object value)
        {
            Base.Reassign(Offset, value);
        }
    }
}
