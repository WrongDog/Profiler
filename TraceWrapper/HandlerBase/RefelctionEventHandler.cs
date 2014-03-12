using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;

namespace TraceWrapper
{
    public enum CompileLanguage
    {
        CSharp,
        VisualBasic
    }
    public class Compiler
    {



        public static Assembly Compile(string sourcecode, CompileLanguage language = CompileLanguage.CSharp)
        {
            List<string> referencedassemblies = new List<string>();
            referencedassemblies.Add("system.dll");
            referencedassemblies.Add("system.core.dll");
            referencedassemblies.Add("Microsoft.CSharp.dll");
            referencedassemblies.Add("system.xml.dll");
            referencedassemblies.Add("system.xml.linq.dll");
            referencedassemblies.Add("system.data.dll");
            referencedassemblies.Add("System.Data.DataSetExtensions.dll");

            return Compile(sourcecode, referencedassemblies.ToArray(), language);
        }

        public static Assembly Compile(string sourcecode, string[] references, CompileLanguage language = CompileLanguage.CSharp)
        {
            CodeDomProvider comp = null;
            switch (language)
            {
                case CompileLanguage.VisualBasic:
                    comp = new Microsoft.VisualBasic.VBCodeProvider();
                    break;
                case CompileLanguage.CSharp:
                default:
                    comp = new Microsoft.CSharp.CSharpCodeProvider();
                    break;
            }
            CompilerParameters cp = new CompilerParameters();
            foreach (string reference in references)
            {
                cp.ReferencedAssemblies.Add(reference);
            }
            cp.GenerateInMemory = true;



            CompilerResults cr = comp.CompileAssemblyFromSource(cp, sourcecode);
            if (cr.Errors.HasErrors)
            {
                string error = string.Empty;
                foreach (CompilerError err in cr.Errors)
                {
                    error += err.ErrorText + System.Environment.NewLine;
                }
                System.Diagnostics.Trace.WriteLine(error);
                return null;
            }

            return cr.CompiledAssembly;
        }

    }
    public class RefelctionEventHandler:IResultHandler
    {
        public event ResultEventHandler OnResultChange;
        protected TraceDefinitionFile traceDefinition= new TraceDefinitionFile();
        protected MethodInfo mi;
        public RefelctionEventHandler(string traceDefinitionXmlFile, string onNewTraceEventSouceFile)
        {
            traceDefinition.ReadFromXml(traceDefinitionXmlFile);
            string source = "using System;" +
                          "using System.Collections.Generic;" +
                          "using System.Linq;" +
                          "using System.Text;" +
                          "using System.Data;" +
                          "using System.IO;" +
                          "using TraceWrapper;"+
                          "    public class OnNewTraceEventHandler" +
                          "    {" +
                          "        public static void Handle(System.Data.DataRow traceEvent,Action<TraceWrapper.Result> updateAction)" +
                          "        {" +
                          "            {0}" +
                          "        }" +
                          "    }";
            using (StreamReader reader = new StreamReader(onNewTraceEventSouceFile))
            {
                source=source.Replace("{0}", reader.ReadToEnd());
                reader.Close();
            }
            List<string> referencedassemblies = new List<string>();
            referencedassemblies.Add("system.dll");
            referencedassemblies.Add("system.core.dll");
            referencedassemblies.Add("Microsoft.CSharp.dll");
            referencedassemblies.Add("system.xml.dll");
            referencedassemblies.Add("system.xml.linq.dll");
            referencedassemblies.Add("system.data.dll");
            referencedassemblies.Add("System.Data.DataSetExtensions.dll");
            referencedassemblies.Add("TraceWrapper.dll");

            mi = Compiler.Compile(source,referencedassemblies.ToArray()).GetType("OnNewTraceEventHandler").GetMethod("Handle");
        }
        protected void UpdateResult(Result result)
        {
            this.OnResultChange(this.GetType().Name, result);
        }
        public void OnNewTraceEvent(TraceEvent traceEvent)
        {
            mi.Invoke(null, new object[] { traceEvent, new Action<Result>(this.UpdateResult) });
        }

        public TraceDefinitionFile TraceDefinition
        {
            get { return traceDefinition; }
        }
        public string Name
        {
            get;
            set;
        }
        public void SaveResult( IResultSaveAdapter resultSaveAdapter)
        {
            //nothing
        }
    }
}
