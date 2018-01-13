using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class ProcedureDeclNode : BaseNode
    {
        public override string AcceptedTypeNames => "procedure_decl";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            Variable functionTemplate = new Variable();
            functionTemplate.TypeName = ChildID[0].GetCode(); //function_name or procedure_name
            string functionName = "@function_" + functionTemplate.TypeName;
            BaseNode parameterList = Child["parameter_list"];
            List<string> paramIndex = new List<string>();
            foreach(BaseNode fpSection in parameterList.ChildID)
            {
                BaseNode parameterNames = fpSection.Child["parameter_name"];
                BaseNode parameterTypes = fpSection.Child["parameter_type"];
                string typeName = parameterTypes.GetCode();
                foreach (BaseNode parameterName in parameterNames.ChildID)
                {
                    string idName = parameterName.GetCode();
                    paramIndex.Add(idName);
                    functionTemplate.RegisterTypeName(idName, typeName);
                }
            }
            if(Child.ContainsKey("return_type"))
            {
                functionTemplate.RegisterTypeName("@return", Child["return_type"].GetCode());
            }
            functionTemplate.RegisterObject("@index", paramIndex.ToArray());
            functionTemplate.RegisterObject("@address", Child["procedure_body"]);
            Mediator.Instance.ExecutingNameSpace.RegisterObject(functionName, functionTemplate);
            yield break;
        }
    }
}
