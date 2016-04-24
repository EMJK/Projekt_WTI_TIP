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
using Newtonsoft.Json.Serialization;

namespace RegistrarWebApi
{
    public abstract class ModuleBase : NancyModule
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        };    

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetModuleName()
        {
            var name = new StackFrame(2).GetMethod().DeclaringType.Name.ToLower();
            return name;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected ModuleBase() : base($"{GetModuleName()}")
        {
            foreach (var method in GetAllMethods())
            {
                var url = $"{ method.Name.ToLower().Map(x => x.EndsWith("module") ? x.Substring(0, x.Length - 6) : x)}";
                Get[$"{url}/{{argument}}"] = 
                    args => CallGetMethod(method, args.argument.ToString());
            }
        }

        private object CallGetMethod(MethodInfo method, string argument)
        {
            var returnType = typeof(Response<>).MakeGenericType(method.ReturnType);
            object returnValue;
            try
            {
                var argValue = JsonConvert.DeserializeObject(argument, method.GetParameters()[0].ParameterType, SerializerSettings);
                var retObj = method.Invoke(this, new[] { argValue });
                returnValue = Activator.CreateInstance(returnType, 200, string.Empty, retObj);
            }
            catch (Exception ex)
            {
                returnValue = Activator.CreateInstance(returnType, 500, ex.ToString(), GetEmptyBody(method.ReturnType));
            }
            return JsonConvert.SerializeObject(returnValue, SerializerSettings);
        }

        private object GetEmptyBody(Type type)
        {
            return type.IsValueType
                ? Activator.CreateInstance(type)
                : null;
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
