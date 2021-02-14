using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PlanetShine
{
  internal class ToolbarTypes
  {
    internal readonly Type iToolbarManagerType;
    internal readonly Type functionVisibilityType;
    internal readonly Type functionDrawableType;
    internal readonly ButtonTypes button;

    internal ToolbarTypes()
    {
      this.iToolbarManagerType = ToolbarTypes.getType("Toolbar.IToolbarManager");
      this.functionVisibilityType = ToolbarTypes.getType("Toolbar.FunctionVisibility");
      this.functionDrawableType = ToolbarTypes.getType("Toolbar.FunctionDrawable");
      this.button = new ButtonTypes(ToolbarTypes.getType("Toolbar.IButton"));
    }

    internal static Type getType(string name) => ((IEnumerable<AssemblyLoader.LoadedAssembly>) AssemblyLoader.loadedAssemblies).SelectMany<AssemblyLoader.LoadedAssembly, Type>((Func<AssemblyLoader.LoadedAssembly, IEnumerable<Type>>) (a => (IEnumerable<Type>) a.get_assembly().GetExportedTypes())).SingleOrDefault<Type>((Func<Type, bool>) (t => t.FullName == name));

    internal static PropertyInfo getProperty(Type type, string name) => type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);

    internal static PropertyInfo getStaticProperty(Type type, string name) => type.GetProperty(name, BindingFlags.Static | BindingFlags.Public);

    internal static EventInfo getEvent(Type type, string name) => type.GetEvent(name, BindingFlags.Instance | BindingFlags.Public);

    internal static MethodInfo getMethod(Type type, string name) => type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
  }
}
