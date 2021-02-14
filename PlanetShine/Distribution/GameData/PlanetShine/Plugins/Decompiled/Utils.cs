using System;
using UnityEngine;

namespace PlanetShine
{
  public class Utils
  {
    public static LineRenderer CreateDebugLine(Color startColor, Color endColor)
    {
      LineRenderer lineRenderer = (LineRenderer) new GameObject("Line").AddComponent<LineRenderer>();
      ((Renderer) lineRenderer).set_material(new Material(Shader.Find("Particles/Additive")));
      lineRenderer.SetColors(startColor, endColor);
      lineRenderer.SetWidth(0.05f, 0.05f);
      lineRenderer.SetVertexCount(2);
      return lineRenderer;
    }

    public static Color GetUnreadableTextureAverageColor(Texture2D texture)
    {
      Texture2D readable = Utils.CreateReadable(texture);
      Color textureAverageColor = Utils.GetTextureAverageColor(readable);
      Object.Destroy((Object) readable);
      return textureAverageColor;
    }

    public static Color GetPixelsAverageColor(Color[] texColors)
    {
      int length = texColors.Length;
      float num1 = 0.0f;
      float num2 = 0.0f;
      float num3 = 0.0f;
      foreach (Color texColor in texColors)
      {
        num1 += (float) texColor.r;
        num2 += (float) texColor.g;
        num3 += (float) texColor.b;
      }
      return new Color(num1 / (float) length, num2 / (float) length, num3 / (float) length, 1f);
    }

    public static Color GetTextureAverageColor(Texture2D texture) => Utils.GetPixelsAverageColor(texture.GetPixels());

    public static Color GetRimOuterColor(Texture2D texture, float fraction) => Utils.GetPixelsAverageColor(texture.GetPixels(0, 0, (int) Math.Round((double) ((Texture) texture).get_width() * (double) fraction), ((Texture) texture).get_height()));

    public static Texture2D CreateReadable(Texture2D original)
    {
      if (Object.op_Equality((Object) original, (Object) null) || (((Texture) original).get_width() == 0 || ((Texture) original).get_height() == 0))
        return (Texture2D) null;
      Texture2D texture2D = new Texture2D(((Texture) original).get_width(), ((Texture) original).get_height());
      RenderTexture temporary = RenderTexture.GetTemporary(((Texture) original).get_width(), ((Texture) original).get_height(), 0, (RenderTextureFormat) 0, (RenderTextureReadWrite) 2, 1);
      Graphics.Blit((Texture) original, temporary);
      RenderTexture.set_active(temporary);
      texture2D.ReadPixels(new Rect(0.0f, 0.0f, (float) ((Texture) texture2D).get_width(), (float) ((Texture) texture2D).get_height()), 0, 0);
      RenderTexture.set_active((RenderTexture) null);
      RenderTexture.ReleaseTemporary(temporary);
      return texture2D;
    }
  }
}
