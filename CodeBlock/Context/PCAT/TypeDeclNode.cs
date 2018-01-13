using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class TypeDeclNode : BaseNode
    {
        public override string AcceptedTypeNames => "type_decl";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            string structureName = ChildID[0].GetCode();
            BaseNode innerTypeNode = ChildID[1];
            Variable prototypeVariable = new Variable();
            prototypeVariable.TypeName = structureName;
            if (innerTypeNode.Type == "record_components")
            {
                BaseNode componentList = innerTypeNode.ChildID[0];
                Mediator.Instance.ExecutingNameSpace.RegisterObject("@prototype_" + structureName, prototypeVariable);
                foreach (BaseNode component in componentList.ChildID)
                {
                    string componentName = component.ChildID[0].GetCode();
                    string componentType = component.ChildID[1].GetCode();
                    prototypeVariable.RegisterTypeName(componentName, componentType);
                }
            }
            else //innerTypeNode.Type == "array_type"
            {
                string elementTypeName = innerTypeNode.ChildID[0].GetCode();
                Mediator.Instance.ExecutingNameSpace.RegisterObject("@prototype_" + structureName, prototypeVariable);
                prototypeVariable.RegisterTypeName("@element", elementTypeName);
            }
            yield break;
        }
    }
}
