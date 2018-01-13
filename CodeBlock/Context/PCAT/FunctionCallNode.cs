using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class FunctionCallNode : BaseNode
    {
        public override string AcceptedTypeNames => "function_call#procedure_call";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            // Step1: prepare new frame
            Variable oldNameSpace = Mediator.Instance.ExecutingNameSpace;
            string functionName = ChildID[0].GetCode();
            Variable templateStructure = Mediator.Instance.ExecutingNameSpace.Get("@function_" + functionName) as Variable;
            Variable newNameSpaceParent = templateStructure.Parent;
            Variable callStructure = templateStructure.Clone() as Variable;
            BaseNode actualParams = ChildID[1];
            string[] paramIndex = callStructure.CurrentLevelGet("@index") as string[];
            int id = 0;
            foreach (BaseNode actualParam in actualParams.ChildID)
            {
                Return rhs = new Return();
                yield return actualParam.Execute(rhs);
                callStructure.Reassign(paramIndex[id++], rhs.Object);
            }

            // Step2: stash old frame, change name space
            Variable newNameSpace = callStructure;
            Variable stashed = null;
            if (newNameSpaceParent.CurrentLevelContainsKey(functionName))
            {
                // Stash the old stuff
                stashed = newNameSpaceParent.CurrentLevelGet(functionName) as Variable;
                newNameSpaceParent.CurrentLevelEraseKey(functionName);
            }
            newNameSpaceParent.RegisterObject(functionName, callStructure);
            Mediator.Instance.ExecutingNameSpace = callStructure;

            // Step3: Execute the procedure
            BaseNode callNode = callStructure.CurrentLevelGet("@address") as BaseNode;
            yield return callNode.Execute();
            object returnValue = Type == "function_call" ?
                callStructure.CurrentLevelGet("@return") : null;

            // Step4: Clean, recover the old frame and old name space
            newNameSpaceParent.CurrentLevelEraseKey(functionName);
            if (!(stashed is null))
            {
                newNameSpaceParent.RegisterObject(functionName, stashed);
            }
            Mediator.Instance.ExecutingNameSpace = oldNameSpace;

            if(Type == "function_call")
                me.Return(returnValue);
            yield break;
        }
    }
}
