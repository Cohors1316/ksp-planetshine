using System.Collections.Generic;

namespace PlanetShine
{
  public sealed class Config
  {
    private static readonly Config instance = new Config();
    public bool blizzyToolbarInstalled = false;
    public bool kopernicusInstalled = false;
    public static string[] qualityLabels = new string[3]
    {
      "Low",
      "Medium",
      "High"
    };
    public static int maxAlbedoLightsQuantity = 4;
    public bool useVertex = false;
    public int albedoLightsQuantity = 4;
    public float baseAlbedoIntensity = 0.24f;
    public float vacuumLightLevel = 0.03f;
    public float baseGroundAmbient = 0.6f;
    public float groundAmbientOverrideRatio = 0.6f;
    public float minAlbedoFadeAltitude = 0.02f;
    public float maxAlbedoFadeAltitude = 0.1f;
    public float minAmbientFadeAltitude = 0.0f;
    public float maxAmbientFadeAltitude = 0.1f;
    public float albedoRange = 10f;
    public bool debug = false;
    public int updateFrequency = 1;
    public Dictionary<CelestialBody, CelestialBodyInfo> celestialBodyInfos = new Dictionary<CelestialBody, CelestialBodyInfo>();
    public bool stockToolbarEnabled = true;

    private Config()
    {
    }

    public static Config Instance => Config.instance;

    public int quality { get; private set; }

    public void setQuality(int selectedQuality)
    {
      this.quality = selectedQuality;
      switch (selectedQuality)
      {
        case 0:
          this.albedoLightsQuantity = 1;
          this.useVertex = true;
          this.updateFrequency = 5;
          break;
        case 1:
          this.albedoLightsQuantity = Config.maxAlbedoLightsQuantity;
          this.useVertex = true;
          this.updateFrequency = 2;
          break;
        case 2:
          this.albedoLightsQuantity = Config.maxAlbedoLightsQuantity;
          this.useVertex = false;
          this.updateFrequency = 1;
          break;
      }
    }
  }
}
