using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ConsoleSamples
{
    class AssemblyDumper
    {
        private string _filePath;
        public AssemblyDumper(string filePath)
        {
            _filePath = filePath;
        }

        public void Dump(TextWriter textWriter)
        {
            Assembly asm = Assembly.LoadFile(_filePath);

            textWriter.WriteLine(asm.FullName);
            foreach (var type in asm.GetTypes())
            {
                textWriter.WriteLine(type.FullName);
                DumpMethods(textWriter, type, "\t");
            }
        }

        private void DumpMethods(TextWriter textWriter, Type type, string indent)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            foreach (MethodInfo methodInfo in type.GetMethods(bindingFlags))
            {
                string methodParams = String.Join(", ", methodInfo.GetParameters().Select(p => p.ToString()));

                textWriter.WriteLine("{0}{1}({2})", indent, methodInfo.Name, methodParams);
            }
        }
    }
}
