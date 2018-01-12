using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context
{
    public class Mediator
    {
        public static Mediator Instance = new Mediator();
        public Dictionary<string, Type> DesignationDict { get; private set; }
        public BaseNode RootNode { get; private set; }
        public Detail DetailForm { get; private set; }
        public List<string> FunctionNames { get; private set; }
        public List<BaseNode> FunctionNodes { get; private set; }
        public string Code { get; internal set; }

  
        public BaseNode ExecutingFunction { get; set; }
        public Variable ExecutingNameSpace { get; set; }

        public ObservableCollection<RenderLayer> VariableRenderer { get; private set; }

        private Mediator()
        {
            DesignationDict = new Dictionary<string, Type>();
            List<BaseNode> reflectedBaseNodes = Reflector.GetEnumerableOfType<BaseNode>();
            foreach(BaseNode reflectedBaseNode in reflectedBaseNodes)
            {
                string[] acceptedTypes = reflectedBaseNode.AcceptedTypeNames.Split('#');
                foreach (string typeName in acceptedTypes)
                {
                    if(DesignationDict.ContainsKey(typeName))
                    {
                        throw new Exception("Duplicated designation: " + typeName);
                    }
                    DesignationDict[typeName] = reflectedBaseNode.GetType();
                }
            }
        }
        public void RegisterGrammarTree(List<RawNode> rawNodes)
        {
            interruptions = null;
            Variable.Root = new Variable();
            BaseNode[] baseNodes = new BaseNode[rawNodes.Count];
            bool hasRoot = false;
            for (int i = 0; i < rawNodes.Count; ++i)
            {
                if (rawNodes[i].TypeName is null)
                    continue;
                Type designatedClass;
                if (DesignationDict.ContainsKey(rawNodes[i].TypeName))
                    designatedClass = DesignationDict[rawNodes[i].TypeName];
                else
                    designatedClass = DesignationDict["*"];
                baseNodes[i] = designatedClass.GetConstructor(new Type[0])
                    .Invoke(new object[0]) as BaseNode;
                baseNodes[i].Init(rawNodes[i], i);
                if (rawNodes[i].FatherLinkType == "root")
                {
                    if (hasRoot)
                    {
                        throw new Exception("Multiple roots");
                    }
                    hasRoot = true;
                    RootNode = baseNodes[i];
                }
            }
            for (int i = 0; i < rawNodes.Count; ++i)
            {
                int fatherID = rawNodes[i].Father;
                if (fatherID != -1)
                {
                    baseNodes[fatherID].LinkChild(baseNodes[i], rawNodes[i].FatherLinkType);
                }
            }
            FunctionNames = new List<string>();
            FunctionNodes = new List<BaseNode>();
            for (int i = 0; i < baseNodes.Length; ++i)
            {
                BaseNode node = baseNodes[i];
                if (node is null)
                    continue;
                if (node.Type == "program")
                {
                    FunctionNames.Add("PROGRAM");
                    FunctionNodes.Add(node);
                }
                else if(node.Type=="procedure_decl")
                {
                    FunctionNames.Add(node.Child["procedure_name"].GetCode());
                    FunctionNodes.Add(node);
                }
            }

            Variable.Root["$OUTPUT$"] = new Variable();
        }
        public void RegisterForm(Detail detailForm)
        {
            DetailForm = detailForm;
            VariableRenderer = new ObservableCollection<RenderLayer>();
            DetailForm.resultView.ItemsSource = VariableRenderer;
        }
        IEnumerator<Interruption> interruptions;
        public void ExecuteOneStep()
        {
            if (interruptions is null)
                interruptions = RootNode.Execute().GetEnumerator();
            if(interruptions.MoveNext())
            {
                Interruption interruption = interruptions.Current;
                DetailForm.ShowRectangles(interruption.Position.ID, false);
                if(interruption.Type!="pause")
                    DetailForm.ShowError(interruption.Type);
            }
            else
            {
                DetailForm.ShowError("Program end!");
            }
            Variable.Root.ToRenderLayer(VariableRenderer);
        }
        public void ExecuteToEnd()
        {
            if (interruptions is null)
                interruptions = RootNode.Execute().GetEnumerator();
            while (interruptions.MoveNext())
            {
                throw new NotImplementedException();
                //Interruption interruption = interruptions.Current;
                //DetailForm.ShowRectangles(interruption.Position.ID, false);
                //DetailForm.ShowError(interruption.Type);
            }
            Variable.Root.ToRenderLayer(VariableRenderer);
        }

        public BaseNode ErrorNode { get; private set; }

        public void SetErrorNode(BaseNode node)
        {
            ErrorNode = node;
        }

    }
}
