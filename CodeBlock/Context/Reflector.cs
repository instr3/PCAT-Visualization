using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
namespace CodeBlock.Context
{
    public static class Reflector
    {
        // This function from https://stackoverflow.com/questions/5411694/get-all-inherited-classes-of-an-abstract-class
        public static List<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                try
                {
                    objects.Add((T)Activator.CreateInstance(type, constructorArgs));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error when constructing class " + type.ToString());
                    throw e;
                }
            }
            return objects;
        }
    }
}
