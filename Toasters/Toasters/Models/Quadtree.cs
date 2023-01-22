using System.Collections.Generic;
using Toasters.ViewModels;

namespace Toasters.Models;

public class Quadtree
{
    private const int MaxObjects = 10;
    private const int MaxLevels = 5;

    private int _level;
    private List<RectangleViewModel> _objects;
    private RectangleViewModel _bounds;
    private Quadtree[] _nodes;

    public Quadtree()
    {
        _level = 0;
        _objects = new List<RectangleViewModel>();
        _nodes = new Quadtree[4];
        _bounds = new RectangleViewModel(0, 0, 0, 0);
    }

    public void Clear()
    {
        _objects.Clear();

        for (var i = 0; i < _nodes.Length; i++)
        {
            if (_nodes[i] != null)
            {
                _nodes[i].Clear();
                _nodes[i] = null;
            }
        }
    }

    public void Insert(RectangleViewModel rect)
    {
        if (_bounds.IsEmpty)
        {
            _bounds = rect;
        }
        else
        {
            _bounds = RectangleViewModel.Union(_bounds, rect);
        }

        if (_nodes[0] != null)
        {
            var index = GetIndex(rect);

            if (index != -1)
            {
                _nodes[index].Insert(rect);

                return;
            }
        }

        _objects.Add(rect);

        if (_objects.Count > MaxObjects && _level < MaxLevels)
        {
            if (_nodes[0] == null)
            {
                Split();
            }

            var i = 0;
            while (i < _objects.Count)
            {
                var index = GetIndex(_objects[i]);
                if (index != -1)
                {
                    _nodes[index].Insert(_objects[i]);
                    _objects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public List<RectangleViewModel> Retrieve(RectangleViewModel rect)
    {
        var returnObjects = new List<RectangleViewModel>();

        if (!_bounds.Intersects(rect))
            return returnObjects;

        var index = GetIndex(rect);
        if (index != -1 && _nodes[0] != null)
        {
            _nodes[index].Retrieve(rect);
        }

        foreach (var obj in _objects)
        {
            if (obj.Intersects(rect))
                returnObjects.Add(obj);
        }

        return returnObjects;
    }

    private void Split()
    {
        var subWidth = _bounds.Width / 2;
        var subHeight = _bounds.Height / 2;
        var x = _bounds.X;
        var y = _bounds.Y;

        _nodes[0] = new Quadtree();
        _nodes[0]._bounds = new RectangleViewModel(x + subWidth, y, subWidth, subHeight);
        _nodes[1] = new Quadtree();
        _nodes[1]._bounds = new RectangleViewModel(x, y, subWidth, subHeight);
        _nodes[2] = new Quadtree();
        _nodes[2]._bounds = new RectangleViewModel(x, y + subHeight, subWidth, subHeight);
        _nodes[3] = new Quadtree();
        _nodes[3]._bounds = new RectangleViewModel(x + subWidth, y + subHeight, subWidth, subHeight);
    }

    private int GetIndex(RectangleViewModel rect)
    {
        var index = -1;
        double verticalMidpoint = _bounds.X + (_bounds.Width / 2);
        double horizontalMidpoint = _bounds.Y + (_bounds.Height / 2);

        // Object can completely fit within the top quadrants
        var topQuadrant = (rect.Y < horizontalMidpoint && rect.Y + rect.Height < horizontalMidpoint);
        // Object can completely fit within the bottom quadrants
        var bottomQuadrant = (rect.Y > horizontalMidpoint);

        // Object can completely fit within the left quadrants
        if (rect.X < verticalMidpoint && rect.X + rect.Width < verticalMidpoint)
        {
            if (topQuadrant)
            {
                index = 1;
            }
            else if (bottomQuadrant)
            {
                index = 2;
            }
        }
        // Object can completely fit within the right quadrants
        else if (rect.X > verticalMidpoint)
        {
            if (topQuadrant)
            {
                index = 0;
            }
            else if (bottomQuadrant)
            {
                index = 3;
            }
        }

        return index;
    }

    public void Update(RectangleViewModel rect)
    {
        Remove(rect);
        Insert(rect);
    }

    private void Remove(RectangleViewModel rect)
    {
        if (_nodes[0] != null)
        {
            int index = GetIndex(rect);

            if (index != -1)
            {
                _nodes[index].Remove(rect);

                return;
            }
        }

        _objects.Remove(rect);
    }
}