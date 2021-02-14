using UnityEngine;

namespace PlanetShine
{
  public interface IButton
  {
    string Text { set; get; }

    Color TextColor { set; get; }

    string TexturePath { set; get; }

    string ToolTip { set; get; }

    bool Visible { set; get; }

    IVisibility Visibility { set; get; }

    bool EffectivelyVisible { get; }

    bool Enabled { set; get; }

    bool Important { set; get; }

    IDrawable Drawable { set; get; }

    event ClickHandler OnClick;

    event MouseEnterHandler OnMouseEnter;

    event MouseLeaveHandler OnMouseLeave;

    void Destroy();
  }
}
