using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlock.Context
{
    // Return is a short name of ReturnGetter
    public class Return
    {
        private enum ReturnTypeEnum
        {
            none=0,
            integer=1,
            real=2,
            boolean=3,
            structure=4,
            reference=-1
        }
        private ReturnTypeEnum type;
        private object storeObject;
        private object value
        {
            get
            {
                if (type == ReturnTypeEnum.reference)
                    return ((VariableReference)storeObject).Dereference();
                else
                    return storeObject;
            }
        }
        public Return()
        {
            storeObject = null;
            type = ReturnTypeEnum.none;
        }

        public bool Bool
        {
            get
            {
                return (bool)value;
            }
        }
        public bool IsBool()
        {
            return type == ReturnTypeEnum.boolean;
        }
        public float Real
        {
            get
            {
                if(type==ReturnTypeEnum.real)
                    return (float)value;
                if (type == ReturnTypeEnum.integer)
                    return (float)(int)value;
                throw new Exception("Type error: excepted real, got " + type);
                
            }
        }
        public bool IsReal()
        {
            return type == ReturnTypeEnum.real;
        }
        public int Integer
        {
            get
            {
                if (type != ReturnTypeEnum.integer)
                    throw new Exception("Type error: excepted integer, got " + type);
                return (int)value;
            }
        }
        public bool IsInteger()
        {
            return type == ReturnTypeEnum.integer;
        }
        public bool IsNumber()
        {
            return type == ReturnTypeEnum.integer || type == ReturnTypeEnum.real;
        }
        public bool IsStructure()
        {
            return type == ReturnTypeEnum.structure;
        }
        public Variable Structure
        {
            get
            {
                return value as Variable;
            }
        }
        public VariableReference Reference
        {
            get
            {
                return storeObject as VariableReference;
            }
        }
        public object Object { get => value; }

        private void SetValue(object inputValue)
        {
            storeObject = inputValue;
            if (storeObject == null)
                type = ReturnTypeEnum.none;
            else if (storeObject is int)
                type = ReturnTypeEnum.integer;
            else if (storeObject is float)
                type = ReturnTypeEnum.real;
            else if (storeObject is bool)
                type = ReturnTypeEnum.boolean;
            else if (storeObject is Variable)
                type = ReturnTypeEnum.structure;
            else if (storeObject is VariableReference)
                type = ReturnTypeEnum.reference;
            else
                throw new NotSupportedException("Unsupported type as return:" + inputValue.GetType().ToString());

        }
        public class ReturnSetter
        {
            private object value;
            public ReturnSetter()
            {
                value = null;
            }
            public void Return(object inputValue)
            {
                value = inputValue;
            }
            public void SetReturnGetter(Return returnGetter)
            {
                returnGetter.SetValue(value);
            }
        }

    }
}
