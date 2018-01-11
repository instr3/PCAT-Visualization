using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context
{
    public class Interruption
    {
        public BaseNode Position { get; protected set; }
        public string Type { get; protected set; }
        public Interruption(BaseNode inputPosition)
        {
            Position = inputPosition;
        }
        public Interruption(string inputType, BaseNode inputPosition)
        {
            Position = inputPosition;
            Type = inputType;
        }
    }
}
