using System;
using System.Reflection;

namespace PlanetShine
{
  public class ClickEvent : EventArgs
  {
    public readonly IButton Button;
    public readonly int MouseButton;

    internal ClickEvent(object realEvent, IButton button)
    {
      Type type = realEvent.GetType();
      this.Button = button;
      this.MouseButton = (int) type.GetField(nameof (MouseButton), BindingFlags.Instance | BindingFlags.Public).GetValue(realEvent);
    }
  }
}
