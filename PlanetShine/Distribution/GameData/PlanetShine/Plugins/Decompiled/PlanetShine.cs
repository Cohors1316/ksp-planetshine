using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace PlanetShine
{
  [KSPAddon]
  public class PlanetShine : MonoBehaviour
  {
    private Config config;
    public static GameObject[] albedoLights;
    public static DynamicAmbientLight ambientLight;
    public static bool renderEnabled = true;
    internal static string CurVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    private Stopwatch performanceTimer;
    public double performanceTimerLast;
    public static LineRenderer debugLineLightDirection = (LineRenderer) null;
    public static LineRenderer debugLineSunDirection = (LineRenderer) null;
    public static LineRenderer debugLineBodyDirection = (LineRenderer) null;
    public static LineRenderer[] debugLineLights = (LineRenderer[]) null;
    public CelestialBody body;
    public Color bodyColor;
    public float bodyAtmosphereAmbient;
    public float bodyGroundAmbientOverride;
    public float bodyIntensity;
    public float bodyRadius;
    public bool bodyIsSun;
    public Vector3 bodyVesselDirection;
    public Vector3 bodySunDirection;
    public float vesselBodyDistance;
    public float vesselAltitude;
    public float visibleSurface;
    public float sunAngle;
    public float visibleLightSunAngleMax;
    public float visibleLightSunAngleMin;
    public float visibleLightRatio;
    public float visibleLightAngleAverage;
    public float visibleLightAngleEffect;
    public float boostedVisibleLightAngleEffect;
    public Vector3 visibleLightPositionAverage;
    public float atmosphereReflectionRatio;
    public float atmosphereReflectionEffect;
    public float atmosphereAmbientRatio;
    public float atmosphereAmbientEffect;
    public float areaSpreadAngle;
    public float areaSpreadAngleRatio;
    public float lightRange;
    public float vesselLightRangeRatio;
    public float lightDistanceEffect;
    public Vector3 visibleLightVesselDirection;
    public float lightIntensity;
    public Color vacuumColor;
    public int fixedUpdateCounter;

    public static PlanetShine.PlanetShine Instance { get; private set; }

    public int albedoLightsQuantity => this.bodyIsSun ? 1 : this.config.albedoLightsQuantity;

    public void Awake()
    {
      if (Object.op_Inequality((Object) PlanetShine.PlanetShine.Instance, (Object) null))
        Object.Destroy((Object) ((Component) PlanetShine.PlanetShine.Instance).get_gameObject());
      PlanetShine.PlanetShine.Instance = this;
    }

    public void Start()
    {
      PlanetShine.PlanetShine.ambientLight = Object.FindObjectOfType(typeof (DynamicAmbientLight)) as DynamicAmbientLight;
      this.vacuumColor = new Color(this.config.vacuumLightLevel, this.config.vacuumLightLevel, this.config.vacuumLightLevel);
      if (Object.op_Inequality((Object) PlanetShine.PlanetShine.ambientLight, (Object) null))
        PlanetShine.PlanetShine.ambientLight.vacuumAmbientColor = (__Null) this.vacuumColor;
      this.CreateAlbedoLights();
      this.CreateDebugLines();
    }

    private void CreateDebugLines()
    {
      PlanetShine.PlanetShine.debugLineLightDirection = Utils.CreateDebugLine(Color.get_white(), Color.get_green());
      PlanetShine.PlanetShine.debugLineSunDirection = Utils.CreateDebugLine(Color.get_white(), Color.get_yellow());
      PlanetShine.PlanetShine.debugLineBodyDirection = Utils.CreateDebugLine(Color.get_white(), Color.get_red());
      PlanetShine.PlanetShine.debugLineLights = new LineRenderer[Config.maxAlbedoLightsQuantity];
      for (int index = 0; index < Config.maxAlbedoLightsQuantity; ++index)
        PlanetShine.PlanetShine.debugLineLights[index] = Utils.CreateDebugLine(Color.get_white(), Color.get_blue());
    }

    private void CreateAlbedoLights()
    {
      PlanetShine.PlanetShine.albedoLights = new GameObject[Config.maxAlbedoLightsQuantity];
      for (int index = 0; index < Config.maxAlbedoLightsQuantity; ++index)
      {
        if (Object.op_Inequality((Object) PlanetShine.PlanetShine.albedoLights[index], (Object) null))
          Object.Destroy((Object) PlanetShine.PlanetShine.albedoLights[index]);
        PlanetShine.PlanetShine.albedoLights[index] = new GameObject();
        Light light = (Light) PlanetShine.PlanetShine.albedoLights[index].AddComponent<Light>();
        light.set_type((LightType) 1);
        light.set_cullingMask(1);
        light.set_shadows((LightShadows) 2);
        light.set_shadowStrength(1f);
        PlanetShine.PlanetShine.albedoLights[index].AddComponent<MeshRenderer>();
      }
    }

    private void UpdateCelestialBody()
    {
      this.body = FlightGlobals.get_ActiveVessel().get_mainBody();
      this.bodyColor = new Color(25f / 64f, 25f / 64f, 25f / 64f);
      this.bodyAtmosphereAmbient = 0.3f;
      this.bodyIntensity = 1f;
      this.bodyGroundAmbientOverride = 0.5f;
      this.bodyIsSun = false;
      if (!this.config.celestialBodyInfos.ContainsKey(this.body))
        return;
      this.bodyColor = this.config.celestialBodyInfos[this.body].albedoColor;
      this.bodyIntensity = this.config.celestialBodyInfos[this.body].albedoIntensity;
      this.bodyAtmosphereAmbient = this.config.celestialBodyInfos[this.body].atmosphereAmbientLevel;
      this.bodyGroundAmbientOverride = this.config.celestialBodyInfos[this.body].groundAmbientOverride;
      this.bodyIsSun = this.config.celestialBodyInfos[this.body].isSun;
    }

    private void UpdateDebugLines()
    {
      if (this.config.debug)
      {
        PlanetShine.PlanetShine.debugLineLightDirection.SetPosition(0, this.visibleLightPositionAverage);
        PlanetShine.PlanetShine.debugLineLightDirection.SetPosition(1, ((Component) FlightGlobals.get_ActiveVessel()).get_transform().get_position());
        PlanetShine.PlanetShine.debugLineSunDirection.SetPosition(0, Vector3d.op_Implicit(((CelestialBody) ((Sun) Sun.Instance).sun).get_position()));
        PlanetShine.PlanetShine.debugLineSunDirection.SetPosition(1, ((Component) FlightGlobals.get_ActiveVessel()).get_transform().get_position());
        PlanetShine.PlanetShine.debugLineBodyDirection.SetPosition(0, Vector3d.op_Implicit(this.body.get_position()));
        PlanetShine.PlanetShine.debugLineBodyDirection.SetPosition(1, ((Component) FlightGlobals.get_ActiveVessel()).get_transform().get_position());
      }
      ((Renderer) PlanetShine.PlanetShine.debugLineLightDirection).set_enabled(this.config.debug);
      ((Renderer) PlanetShine.PlanetShine.debugLineBodyDirection).set_enabled(this.config.debug);
      ((Renderer) PlanetShine.PlanetShine.debugLineSunDirection).set_enabled(this.config.debug);
      foreach (Renderer debugLineLight in PlanetShine.PlanetShine.debugLineLights)
        debugLineLight.set_enabled(false);
      int index = 0;
      foreach (GameObject albedoLight in PlanetShine.PlanetShine.albedoLights)
      {
        if (this.albedoLightsQuantity > 1)
        {
          ((Renderer) PlanetShine.PlanetShine.debugLineLights[index]).set_enabled(this.config.debug);
          PlanetShine.PlanetShine.debugLineLights[index].SetPosition(0, Vector3.op_Subtraction(((Component) FlightGlobals.get_ActiveVessel()).get_transform().get_position(), Vector3.op_Multiply(((Component) albedoLight.GetComponent<Light>()).get_transform().get_forward(), 10000f)));
          PlanetShine.PlanetShine.debugLineLights[index].SetPosition(1, ((Component) FlightGlobals.get_ActiveVessel()).get_transform().get_position());
        }
        else
        {
          ((Renderer) PlanetShine.PlanetShine.debugLineLights[1]).set_enabled(this.config.debug);
          PlanetShine.PlanetShine.debugLineLights[1].SetPosition(0, Vector3.op_Subtraction(((Component) FlightGlobals.get_ActiveVessel()).get_transform().get_position(), Vector3.op_Multiply(((Component) albedoLight.GetComponent<Light>()).get_transform().get_forward(), 10000f)));
          PlanetShine.PlanetShine.debugLineLights[1].SetPosition(1, ((Component) FlightGlobals.get_ActiveVessel()).get_transform().get_position());
        }
        ++index;
      }
    }

    private void UpdateAlbedoLights()
    {
      this.bodyRadius = (float) this.body.Radius * 0.999f;
      Vector3d vector3d1 = Vector3d.op_Subtraction(((Component) FlightGlobals.get_ActiveVessel()).get_transform().get_position(), this.body.get_position());
      this.bodyVesselDirection = Vector3d.op_Implicit(((Vector3d) ref vector3d1).get_normalized());
      Vector3d vector3d2;
      Vector3 vector3_1;
      if (!this.bodyIsSun)
      {
        vector3d2 = Vector3d.op_Subtraction(((CelestialBody) ((Sun) Sun.Instance).sun).get_position(), this.body.get_position());
        vector3_1 = Vector3d.op_Implicit(((Vector3d) ref vector3d2).get_normalized());
      }
      else
        vector3_1 = this.bodyVesselDirection;
      this.bodySunDirection = vector3_1;
      vector3d2 = Vector3d.op_Subtraction(((Component) FlightGlobals.get_ActiveVessel()).get_transform().get_position(), this.body.get_position());
      this.vesselBodyDistance = (float) ((Vector3d) ref vector3d2).get_magnitude();
      this.vesselAltitude = Math.Max(this.vesselBodyDistance - this.bodyRadius, 1f);
      double vesselAltitude = (double) this.vesselAltitude;
      vector3d2 = Vector3d.op_Subtraction(((Component) FlightGlobals.get_ActiveVessel()).get_transform().get_position(), this.body.get_position());
      double magnitude = ((Vector3d) ref vector3d2).get_magnitude();
      this.visibleSurface = (float) (vesselAltitude / magnitude);
      this.sunAngle = Vector3.Angle(this.bodySunDirection, this.bodyVesselDirection);
      this.visibleLightSunAngleMax = (float) (90.0 + 90.0 * (double) this.visibleSurface);
      this.visibleLightSunAngleMin = (float) (90.0 - 90.0 * (double) this.visibleSurface);
      this.visibleLightRatio = Mathf.Clamp01((float) (((double) this.visibleLightSunAngleMax - (double) this.sunAngle) / ((double) this.visibleLightSunAngleMax - (double) this.visibleLightSunAngleMin)));
      this.visibleLightAngleAverage = (float) (90.0 * (double) this.visibleSurface * (1.0 - (double) this.visibleLightRatio * (1.0 - (double) this.sunAngle / 180.0)));
      this.visibleLightAngleEffect = Mathf.Clamp01((float) (1.0 - ((double) this.sunAngle - (double) this.visibleLightAngleAverage) / 90.0));
      this.boostedVisibleLightAngleEffect = Mathf.Clamp01(this.visibleLightAngleEffect + 0.3f);
      this.visibleLightPositionAverage = Vector3d.op_Implicit(Vector3d.op_Addition(this.body.get_position(), Vector3.op_Multiply(Vector3.RotateTowards(this.bodyVesselDirection, this.bodySunDirection, this.visibleLightAngleAverage * 0.01745f, 0.0f), this.bodyRadius)));
      this.atmosphereReflectionRatio = Mathf.Clamp01((float) (((double) this.vesselAltitude - (double) this.bodyRadius * (double) this.config.minAlbedoFadeAltitude) / ((double) this.bodyRadius * ((double) this.config.maxAlbedoFadeAltitude - (double) this.config.minAlbedoFadeAltitude))));
      this.atmosphereReflectionEffect = Mathf.Clamp01(1f - this.bodyAtmosphereAmbient + this.atmosphereReflectionRatio);
      this.atmosphereAmbientRatio = 1f - Mathf.Clamp01((float) (((double) this.vesselAltitude - (double) this.bodyRadius * (double) this.config.minAmbientFadeAltitude) / ((double) this.bodyRadius * ((double) this.config.maxAmbientFadeAltitude - (double) this.config.minAmbientFadeAltitude))));
      this.atmosphereAmbientEffect = this.bodyAtmosphereAmbient * this.config.baseGroundAmbient * this.atmosphereAmbientRatio;
      this.areaSpreadAngle = Math.Min(45f, (float) ((double) this.visibleLightRatio * (1.0 - (double) this.sunAngle / 180.0) * 57.2957801818848) * (float) Math.Acos(Math.Sqrt((double) Math.Max((float) ((double) this.vesselBodyDistance * (double) this.vesselBodyDistance - (double) this.bodyRadius * (double) this.bodyRadius), 1f)) / (double) this.vesselBodyDistance));
      this.areaSpreadAngleRatio = Mathf.Clamp01(this.areaSpreadAngle / 45f);
      this.lightRange = this.bodyRadius * this.config.albedoRange;
      this.vesselLightRangeRatio = this.vesselAltitude / this.lightRange;
      this.lightDistanceEffect = (float) (1.0 / (1.0 + 25.0 * (double) this.vesselLightRangeRatio * (double) this.vesselLightRangeRatio));
      Vector3 vector3_2 = Vector3.op_Subtraction(((Component) FlightGlobals.get_ActiveVessel()).get_transform().get_position(), this.visibleLightPositionAverage);
      this.visibleLightVesselDirection = ((Vector3) ref vector3_2).get_normalized();
      this.lightIntensity = this.config.baseAlbedoIntensity / (float) this.albedoLightsQuantity;
      this.lightIntensity *= this.visibleLightRatio * this.boostedVisibleLightAngleEffect * this.atmosphereReflectionEffect * this.lightDistanceEffect * this.bodyIntensity;
      if (this.albedoLightsQuantity > 1)
        this.lightIntensity *= (float) (1.0 + (double) this.areaSpreadAngleRatio * (double) this.areaSpreadAngleRatio * 0.5);
      int num = 0;
      foreach (GameObject albedoLight in PlanetShine.PlanetShine.albedoLights)
      {
        if (this.config.useVertex && !this.bodyIsSun)
          ((Light) albedoLight.GetComponent<Light>()).set_renderMode((LightRenderMode) 2);
        else
          ((Light) albedoLight.GetComponent<Light>()).set_renderMode((LightRenderMode) 1);
        ((Light) albedoLight.GetComponent<Light>()).set_intensity(this.lightIntensity);
        ((Component) albedoLight.GetComponent<Light>()).get_transform().set_forward(this.visibleLightVesselDirection);
        if (this.albedoLightsQuantity > 1)
        {
          Transform transform = ((Component) albedoLight.GetComponent<Light>()).get_transform();
          double areaSpreadAngle = (double) this.areaSpreadAngle;
          Vector3 vector3_3 = Vector3.Cross(this.bodyVesselDirection, this.bodySunDirection);
          Vector3 normalized = ((Vector3) ref vector3_3).get_normalized();
          Vector3 vector3_4 = Quaternion.op_Multiply(Quaternion.AngleAxis((float) areaSpreadAngle, normalized), ((Component) albedoLight.GetComponent<Light>()).get_transform().get_forward());
          transform.set_forward(vector3_4);
          ((Component) albedoLight.GetComponent<Light>()).get_transform().set_forward(Quaternion.op_Multiply(Quaternion.AngleAxis((float) num * (360f / (float) this.albedoLightsQuantity), this.visibleLightVesselDirection), ((Component) albedoLight.GetComponent<Light>()).get_transform().get_forward()));
        }
        ((Light) albedoLight.GetComponent<Light>()).set_color(this.bodyColor);
        if (PlanetShine.PlanetShine.renderEnabled && num < this.albedoLightsQuantity && !MapView.get_MapIsEnabled())
          ((Behaviour) albedoLight.GetComponent<Light>()).set_enabled(true);
        else
          ((Behaviour) albedoLight.GetComponent<Light>()).set_enabled(false);
        ++num;
      }
    }

    private void UpdateAmbientLights()
    {
      if (!Object.op_Inequality((Object) PlanetShine.PlanetShine.ambientLight, (Object) null))
        return;
      this.vacuumColor.r = (__Null) (double) (this.vacuumColor.g = this.vacuumColor.b = (__Null) this.config.vacuumLightLevel);
      PlanetShine.PlanetShine.ambientLight.vacuumAmbientColor = (__Null) this.vacuumColor;
      if (PlanetShine.PlanetShine.renderEnabled && !MapView.get_MapIsEnabled())
      {
        DynamicAmbientLight ambientLight = PlanetShine.PlanetShine.ambientLight;
        ambientLight.vacuumAmbientColor = (__Null) Color.op_Addition((Color) ambientLight.vacuumAmbientColor, Color.op_Multiply(this.atmosphereAmbientEffect * this.visibleLightAngleEffect, this.bodyColor));
        RenderSettings.set_ambientLight(Color.op_Multiply(RenderSettings.get_ambientLight(), (float) (1.0 - (double) this.config.groundAmbientOverrideRatio * (double) this.bodyGroundAmbientOverride)));
        RenderSettings.set_ambientLight(Color.op_Addition(RenderSettings.get_ambientLight(), Color.op_Multiply(Color.op_Multiply((Color) PlanetShine.PlanetShine.ambientLight.vacuumAmbientColor, this.config.groundAmbientOverrideRatio), this.bodyGroundAmbientOverride)));
      }
    }

    public void FixedUpdate()
    {
      if ((uint) (this.fixedUpdateCounter++ % this.config.updateFrequency) > 0U || Object.op_Equality((Object) FlightGlobals.get_ActiveVessel(), (Object) null))
        return;
      if (this.config.debug)
      {
        this.performanceTimer.Reset();
        this.performanceTimer.Start();
      }
      this.UpdateCelestialBody();
      this.UpdateAlbedoLights();
      this.UpdateDebugLines();
      if (!this.config.debug)
        return;
      this.performanceTimer.Stop();
      this.performanceTimerLast = this.performanceTimer.Elapsed.TotalMilliseconds;
    }

    public void LateUpdate() => this.UpdateAmbientLights();

    public PlanetShine() => base.\u002Ector();
  }
}
