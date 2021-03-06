﻿using System;
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
            string typeName = "";
            if (Child.ContainsKey("var_type"))
            {
                typeName = Child["var_type"].GetCode();
            }
            Return initValue = null; // null means no init_value
            if (Child.ContainsKey("init_value"))
            {
                initValue = new Return();
                yield return Child["init_value"].Execute(initValue);
            }
            foreach (string idName in idNames)
            {
                if(typeName!="")
                {
                    Mediator.Instance.ExecutingNameSpace.RegisterTypeName(idName, typeName);
                    if (!(initValue is null))
                        Mediator.Instance.ExecutingNameSpace.Reassign(idName, initValue.Object);
                }
                else if (!(initValue is null))
                {
                    Mediator.Instance.ExecutingNameSpace.RegisterObject(idName, initValue.Object);
                }
                else
                {
                    throw new Exception("Neither initial value nor type is provided on variable declaration.");
                }
            }
                
            yield break;
        }
    }
}
