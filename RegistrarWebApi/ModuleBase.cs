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
using Newtonsoft.Json;

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
                Get[$"{method.Name.ToLower()}/{{argument}}"] = args => CallGetMethod(method, args.argument.ToString());
            }
        }

        private object CallGetMethod(MethodInfo method, string argument)
        {
            var argType = method.GetParameters()[0].ParameterType;
            var argValue = JsonConvert.DeserializeObject(argument, argType);
            return JsonConvert.SerializeObject(method.Invoke(this, new object[] {argValue}));
        }

        private IEnumerable<MethodInfo> GetAllMethods()
        {
            var methods = GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            if (methods.Any(x => x.GetParameters().Length != 1))
            {
                throw new InvalidOperationException("WebApi methods must have exactly one argument.");
            }
            return methods;
        }
    }
}
