namespace PlanetShine
{
  public class DisplaySettingOption<T>
  {
    public string label { get; private set; }

    public T value { get; private set; }

    public DisplaySettingOption(string label, T value)
    {
      this.label = label;
      this.value = value;
    }
  }
}
