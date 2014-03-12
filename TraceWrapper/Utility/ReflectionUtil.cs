using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TraceWrapper
{
    public abstract class ReflectionUtil
    {
        public static Type CreateType(string instanceSetting)
        {
            if (!String.IsNullOrWhiteSpace(instanceSetting))
            {
                try
                {
                  
                    string assemblyName;
                    string typeinit;
                    SpiltBy(instanceSetting, ',', out typeinit, out assemblyName);
                    if (assemblyName.EndsWith(")"))
                    {
                        typeinit = instanceSetting;
                        assemblyName = "";
                    }
                    Assembly assembly = null;
                    if (assemblyName != string.Empty) assembly = Assembly.LoadFrom(assemblyName);

                    if (!typeinit.Contains("("))
                    {
                       return  GetType(assembly, typeinit);
                    }
                    else
                    {
                        string param;
                        string type;
                        SpiltBy(typeinit, '(', out type, out param, false);
                        param = param.Remove(param.Length - 1);//remove last )
                        return GetType(assembly, type);
                        
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("Error creating {0} Exception{1}", instanceSetting, ex.Message));
                    return null;
                }
            }
            return null;
        }
        public static T CreateInstance<T>(string instanceSetting)
        {
            if (!String.IsNullOrWhiteSpace(instanceSetting))
            {
                try
                {
                    T handler = default(T);
                    string assemblyName;
                    string typeinit;
                    SpiltBy(instanceSetting, ',', out typeinit, out assemblyName);
                    if (assemblyName.EndsWith(")"))
                    {
                        typeinit = instanceSetting;
                        assemblyName = "";
                    }
                    Assembly assembly = null;
                    if (assemblyName != string.Empty) assembly = Assembly.LoadFrom(assemblyName);

                    if (!typeinit.Contains("("))
                    {
                        handler = (T)Activator.CreateInstance(GetType(assembly, typeinit));
                    }
                    else
                    {
                        string param;
                        string type;
                        SpiltBy(typeinit, '(', out type, out param, false);
                        param = param.Remove(param.Length - 1);//remove last )
                        Type handlertype = GetType(assembly, type);
                        if (handlertype == null) return default(T);
                        handler = (T)TryParse(handlertype, param);

                    }


                    return handler;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("Error creating {0} Exception{1}",instanceSetting, ex.Message));

                }
            }
            return default(T);
        }
        private static Type GetType(Assembly assembly, string type)
        {
            if (assembly != null)
            {
                return assembly.GetType(type);
            }
            return Type.GetType(type);
        }
        private static bool SpiltBy(string source, char splitchar, out string left, out string right, bool bylast = true)
        {
            int splitidx;
            if (bylast)
            {
                splitidx = source.LastIndexOf(splitchar);
            }
            else
            {
                splitidx = source.IndexOf(splitchar);
            }
            if (splitidx < 0)
            {
                left = source;
                right = string.Empty;
                return false;
            }

            left = source.Substring(0, splitidx);
            right = source.Remove(0, splitidx + 1);
            return true;
        }
        /// <summary>
        /// simple parse
        /// </summary>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private static object TryParse(Type type, string param)
        {
            string[] paramlist = param.Split(',');
            foreach (ConstructorInfo ci in type.GetConstructors())
            {
                ParameterInfo[] pilist = ci.GetParameters();
                object[] paramobjectlist = new object[paramlist.Length];
                if (pilist.Length == paramlist.Length)
                {
                    bool valid = true;
                    try
                    {
                        for (int idx = 0; idx < pilist.Length; idx++)
                        {
                            if (pilist[idx].ParameterType == typeof(string))
                            {
                                if (paramlist[idx].Trim().StartsWith("\"") &&
                                    paramlist[idx].Trim().EndsWith("\""))
                                {
                                    paramobjectlist[idx] = paramlist[idx].Trim().Substring(1, paramlist[idx].Trim().Length - 2);
                                }
                                else
                                {
                                    paramobjectlist[idx] = paramlist[idx].Trim();
                                }
                            }
                            else
                            {
                                paramobjectlist[idx] = pilist[idx].ParameterType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(null, new object[] { paramlist[idx].Trim() });
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine("TryParse:" + ex.Message);
                        valid = false;

                    }
                    if (valid)
                    {
                        return ci.Invoke(paramobjectlist);
                    }
                }
            }
            return null;

        }
    }
}
