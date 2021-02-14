using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetShine
{
  [KSPAddon]
  public class ConfigManager : MonoBehaviour
  {
    private Config config;
    private ConfigNode configFile;
    private ConfigNode configFileNode;

    public static ConfigManager Instance { get; private set; }

    public void Awake()
    {
      if (Object.op_Inequality((Object) ConfigManager.Instance, (Object) null))
        Object.Destroy((Object) ((Component) ConfigManager.Instance).get_gameObject());
      ConfigManager.Instance = this;
      this.LoadSettings();
      using (IEnumerator<AssemblyLoader.LoadedAssembly> enumerator = ((AssemblyLoader.LoadedAssembyList) AssemblyLoader.loadedAssemblies).GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          AssemblyLoader.LoadedAssembly current = enumerator.Current;
          if (current.get_name() == "Toolbar")
            this.config.blizzyToolbarInstalled = true;
          if (current.get_name() == "Kopernicus")
            this.config.kopernicusInstalled = true;
        }
      }
    }

    public void LoadSettings()
    {
      this.configFile = ConfigNode.Load(KSPUtil.get_ApplicationRootPath() + "GameData/PlanetShine/Config/Settings.cfg");
      this.configFileNode = this.configFile.GetNode("PlanetShine");
      this.config.albedoLightsQuantity = !bool.Parse(this.configFileNode.GetValue("useAreaLight")) ? 1 : Config.maxAlbedoLightsQuantity;
      this.config.baseAlbedoIntensity = float.Parse(this.configFileNode.GetValue("baseAlbedoIntensity"));
      this.config.vacuumLightLevel = float.Parse(this.configFileNode.GetValue("vacuumLightLevel"));
      this.config.baseGroundAmbient = float.Parse(this.configFileNode.GetValue("baseGroundAmbient"));
      this.config.groundAmbientOverrideRatio = float.Parse(this.configFileNode.GetValue("groundAmbientOverrideRatio"));
      this.config.minAlbedoFadeAltitude = float.Parse(this.configFileNode.GetValue("minAlbedoFadeAltitude"));
      this.config.maxAlbedoFadeAltitude = float.Parse(this.configFileNode.GetValue("maxAlbedoFadeAltitude"));
      this.config.minAmbientFadeAltitude = float.Parse(this.configFileNode.GetValue("minAmbientFadeAltitude"));
      this.config.maxAmbientFadeAltitude = float.Parse(this.configFileNode.GetValue("maxAmbientFadeAltitude"));
      this.config.albedoRange = float.Parse(this.configFileNode.GetValue("albedoRange"));
      this.config.useVertex = bool.Parse(this.configFileNode.GetValue("useVertex"));
      this.config.updateFrequency = int.Parse(this.configFileNode.GetValue("updateFrequency"));
      this.config.setQuality(int.Parse(this.configFileNode.GetValue("quality")));
      if (this.configFileNode.HasValue("stockToolbarEnabled"))
        this.config.stockToolbarEnabled = bool.Parse(this.configFileNode.GetValue("stockToolbarEnabled"));
      if (FlightGlobals.get_Bodies() == null)
        return;
      foreach (ConfigNode configNode in GameDatabase.get_Instance().GetConfigNodes("PlanetshineCelestialBody"))
        this.LoadBody(configNode);
    }

    protected void LoadBody(ConfigNode bodySettings)
    {
      try
      {
        CelestialBody key = FlightGlobals.get_Bodies().Find((Predicate<CelestialBody>) (n => n.get_name() == bodySettings.GetValue("name")));
        if (!FlightGlobals.get_Bodies().Contains(key))
          return;
        Color albedoColor = Color.op_Multiply(ConfigNode.ParseColor(bodySettings.GetValue("color")), float.Parse(bodySettings.GetValue("intensity")));
        albedoColor.r = (__Null) (albedoColor.r / (double) byte.MaxValue);
        albedoColor.g = (__Null) (albedoColor.g / (double) byte.MaxValue);
        albedoColor.b = (__Null) (albedoColor.b / (double) byte.MaxValue);
        albedoColor.a = (__Null) 1.0;
        if (!this.config.celestialBodyInfos.ContainsKey(key))
          this.config.celestialBodyInfos.Add(key, new CelestialBodyInfo(albedoColor, float.Parse(bodySettings.GetValue("intensity")), float.Parse(bodySettings.GetValue("atmosphereAmbient")), float.Parse(bodySettings.GetValue("groundAmbientOverride")), bodySettings.HasValue("isSun") && bool.Parse(bodySettings.GetValue("isSun"))));
      }
      catch (Exception ex)
      {
        Debug.LogError((object) string.Format("[PlanetShine] An exception occured reading CelestialBodyColor node:\n{0}\nThe exception was:\n{1}", (object) bodySettings, (object) ex));
      }
    }

    public void SaveSettings()
    {
      this.configFileNode.SetValue("useAreaLight", this.config.albedoLightsQuantity > 1 ? "True" : "False", false);
      this.configFileNode.SetValue("baseAlbedoIntensity", this.config.baseAlbedoIntensity.ToString(), false);
      this.configFileNode.SetValue("vacuumLightLevel", this.config.vacuumLightLevel.ToString(), false);
      this.configFileNode.SetValue("baseGroundAmbient", this.config.baseGroundAmbient.ToString(), false);
      this.configFileNode.SetValue("groundAmbientOverrideRatio", this.config.groundAmbientOverrideRatio.ToString(), false);
      this.configFileNode.SetValue("minAlbedoFadeAltitude", this.config.minAlbedoFadeAltitude.ToString(), false);
      this.configFileNode.SetValue("maxAlbedoFadeAltitude", this.config.maxAlbedoFadeAltitude.ToString(), false);
      this.configFileNode.SetValue("minAmbientFadeAltitude", this.config.minAmbientFadeAltitude.ToString(), false);
      this.configFileNode.SetValue("maxAmbientFadeAltitude", this.config.maxAmbientFadeAltitude.ToString(), false);
      this.configFileNode.SetValue("albedoRange", this.config.albedoRange.ToString(), false);
      this.configFileNode.SetValue("useVertex", this.config.useVertex ? "True" : "False", false);
      this.configFileNode.SetValue("updatefrequency", this.config.updateFrequency.ToString(), false);
      this.configFileNode.SetValue("quality", this.config.quality.ToString(), false);
      this.configFileNode.SetValue("stockToolbarEnabled", this.config.stockToolbarEnabled ? "True" : "False", false);
      this.configFile.Save(KSPUtil.get_ApplicationRootPath() + "GameData/PlanetShine/Config/Settings.cfg");
    }

    public ConfigManager() => base.\u002Ector();
  }
}
