using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace QuestFramework.Framework.Patching
{
    internal abstract class Patcher
    {
        public abstract void Apply(Harmony harmony, IMonitor monitor);

        protected MethodInfo RequireMethod<TTarget>(string name, Type[]? parameters = null, Type[]? generics = null)
        {
            return AccessTools.Method(typeof(TTarget), name, parameters, generics)
                ?? throw new InvalidOperationException($"Can't find method {GetMethodString(typeof(TTarget), name, parameters, generics)} to patch.");
        }

        protected MethodInfo RequirePropertyGetter<TTarget>(string name)
        {
            return AccessTools.PropertyGetter(typeof(TTarget), name)
                ?? throw new InvalidOperationException($"Can't find property getter {GetMethodString(typeof(TTarget), $"get_{name}")} to patch.");
        }

        protected MethodInfo RequirePropertySetter<TTarget>(string name)
        {
            return AccessTools.PropertySetter(typeof(TTarget), name)
                ?? throw new InvalidOperationException($"Can't find property setter {GetMethodString(typeof(TTarget), $"set_{name}")} to patch.");
        }

        protected ConstructorInfo RequireConstructor<TTarget>(Type[]? parameters = null)
        {
            return
                AccessTools.Constructor(typeof(TTarget), parameters)
                ?? throw new InvalidOperationException($"Can't find constructor {GetMethodString(typeof(TTarget), null, parameters)} to patch.");
        }

        protected HarmonyMethod GetHarmonyMethod(string name, int priority = -1, string[]? before = null, string[]? after = null, bool? debug = null)
        {
            var method = AccessTools.Method(GetType(), name)
                    ?? throw new InvalidOperationException($"Can't find patcher method {GetMethodString(GetType(), name)}.");

            if (!method.IsStatic)
            {
                throw new InvalidOperationException($"Patcher method {GetMethodString(GetType(), name)} must be static.");
            }

            return new HarmonyMethod(
                method,
                priority, before, after, debug
            );
        }

        private static string GetMethodString(Type type, string? name, Type[]? parameters = null, Type[]? generics = null)
        {
            StringBuilder str = new();

            // type
            str.Append(type.FullName);

            // method name (if not constructor)
            if (name != null)
            {
                str.Append('.');
                str.Append(name);
            }

            // generics
            if (generics?.Any() == true)
            {
                str.Append('<');
                str.Append(string.Join(", ", generics.Select(p => p.FullName)));
                str.Append('>');
            }

            // parameters
            if (parameters?.Any() == true)
            {
                str.Append('(');
                str.Append(string.Join(", ", parameters.Select(p => p.FullName)));
                str.Append(')');
            }

            return str.ToString();
        }
    }
}
