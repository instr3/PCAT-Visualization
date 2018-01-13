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
            boolean=4,
            structure=8,
            reference=1024
        }
        private ReturnTypeEnum type;
        private object storeReference;
        private object value;
        public Return()
        {
            storeReference = null;
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
            return (type & ReturnTypeEnum.boolean) != 0;
        }
        public float Real
        {
            get
            {
                if (IsReal())
                    return (float)value;
                if (IsInteger())
                    return (float)(int)value;
                throw new Exception("Type error: excepted real, got " + type);
                
            }
        }
        public bool IsReal()
        {
            return (type & ReturnTypeEnum.real) != 0;
        }
        public int Integer
        {
            get
            {
                if (!IsInteger())
                    throw new Exception("Type error: excepted integer, got " + type);
                return (int)value;
            }
        }
        public bool IsInteger()
        {
            return (type & ReturnTypeEnum.integer) != 0;
        }
        public bool IsNumber()
        {
            return (type & (ReturnTypeEnum.integer | ReturnTypeEnum.real)) !=0;
        }
        public bool IsStructure()
        {
            return (type & ReturnTypeEnum.structure) != 0;
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
                return storeReference as VariableReference;
            }
        }
        public object Object { get => value; }

        private void SetValue(object inputObject)
        {
            if (inputObject is VariableReference)
            {
                storeReference = inputObject;
                value = ((VariableReference)storeReference).Dereference();
                // Need clone here, too!!
                if (value is ICloneable)
                {
                    value = (value as ICloneable).Clone();
                }
            }
            else
            {
                value = inputObject;
            }
            if (value is null)
                type = ReturnTypeEnum.none;
            else if (value is int)
                type = ReturnTypeEnum.integer;
            else if (value is float)
                type = ReturnTypeEnum.real;
            else if (value is bool)
                type = ReturnTypeEnum.boolean;
            else if (value is Variable)
                type = ReturnTypeEnum.structure;
            else if (inputObject is VariableReference)
                type |= ReturnTypeEnum.reference;
            else
                throw new NotSupportedException("Unsupported type as return:" + inputObject.GetType().ToString());

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
