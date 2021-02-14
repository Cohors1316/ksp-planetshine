using System;
using System.Reflection;

namespace PlanetShine
{
  internal class ButtonTypes
  {
    internal readonly Type iButtonType;
    internal readonly PropertyInfo textProperty;
    internal readonly PropertyInfo textColorProperty;
    internal readonly PropertyInfo texturePathProperty;
    internal readonly PropertyInfo toolTipProperty;
    internal readonly PropertyInfo visibleProperty;
    internal readonly PropertyInfo visibilityProperty;
    internal readonly PropertyInfo effectivelyVisibleProperty;
    internal readonly PropertyInfo enabledProperty;
    internal readonly PropertyInfo importantProperty;
    internal readonly PropertyInfo drawableProperty;
    internal readonly EventInfo onClickEvent;
    internal readonly EventInfo onMouseEnterEvent;
    internal readonly EventInfo onMouseLeaveEvent;
    internal readonly MethodInfo destroyMethod;

    internal ButtonTypes(Type iButtonType)
    {
      this.iButtonType = iButtonType;
      this.textProperty = ToolbarTypes.getProperty(iButtonType, "Text");
      this.textColorProperty = ToolbarTypes.getProperty(iButtonType, "TextColor");
      this.texturePathProperty = ToolbarTypes.getProperty(iButtonType, "TexturePath");
      this.toolTipProperty = ToolbarTypes.getProperty(iButtonType, "ToolTip");
      this.visibleProperty = ToolbarTypes.getProperty(iButtonType, "Visible");
      this.visibilityProperty = ToolbarTypes.getProperty(iButtonType, "Visibility");
      this.effectivelyVisibleProperty = ToolbarTypes.getProperty(iButtonType, "EffectivelyVisible");
      this.enabledProperty = ToolbarTypes.getProperty(iButtonType, "Enabled");
      this.importantProperty = ToolbarTypes.getProperty(iButtonType, "Important");
      this.drawableProperty = ToolbarTypes.getProperty(iButtonType, "Drawable");
      this.onClickEvent = ToolbarTypes.getEvent(iButtonType, "OnClick");
      this.onMouseEnterEvent = ToolbarTypes.getEvent(iButtonType, "OnMouseEnter");
      this.onMouseLeaveEvent = ToolbarTypes.getEvent(iButtonType, "OnMouseLeave");
      this.destroyMethod = ToolbarTypes.getMethod(iButtonType, "Destroy");
    }
  }
}
