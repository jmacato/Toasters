// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Toasters.Models;

namespace Toasters.ViewModels;

/// <summary>
/// Stores the location and size of a rectangular region.
/// </summary> 
public partial class RectangleViewModel : ViewModelBase, IEquatable<RectangleViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref='RectangleViewModel'/> class.
    /// </summary>
    public static readonly RectangleViewModel Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref='RectangleViewModel'/> class with the specified location
    /// and size.
    /// </summary>
    public RectangleViewModel(float x, float y, float width, float height)
    {
        Location = new Vector(x, y);
        Size = new Size(width, height);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='RectangleViewModel'/> class with the specified location
    /// and size.
    /// </summary>
    public RectangleViewModel(Vector location, Size size)
    {
        Location = location;
        Size = size;
    }

    /// <summary>
    /// Creates a new <see cref='RectangleViewModel'/> with the specified location and size.
    /// </summary>
    public static RectangleViewModel FromLTRB(float left, float top, float right, float bottom) =>
        new RectangleViewModel(left, top, right - left, bottom - top);

    /// <summary>
    /// Gets or sets the coordinates of the upper-left corner of the rectangular region represented by this
    /// <see cref='RectangleViewModel'/>.
    /// </summary>
    [ObservableProperty] private Vector _location;


    /// <summary>
    /// Gets or sets the size of this <see cref='RectangleViewModel'/>.
    /// </summary> 
    [ObservableProperty] private Size _size;

    /// <summary>
    /// Gets or sets the x-coordinate of the upper-left corner of the rectangular region defined by this
    /// <see cref='RectangleViewModel'/>.
    /// </summary>
    public float X
    {
        get => _location.X;
        set => _location.X = value;
    }

    /// <summary>
    /// Gets or sets the y-coordinate of the upper-left corner of the rectangular region defined by this
    /// <see cref='RectangleViewModel'/>.
    /// </summary>
    public float Y
    {
        get => _location.Y;
        set => _location.Y = value;
    }

    /// <summary>
    /// Gets or sets the width of the rectangular region defined by this <see cref='RectangleViewModel'/>.
    /// </summary>
    public float Width
    {
        get => _size.Width;
        set => _size.Width = value;
    }

    /// <summary>
    /// Gets or sets the height of the rectangular region defined by this <see cref='RectangleViewModel'/>.
    /// </summary>
    public float Height
    {
        get => _size.Height;
        set => _size.Height = value;
    }

    /// <summary>
    /// Gets the x-coordinate of the upper-left corner of the rectangular region defined by this
    /// <see cref='RectangleViewModel'/> .
    /// </summary>
    public float Left => X;

    /// <summary>
    /// Gets the y-coordinate of the upper-left corner of the rectangular region defined by this
    /// <see cref='RectangleViewModel'/>.
    /// </summary>
    public float Top => Y;

    /// <summary>
    /// Gets the x-coordinate of the lower-right corner of the rectangular region defined by this
    /// <see cref='RectangleViewModel'/>.
    /// </summary>

    public float Right => X + Width;

    /// <summary>
    /// Gets the y-coordinate of the lower-right corner of the rectangular region defined by this
    /// <see cref='RectangleViewModel'/>.
    /// </summary>

    public float Bottom => Y + Height;

    /// <summary>
    /// Tests whether this <see cref='RectangleViewModel'/> has a <see cref='RectangleViewModel.Width'/> or a <see cref='RectangleViewModel.Height'/> of 0.
    /// </summary>

    public bool IsEmpty => (Width <= 0) || (Height <= 0);

    /// <summary>
    /// Tests whether <paramref name="obj"/> is a <see cref='RectangleViewModel'/> with the same location and
    /// size of this <see cref='RectangleViewModel'/>.
    /// </summary>
    public override bool Equals(object? obj) => obj is RectangleViewModel && Equals((RectangleViewModel)obj);

    public bool Equals(RectangleViewModel other) => this == other;

    /// <summary>
    /// Tests whether two <see cref='RectangleViewModel'/> objects have equal location and size.
    /// </summary>
    public static bool operator ==(RectangleViewModel left, RectangleViewModel right) =>
        left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;

    /// <summary>
    /// Tests whether two <see cref='RectangleViewModel'/> objects differ in location or size.
    /// </summary>
    public static bool operator !=(RectangleViewModel left, RectangleViewModel right) => !(left == right);

    /// <summary>
    /// Determines if the specified point is contained within the rectangular region defined by this
    /// <see cref='Rectangle'/> .
    /// </summary>
    public bool Contains(float x, float y) => X <= x && x < X + Width && Y <= y && y < Y + Height;

    /// <summary>
    /// Determines if the specified point is contained within the rectangular region defined by this
    /// <see cref='Rectangle'/> .
    /// </summary>
    public bool Contains(Vector pt) => Contains(pt.X, pt.Y);

    /// <summary>
    /// Determines if the rectangular region represented by <paramref name="rect"/> is entirely contained within
    /// the rectangular region represented by this <see cref='Rectangle'/> .
    /// </summary>
    public bool Contains(RectangleViewModel rect) =>
        (X <= rect.X) && (rect.X + rect.Width <= X + Width) && (Y <= rect.Y) && (rect.Y + rect.Height <= Y + Height);

    /// <summary>
    /// Gets the hash code for this <see cref='RectangleViewModel'/>.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

    /// <summary>
    /// Inflates this <see cref='RectangleViewModel'/> by the specified amount.
    /// </summary>
    public void Inflate(float x, float y)
    {
        X -= x;
        Y -= y;
        Width += 2 * x;
        Height += 2 * y;
    }

    /// <summary>
    /// Inflates this <see cref='RectangleViewModel'/> by the specified amount.
    /// </summary>
    public void Inflate(Size size) => Inflate(size.Width, size.Height);

    /// <summary>
    /// Creates a <see cref='RectangleViewModel'/> that is inflated by the specified amount.
    /// </summary>
    public static RectangleViewModel Inflate(RectangleViewModel rect, float x, float y)
    {
        var r = rect;
        r.Inflate(x, y);
        return r;
    }

    /// <summary>
    /// Creates a Rectangle that represents the intersection between this Rectangle and rect.
    /// </summary>
    public void Intersect(RectangleViewModel rect)
    {
        var result = Intersect(rect, this);

        X = result.X;
        Y = result.Y;
        Width = result.Width;
        Height = result.Height;
    }

    /// <summary>
    /// Creates a rectangle that represents the intersection between a and b. If there is no intersection, an
    /// empty rectangle is returned.
    /// </summary>
    public static RectangleViewModel Intersect(RectangleViewModel a, RectangleViewModel b)
    {
        var x1 = Math.Max(a.X, b.X);
        var x2 = Math.Min(a.X + a.Width, b.X + b.Width);
        var y1 = Math.Max(a.Y, b.Y);
        var y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

        if (x2 >= x1 && y2 >= y1)
        {
            return new RectangleViewModel(x1, y1, x2 - x1, y2 - y1);
        }

        return Empty;
    }

    public bool IntersectsWith(RectangleViewModel rect) =>
        (rect.X < X + Width) && (X < rect.X + rect.Width) && (rect.Y < Y + Height) && (Y < rect.Y + rect.Height);

    public static RectangleViewModel Union(RectangleViewModel a, RectangleViewModel b)
    {
        var x1 = Math.Min(a.X, b.X);
        var x2 = Math.Max(a.X + a.Width, b.X + b.Width);
        var y1 = Math.Min(a.Y, b.Y);
        var y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

        return new RectangleViewModel(x1, y1, x2 - x1, y2 - y1);
    }

    public void Offset(Vector pos) => Offset(pos.X, pos.Y);

    public void Offset(float x, float y)
    {
        X += x;
        Y += y;
    }

    public override string ToString() => $"{{X={X},Y={Y},Width={Width},Height={Height}}}";
}