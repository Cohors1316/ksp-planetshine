using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetShine
{
  internal class GuiRenderer
  {
    private Config config = Config.Instance;
    private GuiManager guiManager;
    private PlanetShine.PlanetShine planetShine;
    private static string[] tabs = new string[3]
    {
      "Performance",
      "Intensity",
      "Advanced"
    };
    private static int currentTab = 0;
    private static Color tabUnselectedColor = new Color(0.8f, 0.8f, 0.8f);
    private static Color tabSelectedColor = new Color(0.1f, 0.1f, 0.1f);
    private static Color tabUnselectedTextColor = new Color(0.8f, 0.8f, 0.8f);
    private static Color tabSelectedTextColor = new Color(0.4f, 0.4f, 0.4f);
    private static Rect configWindowPosition = new Rect(0.0f, 60f, 200f, 80f);
    private static Rect debugWindowPosition = new Rect((float) (Screen.get_width() - 420), 60f, 80f, 80f);
    private static GUIStyle windowStyle = (GUIStyle) null;
    private static GUIStyle tabStyle = (GUIStyle) null;
    private Color originalBackgroundColor;
    private Color originalTextColor;
    private int debugWindowLabelWidth = 200;
    private int debugWindowDataWidth = 200;
    private int settingsLabelWidth = 100;
    private int updateCounter = 0;

    public GuiRenderer(GuiManager manager)
    {
      this.guiManager = manager;
      GuiRenderer.windowStyle = new GUIStyle(HighLogic.get_Skin().get_window());
      GuiRenderer.tabStyle = new GUIStyle(HighLogic.get_Skin().get_window());
      this.planetShine = PlanetShine.PlanetShine.Instance;
    }

    public bool Render(PlanetShine.PlanetShine planetShine)
    {
      // ISSUE: method pointer
      GuiRenderer.configWindowPosition = GUILayout.Window(143751300, GuiRenderer.configWindowPosition, new GUI.WindowFunction((object) this, __methodptr(OnConfigWindow)), "PlanetShine v" + PlanetShine.PlanetShine.CurVersion + " - Beta", GuiRenderer.windowStyle, new GUILayoutOption[0]);
      if (this.config.debug && Object.op_Inequality((Object) PlanetShine.PlanetShine.Instance, (Object) null))
      {
        // ISSUE: method pointer
        GuiRenderer.debugWindowPosition = GUILayout.Window(143751301, GuiRenderer.debugWindowPosition, new GUI.WindowFunction((object) this, __methodptr(OnDebugWindow)), "--- PLANETSHINE DEBUG ---", GuiRenderer.windowStyle, new GUILayoutOption[0]);
      }
      if (this.updateCounter % 100 == 0)
        ConfigManager.Instance.SaveSettings();
      ++this.updateCounter;
      return true;
    }

    private void OnConfigWindow(int windowID)
    {
      this.originalBackgroundColor = GUI.get_backgroundColor();
      this.originalTextColor = GUI.get_contentColor();
      if (GUI.Button(new Rect(((Rect) ref GuiRenderer.configWindowPosition).get_width() - 22f, 3f, 19f, 19f), "x"))
        this.guiManager.isConfigDisplayed = false;
      GUILayout.Space(15f);
      GUILayout.BeginVertical(new GUILayoutOption[0]);
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      for (int index = 0; index < GuiRenderer.tabs.Length; ++index)
      {
        GUI.set_backgroundColor(GuiRenderer.currentTab == index ? GuiRenderer.tabSelectedColor : GuiRenderer.tabUnselectedColor);
        GUI.set_contentColor(GuiRenderer.currentTab == index ? new Color(0.6f, 1f, 0.8f) : new Color(0.4f, 0.66f, 0.53f));
        if (GUILayout.Button(GuiRenderer.tabs[index], new GUILayoutOption[0]))
          GuiRenderer.currentTab = index;
      }
      GUI.set_contentColor(this.originalTextColor);
      GUI.set_backgroundColor(this.originalBackgroundColor);
      GUILayout.EndHorizontal();
      GUILayout.Space(30f);
      switch (GuiRenderer.currentTab)
      {
        case 0:
          this.DisplayTab();
          break;
        case 1:
          this.IntensityTab();
          break;
        case 2:
          this.AdvancedTab();
          break;
      }
      GUILayout.EndVertical();
      GUI.DragWindow();
      ((Rect) ref GuiRenderer.configWindowPosition).set_x(Mathf.Clamp(((Rect) ref GuiRenderer.configWindowPosition).get_x(), 0.0f, (float) Screen.get_width() - ((Rect) ref GuiRenderer.configWindowPosition).get_width()));
      ((Rect) ref GuiRenderer.configWindowPosition).set_y(Mathf.Clamp(((Rect) ref GuiRenderer.configWindowPosition).get_y(), 0.0f, (float) Screen.get_height() - ((Rect) ref GuiRenderer.configWindowPosition).get_height()));
    }

    private void DisplayTab()
    {
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      GUILayout.Label("Quality preset", new GUILayoutOption[1]
      {
        GUILayout.Width((float) this.settingsLabelWidth)
      });
      for (int selectedQuality = 0; selectedQuality < 3; ++selectedQuality)
      {
        GUI.set_backgroundColor(this.config.quality == selectedQuality ? GuiRenderer.tabSelectedColor : GuiRenderer.tabUnselectedColor);
        GUI.set_contentColor(this.config.quality == selectedQuality ? Color.get_white() : new Color(0.6f, 0.6f, 0.6f));
        if (GUILayout.Button(Config.qualityLabels[selectedQuality], new GUILayoutOption[0]))
          this.config.setQuality(selectedQuality);
      }
      GUI.set_backgroundColor(this.originalBackgroundColor);
      GUI.set_contentColor(this.originalTextColor);
      GUILayout.EndHorizontal();
      GUILayout.Space(15f);
      this.QualityChoiceRow<int>("Lights quantity", ref this.config.albedoLightsQuantity, new DisplaySettingOption<int>[2]
      {
        new DisplaySettingOption<int>("Single", 1),
        new DisplaySettingOption<int>("Multiple (area)", Config.maxAlbedoLightsQuantity)
      });
      this.QualityChoiceRow<bool>("Lights rendering", ref this.config.useVertex, new DisplaySettingOption<bool>[2]
      {
        new DisplaySettingOption<bool>("Vertex mode", true),
        new DisplaySettingOption<bool>("Pixel mode", false)
      });
      this.QualityChoiceRow<int>("Update frequency", ref this.config.updateFrequency, new DisplaySettingOption<int>[3]
      {
        new DisplaySettingOption<int>("10 per second", 5),
        new DisplaySettingOption<int>("25 per second", 2),
        new DisplaySettingOption<int>("50 per second", 1)
      });
    }

    private void IntensityTab()
    {
      GUILayout.Label("Planetshine light intensity: " + (object) this.config.baseAlbedoIntensity, new GUILayoutOption[0]);
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      this.config.baseAlbedoIntensity = (float) Math.Round((double) GUILayout.HorizontalSlider(this.config.baseAlbedoIntensity, 0.0f, 0.5f, new GUILayoutOption[0]), 2);
      this.ResetButton<float>(ref this.config.baseAlbedoIntensity, ConfigDefaults.baseAlbedoIntensity);
      GUILayout.EndHorizontal();
      GUILayout.Space(20f);
      GUILayout.Label("Vacuum ambient light level: " + (object) this.config.vacuumLightLevel, new GUILayoutOption[0]);
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      this.config.vacuumLightLevel = 5f * (float) Math.Round((double) GUILayout.HorizontalSlider(this.config.vacuumLightLevel / 5f, 0.0f, 0.2f, new GUILayoutOption[0]), 3);
      this.ResetButton<float>(ref this.config.vacuumLightLevel, ConfigDefaults.vacuumLightLevel);
      GUILayout.EndHorizontal();
      GUILayout.Space(20f);
      GUILayout.Label("Ground and atmosphere ambient light intensity: " + (object) this.config.baseGroundAmbient, new GUILayoutOption[0]);
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      this.config.baseGroundAmbient = (float) Math.Round((double) GUILayout.HorizontalSlider(this.config.baseGroundAmbient, 0.0f, 2f, new GUILayoutOption[0]), 1);
      this.ResetButton<float>(ref this.config.baseGroundAmbient, ConfigDefaults.baseGroundAmbient);
      GUILayout.EndHorizontal();
      GUILayout.Space(20f);
      GUILayout.Label("Ground ambient light override: " + (object) (float) ((double) this.config.groundAmbientOverrideRatio * 100.0) + "%", new GUILayoutOption[0]);
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      this.config.groundAmbientOverrideRatio = (float) Math.Round((double) GUILayout.HorizontalSlider(this.config.groundAmbientOverrideRatio, 0.0f, 1f, new GUILayoutOption[0]), 1);
      this.ResetButton<float>(ref this.config.groundAmbientOverrideRatio, ConfigDefaults.groundAmbientOverrideRatio);
      GUILayout.EndHorizontal();
      GUILayout.Space(20f);
      GUILayout.Label("Planetshine maximum range: " + (object) this.config.albedoRange, new GUILayoutOption[0]);
      GUILayout.Label("(approximately " + (object) Math.Round((double) this.config.albedoRange * (double) this.planetShine.bodyRadius / 2000.0) + "km from " + this.planetShine.body.get_name() + ")", new GUILayoutOption[0]);
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      this.config.albedoRange = (float) Math.Round((double) GUILayout.HorizontalSlider(this.config.albedoRange, 0.0f, 20f, new GUILayoutOption[0]), 1);
      this.ResetButton<float>(ref this.config.albedoRange, ConfigDefaults.albedoRange);
      GUILayout.EndHorizontal();
    }

    private void AdvancedTab()
    {
      PlanetShine.PlanetShine.renderEnabled = GUILayout.Toggle(PlanetShine.PlanetShine.renderEnabled, "Planetshine global ON/OFF", new GUILayoutOption[0]);
      if (this.config.blizzyToolbarInstalled)
      {
        bool stockToolbarEnabled = this.config.stockToolbarEnabled;
        this.config.stockToolbarEnabled = GUILayout.Toggle(this.config.stockToolbarEnabled, "Use stock toolbar", new GUILayoutOption[0]);
        if (this.config.stockToolbarEnabled != stockToolbarEnabled)
          this.guiManager.UpdateToolbarStock();
      }
      GUILayout.Space(15f);
      GUI.set_contentColor(new Color(0.8f, 1f, 0.8f));
      GUILayout.Label("Planetshine fade altitude: " + (object) this.config.minAlbedoFadeAltitude + " to " + (object) this.config.maxAlbedoFadeAltitude, new GUILayoutOption[0]);
      GUILayout.Label("(from " + (object) Math.Round((double) this.config.minAlbedoFadeAltitude * (double) this.planetShine.bodyRadius / 1000.0) + "km to " + (object) Math.Round((double) this.config.maxAlbedoFadeAltitude * (double) this.planetShine.bodyRadius / 1000.0) + "km on " + this.planetShine.body.get_name() + ")", new GUILayoutOption[0]);
      GUI.set_contentColor(this.originalTextColor);
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      GUILayout.Label("Min", new GUILayoutOption[1]
      {
        GUILayout.Width(50f)
      });
      this.config.minAlbedoFadeAltitude = (float) Math.Round((double) GUILayout.HorizontalSlider(this.config.minAlbedoFadeAltitude, 0.0f, this.config.maxAlbedoFadeAltitude, new GUILayoutOption[0]), 2);
      this.ResetButton<float>(ref this.config.minAlbedoFadeAltitude, ConfigDefaults.minAlbedoFadeAltitude);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      GUILayout.Label("Max", new GUILayoutOption[1]
      {
        GUILayout.Width(50f)
      });
      this.config.maxAlbedoFadeAltitude = (float) Math.Round((double) GUILayout.HorizontalSlider(this.config.maxAlbedoFadeAltitude, this.config.minAlbedoFadeAltitude, 0.1f, new GUILayoutOption[0]), 2);
      this.ResetButton<float>(ref this.config.maxAlbedoFadeAltitude, ConfigDefaults.maxAlbedoFadeAltitude);
      GUILayout.EndHorizontal();
      GUILayout.Space(15f);
      GUI.set_contentColor(new Color(0.8f, 1f, 0.8f));
      GUILayout.Label("Ground ambient fade altitude: " + (object) this.config.minAmbientFadeAltitude + " to " + (object) this.config.maxAmbientFadeAltitude, new GUILayoutOption[0]);
      GUILayout.Label("(from " + (object) Math.Round((double) this.config.minAmbientFadeAltitude * (double) this.planetShine.bodyRadius / 1000.0) + "km to " + (object) Math.Round((double) this.config.maxAmbientFadeAltitude * (double) this.planetShine.bodyRadius / 1000.0) + "km on " + this.planetShine.body.get_name() + ")", new GUILayoutOption[0]);
      GUI.set_contentColor(this.originalTextColor);
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      GUILayout.Label("Min", new GUILayoutOption[1]
      {
        GUILayout.Width(50f)
      });
      this.config.minAmbientFadeAltitude = (float) Math.Round((double) GUILayout.HorizontalSlider(this.config.minAmbientFadeAltitude, 0.0f, this.config.maxAmbientFadeAltitude, new GUILayoutOption[0]), 2);
      this.ResetButton<float>(ref this.config.minAmbientFadeAltitude, ConfigDefaults.minAmbientFadeAltitude);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      GUILayout.Label("Max", new GUILayoutOption[1]
      {
        GUILayout.Width(50f)
      });
      this.config.maxAmbientFadeAltitude = (float) Math.Round((double) GUILayout.HorizontalSlider(this.config.maxAmbientFadeAltitude, this.config.minAmbientFadeAltitude, 0.1f, new GUILayoutOption[0]), 2);
      this.ResetButton<float>(ref this.config.maxAmbientFadeAltitude, ConfigDefaults.maxAmbientFadeAltitude);
      GUILayout.EndHorizontal();
      GUILayout.Space(30f);
      GUI.set_contentColor(new Color(1f, 0.7f, 0.6f));
      this.config.debug = GUILayout.Toggle(this.config.debug, "Debug mode", new GUILayoutOption[0]);
      GUI.set_contentColor(this.originalTextColor);
    }

    private void OnDebugWindow(int windowID)
    {
      GUILayout.BeginVertical(new GUILayoutOption[0]);
      this.VariableDebugLabel<bool>("MapView.MapIsEnabled", MapView.get_MapIsEnabled());
      this.VariableDebugLabel<double>("performanceTimerLast", this.planetShine.performanceTimerLast);
      this.VariableDebugLabel<string>("body.name", this.planetShine.body.get_name());
      GUI.set_contentColor(this.planetShine.bodyColor);
      this.VariableDebugLabel<Color>("bodyColor", this.planetShine.bodyColor);
      GUI.set_contentColor(this.originalTextColor);
      this.VariableDebugLabel<float>("bodyAtmosphereAmbient", this.planetShine.bodyAtmosphereAmbient);
      this.VariableDebugLabel<float>("bodyIntensity", this.planetShine.bodyIntensity);
      this.VariableDebugLabel<float>("bodyRadius", this.planetShine.bodyRadius);
      this.VariableDebugLabel<Vector3>("bodyVesselDirection", this.planetShine.bodyVesselDirection);
      this.VariableDebugLabel<Vector3>("bodySunDirection", this.planetShine.bodySunDirection);
      this.VariableDebugLabel<float>("vesselAltitude", this.planetShine.vesselAltitude);
      this.VariableDebugLabel<float>("visibleSurface", this.planetShine.visibleSurface);
      this.VariableDebugLabel<float>("sunAngle", this.planetShine.sunAngle);
      this.VariableDebugLabel<float>("visibleLightSunAngleMax", this.planetShine.visibleLightSunAngleMax);
      this.VariableDebugLabel<float>("visibleLightSunAngleMin", this.planetShine.visibleLightSunAngleMin);
      this.VariableDebugLabel<float>("visibleLightRatio", this.planetShine.visibleLightRatio);
      this.VariableDebugLabel<float>("visibleLightAngleAverage", this.planetShine.visibleLightAngleAverage);
      this.VariableDebugLabel<float>("visibleLightAngleEffect", this.planetShine.visibleLightAngleEffect);
      this.VariableDebugLabel<float>("boostedVisibleLightAngleEffect", this.planetShine.boostedVisibleLightAngleEffect);
      this.VariableDebugLabel<Vector3>("visibleLightPositionAverage", this.planetShine.visibleLightPositionAverage);
      this.VariableDebugLabel<float>("atmosphereReflectionRatio", this.planetShine.atmosphereReflectionRatio);
      this.VariableDebugLabel<float>("atmosphereReflectionEffect", this.planetShine.atmosphereReflectionEffect);
      this.VariableDebugLabel<float>("atmosphereAmbientRatio", this.planetShine.atmosphereAmbientRatio);
      this.VariableDebugLabel<float>("atmosphereAmbientEffect", this.planetShine.atmosphereAmbientEffect);
      this.VariableDebugLabel<float>("areaSpreadAngle", this.planetShine.areaSpreadAngle);
      this.VariableDebugLabel<float>("areaSpreadAngleRatio", this.planetShine.areaSpreadAngleRatio);
      this.VariableDebugLabel<float>("lightRange", this.planetShine.lightRange);
      this.VariableDebugLabel<float>("vesselLightRangeRatio", this.planetShine.vesselLightRangeRatio);
      this.VariableDebugLabel<float>("lightDistanceEffect", this.planetShine.lightDistanceEffect);
      this.VariableDebugLabel<Vector3>("visibleLightVesselDirection", this.planetShine.visibleLightVesselDirection);
      this.VariableDebugLabel<float>("lightIntensity", this.planetShine.lightIntensity);
      this.VariableDebugLabel<Color>("vacuumColor", this.planetShine.vacuumColor);
      this.VariableDebugLabel<int>("Gui.updateCounter", this.updateCounter);
      GUILayout.EndVertical();
      GUI.DragWindow();
      ((Rect) ref GuiRenderer.debugWindowPosition).set_x(Mathf.Clamp(((Rect) ref GuiRenderer.debugWindowPosition).get_x(), 0.0f, (float) Screen.get_width() - ((Rect) ref GuiRenderer.debugWindowPosition).get_width()));
      ((Rect) ref GuiRenderer.debugWindowPosition).set_y(Mathf.Clamp(((Rect) ref GuiRenderer.debugWindowPosition).get_y(), 0.0f, (float) Screen.get_height() - ((Rect) ref GuiRenderer.debugWindowPosition).get_height()));
    }

    private void QualityChoiceRow<T>(string label, ref T target, DisplaySettingOption<T>[] choices)
    {
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      GUILayout.Label(label, new GUILayoutOption[1]
      {
        GUILayout.Width((float) this.settingsLabelWidth)
      });
      foreach (DisplaySettingOption<T> choice in choices)
      {
        GUI.set_backgroundColor(EqualityComparer<T>.Default.Equals(target, choice.value) ? GuiRenderer.tabSelectedColor : GuiRenderer.tabUnselectedColor);
        GUI.set_contentColor(EqualityComparer<T>.Default.Equals(target, choice.value) ? Color.get_white() : new Color(0.6f, 0.6f, 0.6f));
        if (GUILayout.Button(choice.label, new GUILayoutOption[0]) && !EqualityComparer<T>.Default.Equals(target, choice.value))
        {
          this.config.setQuality(3);
          target = choice.value;
        }
      }
      GUI.set_backgroundColor(this.originalBackgroundColor);
      GUI.set_contentColor(this.originalTextColor);
      GUILayout.EndHorizontal();
    }

    private void VariableDebugLabel<T>(string name, T data)
    {
      GUILayout.BeginHorizontal(new GUILayoutOption[0]);
      GUILayout.Label(name, new GUILayoutOption[1]
      {
        GUILayout.Width((float) this.debugWindowLabelWidth)
      });
      GUILayout.Label(data.ToString(), new GUILayoutOption[1]
      {
        GUILayout.Width((float) this.debugWindowDataWidth)
      });
      GUILayout.EndHorizontal();
    }

    private void ResetButton<T>(ref T target, T original)
    {
      if (!GUILayout.Button("Reset", new GUILayoutOption[1]
      {
        GUILayout.Width(50f)
      }))
        return;
      target = original;
    }
  }
}
