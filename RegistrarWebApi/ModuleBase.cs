using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Julas.Utils;
using Nancy;

namespace RegistrarWebApi
{
    public abstract class ModuleBase : NancyModule
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetModuleName()
        {
            var name = new StackFrame(2).GetMethod().DeclaringType.Name.ToLower();
            return name;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected ModuleBase() : base(GetModuleName())
        {
            foreach (var method in GetAllMethods())
            {
                Get[method.Name.ToLower()] = _ => CallGetMethod(method);
            }
        }

        private object CallGetMethod(MethodInfo method)
        {
            var requestArgs = (IDictionary<string, object>)Request.Query;
            var reflectionArgs = method.GetParameters();
            var methodArgs = new object[reflectionArgs.Length];
            foreach (var methodArg in reflectionArgs.Select((x, i) => new { Index = i, Value = x }))
            {
                var name = methodArg.Value.Name.ToLower();
                object value = null;
                if (requestArgs.Keys.Any(k => k.ToLower() == name))
                {
                    var requestArgValue = requestArgs.First(kvp => kvp.Key.ToLower() == name).Value.ToString();
                    value = requestArgValue.ConvertTo(methodArg.Value.ParameterType);
                }
                else
                {
                    value = methodArg.Value.ParameterType.IsValueType
                        ? Activator.CreateInstance(methodArg.Value.ParameterType)
                        : null;
                }
                methodArgs[methodArg.Index] = value;
            }
            return method.Invoke(this, methodArgs);
        }

        private IEnumerable<MethodInfo> GetAllMethods()
        {
            return GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        }
    }
}
