using System;
using System.Collections.Generic;
using Toasters.ViewModels;

namespace Toasters.Models;

public class QuadTree
{
    /// <summary>
    /// The root QuadTreeNode
    /// </summary>
    private readonly QuadTreeNode _mRoot;

    /// <summary>
    /// The bounds of this QuadTree
    /// </summary>
    private readonly RectangleViewModel _mRectangle;

    /// <summary>
    /// An delegate that performs an action on a QuadTreeNode
    /// </summary>
    /// <param name="obj"></param>
    public delegate void QtAction(QuadTreeNode obj);

    public int NodeCapacity => 5;

    public int MinNodeSize => 10;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rectangle"></param>
    public QuadTree(RectangleViewModel rectangle)
    {
        _mRectangle = rectangle;
        _mRoot = new QuadTreeNode(_mRectangle, this);
    }

    /// <summary>
    /// Get the count of items in the QuadTree
    /// </summary>
    public int Count => _mRoot.Count;

    /// <summary>
    /// Insert the feature into the QuadTree
    /// </summary>
    /// <param name="item"></param>
    public void Insert(RectangleViewModel item)
    {
        _mRoot.Insert(item);
    }

    public void Remove(RectangleViewModel item)
    {
        ForEach(node =>
        {
            if (node.Contents.Contains(item))
                node.Contents.Remove(item);
        });
    }

    /// <summary>
    /// Query the QuadTree, returning the items that are in the given area
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public IEnumerable<RectangleViewModel> Query(RectangleViewModel area)
    {
        return _mRoot.Query(area);
    }

    public IEnumerable<QuadTreeNode> Nodes
    {
        get
        {
            yield return _mRoot;
            foreach (var r in _mRoot.Nodes)
                yield return r;
        }
    }

    /// <summary>
    /// Do the specified action for each item in the quadtree
    /// </summary>
    /// <param name="action"></param>
    public void ForEach(QtAction action)
    {
        _mRoot.ForEach(action);
    }


    public void Clear()
    {
        ForEach(node => { node.Contents.Clear(); });
    }

    public void Update(RectangleViewModel item)
    {
        Remove(item);
        Insert(item);
    }
}