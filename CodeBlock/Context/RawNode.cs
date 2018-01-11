using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context
{
    public class RawNode
    {
        public int Father { get; set; }
        public string FatherLinkType { get; set; }
        public int LineNumber { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int OneLineLeft { get; set; }
        public int OneLineRight { get; set; }
        public int TypeMacro { get; set; }
        public string TypeName { get; set; }
        public int ChildrenCount { get; set; }
    };
}
