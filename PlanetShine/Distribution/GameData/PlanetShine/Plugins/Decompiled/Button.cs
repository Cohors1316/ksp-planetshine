using System;
using System.Reflection;
using UnityEngine;

namespace PlanetShine
{
  internal class Button : IButton
  {
    private object realButton;
    private ToolbarTypes types;
    private Delegate realClickHandler;
    private Delegate realMouseEnterHandler;
    private Delegate realMouseLeaveHandler;
    private IVisibility visibility_;
    private IDrawable drawable_;

    internal Button(object realButton, ToolbarTypes types)
    {
      this.realButton = realButton;
      this.types = types;
      this.realClickHandler = this.attachEventHandler(types.button.onClickEvent, "clicked", realButton);
      this.realMouseEnterHandler = this.attachEventHandler(types.button.onMouseEnterEvent, "mouseEntered", realButton);
      this.realMouseLeaveHandler = this.attachEventHandler(types.button.onMouseLeaveEvent, "mouseLeft", realButton);
    }

    private Delegate attachEventHandler(
      EventInfo @event,
      string methodName,
      object realButton)
    {
      MethodInfo method = this.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
      Delegate handler = Delegate.CreateDelegate(@event.EventHandlerType, (object) this, method);
      @event.AddEventHandler(realButton, handler);
      return handler;
    }

    public string Text
    {
      set => this.types.button.textProperty.SetValue(this.realButton, (object) value, (object[]) null);
      get => (string) this.types.button.textProperty.GetValue(this.realButton, (object[]) null);
    }

    public Color TextColor
    {
      set => this.types.button.textColorProperty.SetValue(this.realButton, (object) value, (object[]) null);
      get => (Color) this.types.button.textColorProperty.GetValue(this.realButton, (object[]) null);
    }

    public string TexturePath
    {
      set => this.types.button.texturePathProperty.SetValue(this.realButton, (object) value, (object[]) null);
      get => (string) this.types.button.texturePathProperty.GetValue(this.realButton, (object[]) null);
    }

    public string ToolTip
    {
      set => this.types.button.toolTipProperty.SetValue(this.realButton, (object) value, (object[]) null);
      get => (string) this.types.button.toolTipProperty.GetValue(this.realButton, (object[]) null);
    }

    public bool Visible
    {
      set => this.types.button.visibleProperty.SetValue(this.realButton, (object) value, (object[]) null);
      get => (bool) this.types.button.visibleProperty.GetValue(this.realButton, (object[]) null);
    }

    public IVisibility Visibility
    {
      set
      {
        object obj = (object) null;
        if (value != null)
          obj = Activator.CreateInstance(this.types.functionVisibilityType, (object) (Func<bool>) (() => value.Visible));
        this.types.button.visibilityProperty.SetValue(this.realButton, obj, (object[]) null);
        this.visibility_ = value;
      }
      get => this.visibility_;
    }

    public bool EffectivelyVisible => (bool) this.types.button.effectivelyVisibleProperty.GetValue(this.realButton, (object[]) null);

    public bool Enabled
    {
      set => this.types.button.enabledProperty.SetValue(this.realButton, (object) value, (object[]) null);
      get => (bool) this.types.button.enabledProperty.GetValue(this.realButton, (object[]) null);
    }

    public bool Important
    {
      set => this.types.button.importantProperty.SetValue(this.realButton, (object) value, (object[]) null);
      get => (bool) this.types.button.importantProperty.GetValue(this.realButton, (object[]) null);
    }

    public IDrawable Drawable
    {
      set
      {
        object obj = (object) null;
        if (value != null)
          obj = Activator.CreateInstance(this.types.functionDrawableType, (object) (Action) (() => value.Update()), (object) (Func<Vector2, Vector2>) (pos => value.Draw(pos)));
        this.types.button.drawableProperty.SetValue(this.realButton, obj, (object[]) null);
        this.drawable_ = value;
      }
      get => this.drawable_;
    }

    public event ClickHandler OnClick;

    private void clicked(object realEvent)
    {
      if (this.OnClick == null)
        return;
      this.OnClick(new ClickEvent(realEvent, (IButton) this));
    }

    public event MouseEnterHandler OnMouseEnter;

    private void mouseEntered(object realEvent)
    {
      if (this.OnMouseEnter == null)
        return;
      this.OnMouseEnter(new MouseEnterEvent((IButton) this));
    }

    public event MouseLeaveHandler OnMouseLeave;

    private void mouseLeft(object realEvent)
    {
      if (this.OnMouseLeave == null)
        return;
      this.OnMouseLeave(new MouseLeaveEvent((IButton) this));
    }

    public void Destroy()
    {
      this.detachEventHandler(this.types.button.onClickEvent, this.realClickHandler, this.realButton);
      this.detachEventHandler(this.types.button.onMouseEnterEvent, this.realMouseEnterHandler, this.realButton);
      this.detachEventHandler(this.types.button.onMouseLeaveEvent, this.realMouseLeaveHandler, this.realButton);
      this.types.button.destroyMethod.Invoke(this.realButton, (object[]) null);
    }

    private void detachEventHandler(EventInfo @event, Delegate d, object realButton) => @event.RemoveEventHandler(realButton, d);
  }
}
