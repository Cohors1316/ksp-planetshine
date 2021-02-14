using System;
using System.Reflection;
using UnityEngine;

namespace PlanetShine
{
  public class PopupMenuDrawable : IDrawable
  {
    private object realPopupMenuDrawable;
    private MethodInfo updateMethod;
    private MethodInfo drawMethod;
    private MethodInfo addOptionMethod;
    private MethodInfo addSeparatorMethod;
    private MethodInfo destroyMethod;
    private EventInfo onAnyOptionClickedEvent;

    public event Action OnAnyOptionClicked
    {
      add => this.onAnyOptionClickedEvent.AddEventHandler(this.realPopupMenuDrawable, (Delegate) value);
      remove => this.onAnyOptionClickedEvent.RemoveEventHandler(this.realPopupMenuDrawable, (Delegate) value);
    }

    public PopupMenuDrawable()
    {
      Type type = ToolbarTypes.getType("Toolbar.PopupMenuDrawable");
      this.realPopupMenuDrawable = Activator.CreateInstance(type, (object[]) null);
      this.updateMethod = ToolbarTypes.getMethod(type, "Update");
      this.drawMethod = ToolbarTypes.getMethod(type, "Draw");
      this.addOptionMethod = ToolbarTypes.getMethod(type, "AddOption");
      this.addSeparatorMethod = ToolbarTypes.getMethod(type, "AddSeparator");
      this.destroyMethod = ToolbarTypes.getMethod(type, "Destroy");
      this.onAnyOptionClickedEvent = ToolbarTypes.getEvent(type, "OnAnyOptionClicked");
    }

    public void Update() => this.updateMethod.Invoke(this.realPopupMenuDrawable, (object[]) null);

    public Vector2 Draw(Vector2 position) => (Vector2) this.drawMethod.Invoke(this.realPopupMenuDrawable, new object[1]
    {
      (object) position
    });

    public IButton AddOption(string text) => (IButton) new Button(this.addOptionMethod.Invoke(this.realPopupMenuDrawable, new object[1]
    {
      (object) text
    }), new ToolbarTypes());

    public void AddSeparator() => this.addSeparatorMethod.Invoke(this.realPopupMenuDrawable, (object[]) null);

    public void Destroy() => this.destroyMethod.Invoke(this.realPopupMenuDrawable, (object[]) null);
  }
}
