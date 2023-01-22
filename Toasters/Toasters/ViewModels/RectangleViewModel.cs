using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Toasters.Models;

namespace Toasters.ViewModels;

public partial class RectangleViewModel : ViewModelBase
{
    [ObservableProperty] private Vector _position;

    [ObservableProperty] private Size _size;

    public RectangleViewModel(Vector position, Size size)
    {
        Position = position;
        Size = size;
    }

    public RectangleViewModel(float x, float y, float width, float height)
    {
        Position = new Vector(x, y);
        Size = new Size(width, height);
    }

    public float X
    {
        get { return Position.X; }
        set { Position = new Vector(value, Position.Y); }
    }

    public float Y
    {
        get { return Position.Y; }
        set { Position = new Vector(Position.X, value); }
    }

    public float Width
    {
        get { return Size.Width; }
        set { Size = new Size(value, Height); }
    }

    public float Height
    {
        get { return Size.Height; }
        set { Size = new Size(Width, value); }
    }

    public bool Intersects(RectangleViewModel other)
    {
        return (X < other.X + other.Width &&
                X + Width > other.X &&
                Y < other.Y + other.Height &&
                Y + Height > other.Y);
    }


    public static RectangleViewModel Union(RectangleViewModel a, RectangleViewModel b)
    {
        var x = Math.Min(a.X, b.X);
        var y = Math.Min(a.Y, b.Y);
        var width = Math.Max(a.X + a.Width, b.X + b.Width) - x;
        var height = Math.Max(a.Y + a.Height, b.Y + b.Height) - y;
        return new RectangleViewModel(x, y, width, height);
    }


    public bool IsEmpty
    {
        get { return Width == 0 || Height == 0; }
    }
}