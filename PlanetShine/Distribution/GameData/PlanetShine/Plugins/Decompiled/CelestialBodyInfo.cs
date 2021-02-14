using UnityEngine;

namespace PlanetShine
{
  public class CelestialBodyInfo
  {
    public Color albedoColor;
    public float albedoIntensity;
    public float atmosphereAmbientLevel;
    public float groundAmbientOverride;
    public bool isSun;

    public CelestialBodyInfo(
      Color albedoColor,
      float albedoIntensity,
      float atmosphereAmbientLevel,
      float groundAmbientOverride,
      bool isSun)
    {
      this.albedoColor = albedoColor;
      this.albedoIntensity = albedoIntensity;
      this.atmosphereAmbientLevel = atmosphereAmbientLevel;
      this.groundAmbientOverride = groundAmbientOverride;
      this.isSun = isSun;
    }
  }
}
