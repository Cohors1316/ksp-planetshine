using System;
using System.Reflection;

namespace PlanetShine
{
  public class GameScenesVisibility : IVisibility
  {
    private object realGameScenesVisibility;
    private PropertyInfo visibleProperty;

    public bool Visible => (bool) this.visibleProperty.GetValue(this.realGameScenesVisibility, (object[]) null);

    public GameScenesVisibility(params GameScenes[] gameScenes)
    {
      Type type = ToolbarTypes.getType("Toolbar.GameScenesVisibility");
      this.realGameScenesVisibility = Activator.CreateInstance(type, (object) gameScenes);
      this.visibleProperty = ToolbarTypes.getProperty(type, nameof (Visible));
    }
  }
}
