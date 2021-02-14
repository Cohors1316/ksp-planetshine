using KSP.UI.Screens;
using UnityEngine;

namespace PlanetShine
{
  [KSPAddon]
  public class GuiManager : MonoBehaviour
  {
    private bool _isConfigDisplayed;
    private Config config;
    private PlanetShine.PlanetShine planetShine;
    private IButton blizzyButton;
    private ApplicationLauncherButton stockButton;
    private GuiRenderer guiRenderer;

    public bool isConfigDisplayed
    {
      get => this._isConfigDisplayed;
      set
      {
        if (this._isConfigDisplayed == value)
          return;
        this._isConfigDisplayed = value;
        this.UpdateButtonIcons();
      }
    }

    public void Start()
    {
      this.guiRenderer = new GuiRenderer(this);
      this.UpdateToolbarBlizzy();
      this.UpdateToolbarStock();
    }

    public void UpdateToolbarStock()
    {
      if (Object.op_Inequality((Object) this.stockButton, (Object) null))
      {
        if (this.config.stockToolbarEnabled || !this.config.blizzyToolbarInstalled)
          return;
        ApplicationLauncher.get_Instance().RemoveModApplication(this.stockButton);
        this.stockButton = (ApplicationLauncherButton) null;
      }
      else
      {
        if (!this.config.stockToolbarEnabled && this.config.blizzyToolbarInstalled)
          return;
        // ISSUE: method pointer
        // ISSUE: method pointer
        this.stockButton = ApplicationLauncher.get_Instance().AddModApplication(new Callback((object) this, __methodptr(\u003CUpdateToolbarStock\u003Eb__10_0)), new Callback((object) this, __methodptr(\u003CUpdateToolbarStock\u003Eb__10_1)), (Callback) null, (Callback) null, (Callback) null, (Callback) null, (ApplicationLauncher.AppScenes) 2, (Texture) GameDatabase.get_Instance().GetTexture("PlanetShine/Icons/ps_toolbar", false));
        if (!this.isConfigDisplayed)
          return;
        this.stockButton.SetTrue(true);
      }
    }

    public void UpdateToolbarBlizzy()
    {
      if (!this.config.blizzyToolbarInstalled || this.blizzyButton != null)
        return;
      this.blizzyButton = ToolbarManager.Instance.add("PlanetShine", "Gui");
      this.blizzyButton.TexturePath = "PlanetShine/Icons/ps_disabled";
      this.blizzyButton.Visibility = (IVisibility) new GameScenesVisibility(new GameScenes[1]
      {
        (GameScenes) 7
      });
      this.blizzyButton.ToolTip = "PlanetShine Settings";
      this.blizzyButton.OnClick += (ClickHandler) (e =>
      {
        this.planetShine = PlanetShine.PlanetShine.Instance;
        this.isConfigDisplayed = !this.isConfigDisplayed;
      });
    }

    private void UpdateButtonIcons()
    {
      if (this.blizzyButton != null)
        this.blizzyButton.TexturePath = this.isConfigDisplayed ? "PlanetShine/Icons/ps_enabled" : "PlanetShine/Icons/ps_disabled";
      if (!Object.op_Inequality((Object) this.stockButton, (Object) null))
        return;
      if (this._isConfigDisplayed)
        this.stockButton.SetTrue(true);
      else
        this.stockButton.SetFalse(true);
    }

    private void OnGUI()
    {
      if (!this.isConfigDisplayed)
        return;
      this.guiRenderer.Render(this.planetShine);
    }

    private void OnDestroy()
    {
      if (Object.op_Inequality((Object) this.stockButton, (Object) null))
        ApplicationLauncher.get_Instance().RemoveModApplication(this.stockButton);
      if (this.blizzyButton == null)
        return;
      this.blizzyButton.Destroy();
    }

    public GuiManager() => base.\u002Ector();
  }
}
