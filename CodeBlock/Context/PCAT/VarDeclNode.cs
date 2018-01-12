using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class VarDeclNode : BaseNode
    {
        public override string AcceptedTypeNames => "var_decl";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            BaseNode ids = Child["var_names"];
            string[] idNames = ids.ChildID.Select(node => node.GetCode()).ToArray();
            string typeName = Child["var_type"].GetCode();
            string initValue = null;
            if (Child.ContainsKey("init_value"))
                initValue = Child["init_value"].GetCode();
            foreach (string idName in idNames)
                Mediator.Instance.ExecutingNameSpace.AddVariable(idName, typeName, initValue);
            yield break;
        }
    }
}
