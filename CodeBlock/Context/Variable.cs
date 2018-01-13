using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context
{
    public class Variable : ICloneable
    {
        public static Variable Root { get; set; }
        public string TypeName { get; set; }
        private Dictionary<object, object> dict;
        public Variable Parent;
        public Variable()
        {
            dict = new Dictionary<object, object>();
        }
        /*public object this[object name]
        {
            get { return dict[name]; }
            set { dict[name] = value; }
        }*/
        public bool CanFind(object key)
        {
            if (dict.ContainsKey(key))
                return true;
            else if (Parent is null)
                return false;
            else return Parent.CanFind(key);
        }

        public bool CurrentLevelContainsKey(object key)
        {
            return dict.ContainsKey(key);
        }

        public object CurrentLevelGet(object key)
        {
            return dict[key];
        }

        public void CurrentLevelEraseKey(object key)
        {
            dict.Remove(key);
        }

        public object Get(object id)
        {
            if (dict.ContainsKey(id))
                return dict[id];
            else if (Parent is null)
                throw new Exception("Cannot find object with name " + id);
            else return Parent.Get(id);
        }

        public Variable GetPrototypeOf(string structureName)
        {
            string prototypeName = "@prototype_" + structureName;
            return Get(prototypeName) as Variable;

        }
        public int ChildCount()
        {
            return dict.Count();
        }
        public void RegisterObject(object id,object obj)
        {
            if(obj is Variable)
            {
                dict[id] = obj;
                (obj as Variable).Parent = this;
            }
            else
            {
                dict[id] = obj;
            }
        }
        public void RegisterTypeName(object id, string typeName)
        {
            switch(typeName)
            {
                case "INTEGER":
                    dict[id] = 0;
                    break;
                case "REAL":
                    dict[id] = 0.0f;
                    break;
                case "BOOLEAN":
                    dict[id] = false;
                    break;
                default:
                    Variable prototypeVariable = GetPrototypeOf(typeName);
                    RegisterObject(id, prototypeVariable.Clone());
                    break;
            }

        }

        public void Reassign(object id, object value)
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
                else if(dict[id] is float && value is int)
                {
                    dict[id] = (float)(int)value;
                    return;
                }
                else if(dict[id] is float && value is string)
                {
                    dict[id] = float.Parse(value.ToString());
                    return;
                }
                else if (dict[id] is int && value is string)
                {
                    dict[id] = int.Parse(value.ToString());
                    return;
                }
                else
                {
                    throw new Exception("Cannot cast variable of type " + GetPCATStyleTypeName(originValue) + " to " + GetPCATStyleTypeName(value));
                }
            }
            else if(Parent is null)
            {
                throw new Exception("Not decleared variable: " + id);
            }
            Parent.Reassign(id, value);
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
        private void ToRenderLayerInner(ICollection<RenderLayer> layers, int depth, object suffix)
        {

            string str = suffix.ToString();
            if (!string.IsNullOrEmpty(TypeName))
                str += " : " + TypeName;
            layers.Add(new RenderLayer(depth, str));
            foreach (KeyValuePair<object, object> kv in dict)
            {
                if (kv.Value is Variable)
                    (kv.Value as Variable).ToRenderLayerInner(layers, depth + 1, kv.Key);
                else
                {
                    char connector = kv.Key is int ? ':' : '=';
                    layers.Add(new RenderLayer(depth + 1, kv.Key.ToString() + " "+ connector + " " + kv.Value.ToString().Replace("\n", "\\n")));
                }
                    
            }
        }
        public void ToRenderLayer(ICollection<RenderLayer> layers)
        {
            layers.Clear();
            ToRenderLayerInner(layers, 0, "root");
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

        public object Clone()
        {
            Variable clone = new Variable();
            clone.TypeName = TypeName.Clone() as string;
            clone.Parent = null;
            clone.dict = new Dictionary<object, object>();
            foreach (KeyValuePair<object,object> kv in dict)
            {
                object key = kv.Key is ICloneable ? (kv.Key as ICloneable).Clone() : kv.Key;
                object value = kv.Value is ICloneable ? (kv.Value as ICloneable).Clone() : kv.Value;
                clone.dict.Add(key, value);
            }
            return clone;

        }
    }
}
