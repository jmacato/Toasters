namespace Toasters.Models;

public struct Vector
{
    public float X { get; set; }
    public float Y { get; set; }

    public Vector(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static Vector operator +(Vector a, Vector b)
    {
        return new Vector(a.X + b.X, a.Y + b.Y);
    }

    public static Vector operator -(Vector a, Vector b)
    {
        return new Vector(a.X - b.X, a.Y - b.Y);
    }

    public static Vector operator *(Vector a, float scalar)
    {
        return new Vector(a.X * scalar, a.Y * scalar);
    }

    public static Vector operator /(Vector a, float scalar)
    {
        return new Vector(a.X / scalar, a.Y / scalar);
    }

    public static Vector Random(float maxX, float maxY)
    {
        return new Vector(System.Random.Shared.Next((int)maxX), System.Random.Shared.Next((int)maxY));
    }
}
