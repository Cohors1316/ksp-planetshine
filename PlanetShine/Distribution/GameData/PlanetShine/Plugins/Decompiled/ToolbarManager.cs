using System;
using System.Collections.Generic;
using System.Reflection;

namespace PlanetShine
{
  public class ToolbarManager : IToolbarManager
  {
    private static bool? toolbarAvailable = new bool?();
    private static IToolbarManager instance_;
    private object realToolbarManager;
    private MethodInfo addMethod;
    private Dictionary<object, IButton> buttons = new Dictionary<object, IButton>();
    private ToolbarTypes types = new ToolbarTypes();

    public static bool ToolbarAvailable
    {
      get
      {
        if (!ToolbarManager.toolbarAvailable.HasValue)
          ToolbarManager.toolbarAvailable = new bool?(ToolbarManager.Instance != null);
        return ToolbarManager.toolbarAvailable.Value;
      }
    }

    public static IToolbarManager Instance
    {
      get
      {
        bool? toolbarAvailable = ToolbarManager.toolbarAvailable;
        bool flag = false;
        if ((toolbarAvailable.GetValueOrDefault() == flag ? (!toolbarAvailable.HasValue ? 1 : 0) : 1) != 0 && ToolbarManager.instance_ == null)
        {
          Type type = ToolbarTypes.getType("Toolbar.ToolbarManager");
          if (type != null)
            ToolbarManager.instance_ = (IToolbarManager) new ToolbarManager(ToolbarTypes.getStaticProperty(type, nameof (Instance)).GetValue((object) null, (object[]) null));
        }
        return ToolbarManager.instance_;
      }
    }

    private ToolbarManager(object realToolbarManager)
    {
      this.realToolbarManager = realToolbarManager;
      this.addMethod = ToolbarTypes.getMethod(this.types.iToolbarManagerType, "add");
    }

    public IButton add(string ns, string id)
    {
      object obj = this.addMethod.Invoke(this.realToolbarManager, new object[2]
      {
        (object) ns,
        (object) id
      });
      IButton button = (IButton) new Button(obj, this.types);
      this.buttons.Add(obj, button);
      return button;
    }
  }
}
