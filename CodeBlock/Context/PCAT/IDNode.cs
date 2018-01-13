using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context.PCAT
{
    class IDNode : BaseNode
    {
        public override string AcceptedTypeNames => "ID";

        protected override IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me)
        {
            string idName = GetCode();
            if (!Mediator.Instance.ExecutingNameSpace.CanFind(idName))
            {
                throw new Exception("Not decleared variable: " + idName);
            }
            object returnObject = Mediator.Instance.ExecutingNameSpace.Get(idName);
            // Need clone here!!
            if(returnObject is ICloneable)
            {
                returnObject = (returnObject as ICloneable).Clone();
            }
            me.Return(returnObject);
            yield break;
        }
    }
}
