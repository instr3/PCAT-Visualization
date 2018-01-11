using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context
{
    public abstract class BaseNode
    {
        public abstract string AcceptedTypeNames { get; }
        public RawNode RawNode { get; private set; }
        public int ID { get; private set; }
        public int Depth { get; private set; }
        public string Type { get; private set; }
        public BaseNode[] ChildID { get; private set; }
        public string[] LinkName { get; private set; }
        public BaseNode Father { get; private set; }
        public string FatherLinkName { get; private set; }
        public Dictionary<string, BaseNode> Child { get; private set; }

        private string[] acceptedTypes;
        private int assignedChildCount;

        public BaseNode()
        {
            acceptedTypes = AcceptedTypeNames.Split('#');
        }

        public void Init(RawNode rawNode, int id)
        {
            string inputType = rawNode.TypeName;
            if (!acceptedTypes.Contains(inputType) && !acceptedTypes.Contains("*"))
            {
                throw new ArgumentException("Error node type: excepted " + AcceptedTypeNames + ", got " + inputType);
            }
            Type = inputType;
            RawNode = rawNode;
            ID = id;
            int childCount = rawNode.ChildrenCount;
            
            ChildID = new BaseNode[childCount];
            LinkName = new string[childCount];
            Child = new Dictionary<string, BaseNode>();
            assignedChildCount = 0;
        }
        // Link from top to bottom
        public void LinkChild(BaseNode node, string linkName)
        {
            ChildID[assignedChildCount] = node;
            LinkName[assignedChildCount] = linkName;
            if (!string.IsNullOrEmpty(linkName))
            {
                Child[linkName] = node;
            }
            assignedChildCount++;
            node.Father = this;
            node.FatherLinkName = linkName;
            node.Depth = Depth + 1;
        }
        // When you need a return value
        public IEnumerable<Interruption> Execute(Return returnGetter)
        {
            Return.ReturnSetter returnSetter = new Return.ReturnSetter();
            foreach (IEnumerable<Interruption> col in InnerExecute(returnSetter))
            {
                foreach(Interruption ir in col)
                {
                    yield return ir;
                }
            }
            returnSetter.SetReturnGetter(returnGetter);
            yield break;
        }

        // When you do not need a return value
        public IEnumerable<Interruption> Execute()
        {
            Return.ReturnSetter returnSetter = new Return.ReturnSetter();
            foreach (IEnumerable<Interruption> col in InnerExecute(returnSetter))
            {
                foreach (Interruption ir in col)
                {
                    yield return ir;
                }
            }
            yield break;
        }

        public IEnumerable<Interruption> NewPause(BaseNode nextNode)
        {
            yield return new Interruption("pause", nextNode);
            yield break;
        }
        public IEnumerable<Interruption> NewInterrupt(string type, BaseNode nextNode)
        {
            yield return new Interruption(type, nextNode);
            yield break;
        }
        public string GetCode()
        {
            return Mediator.Instance.Code.Substring(RawNode.OneLineLeft, RawNode.OneLineRight - RawNode.OneLineLeft);
        }

        abstract protected IEnumerable<IEnumerable<Interruption>> InnerExecute(Return.ReturnSetter me);
    }
}
