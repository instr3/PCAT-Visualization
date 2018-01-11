using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context
{
    public class Variable
    {
        public static Variable Root { get; private set; } = new Variable();
        public string TypeName { get; protected set; }
        private Dictionary<object, object> dict;
        public Variable()
        {
            dict = new Dictionary<object, object>();
        }
        public object this[object name]
        {
            get { return dict[name]; }
            set { dict[name] = value; }
        }
        public bool ContainsKey(object key)
        {
            return dict.ContainsKey(key);
        }

        internal void AddVariable(string idName, string typeName, string initValue)
        {
            switch(typeName)
            {
                case "INTEGER":
                    dict[idName] = initValue is null ? 0 : int.Parse(initValue);
                    break;
                case "REAL":
                    dict[idName] = initValue is null ? 0.0f : float.Parse(initValue);
                    break;
                case "BOOLEAN":
                    if (initValue is null)
                        dict[idName] = false;
                    else if (initValue == "TRUE")
                        dict[idName] = true;
                    else if (initValue == "FALSE")
                        dict[idName] = false;
                    else
                        throw new FormatException("Illegal boolean value : " + initValue);
                    break;
                default:
                    throw new NotImplementedException();

            }

        }

        public void Assign(object id, object value)
        {
            if(dict.ContainsKey(id))
            {
                object originValue = dict[id];
                Type originType = originValue.GetType();
                Type valueType = value.GetType();
                if(originType==valueType)
                {
                    dict[id] = value;
                    return;
                }
                else if(dict[id] is float && !(value is int))
                {
                    dict[id] = (float)(int)value;
                    return;
                }
                else
                {
                    throw new Exception("Cannot cast variable of type " + GetPCATStyleTypeName(originValue) + " to " + GetPCATStyleTypeName(value));
                }
            }
            else
            {
                throw new Exception("Not decleared variable: " + id);
            }
            throw new NotImplementedException();
        }
        private string GetPCATStyleTypeName(object obj)
        {
            if (obj is int)
                return "INTEGER";
            else if (obj is float)
                return "REAL";
            else if (obj is Variable)
                return "structure";
            return obj.GetType().ToString();
        }
        private void ToRenderLayerInner(ICollection<RenderLayer> layers, int depth, string suffix)
        {
            foreach (KeyValuePair<object, object> kv in dict)
            {
                if (kv.Value is Variable)
                    (kv.Value as Variable).ToRenderLayerInner(layers, depth + 1, "");
                else
                    layers.Add(new RenderLayer(depth, suffix + kv.Key.ToString() + " = " + kv.Value.ToString()));
            }
        }
        public void ToRenderLayer(ICollection<RenderLayer> layers)
        {
            layers.Clear();
            ToRenderLayerInner(layers, 0, "");
        }
        private string ToStringInner(string suffix)
        {
            string result = "";
            foreach (KeyValuePair<object, object> kv in dict)
            {
                if (kv.Value is Variable)
                    result += (kv.Value as Variable).ToStringInner(suffix + kv.Key + ".");
                else
                    result += suffix + kv.Key.ToString() + " = " + kv.Value.ToString() + Environment.NewLine;
            }
            return result;
        }

        public override string ToString()
        {
            return ToStringInner("");
        }
    }
}
