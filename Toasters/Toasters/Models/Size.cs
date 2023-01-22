namespace Toasters.Models;

public struct Size
{
    public float Width { get; set; }
    public float Height { get; set; }

    public Size(float width, float height)
    {
        Width = width;
        Height = height;
    }
}