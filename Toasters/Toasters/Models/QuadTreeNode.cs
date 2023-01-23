using System.Collections.Generic;
using System.Linq;
using Toasters.ViewModels;

namespace Toasters.Models;

/// <summary>
/// The QuadTreeNode
/// </summary>
public class QuadTreeNode
{
    /// <summary>
    /// Construct a quadtree node with the given bounds 
    /// </summary>
    public QuadTreeNode(RectangleViewModel bounds, QuadTree tree)
    {
        Tree = tree;
        _mBounds = bounds;
    }

    public QuadTree Tree { get; }

    /// <summary>
    /// The area of this node
    /// </summary>
    private readonly RectangleViewModel _mBounds;

    /// <summary>
    /// The child nodes of the QuadTree
    /// </summary>
    private readonly List<QuadTreeNode> _mNodes = new(4);

    /// <summary>
    /// Is the node empty
    /// </summary>
    public bool IsEmpty => Contents.Count == 0 && (_mBounds.IsEmpty || _mNodes.Count == 0);

    /// <summary>
    /// Area of the quadtree node
    /// </summary>
    public RectangleViewModel Bounds => _mBounds;

    /// <summary>
    /// Total number of nodes in the this node and all SubNodes
    /// </summary>
    public int Count
    {
        get
        {
            var count = 0;

            foreach (var node in _mNodes)
                count += node.Count;

            count += Contents.Count;

            return count;
        }
    }

    /// <summary>
    /// Return the contents of this node and all subnodes in the true below this one.
    /// </summary>
    public IEnumerable<RectangleViewModel> SubTreeContents
    {
        get
        {
            foreach (var node in _mNodes)
            foreach (var t in node.SubTreeContents)
                yield return t;

            foreach (var t in Contents)
                yield return t;
        }
    }

    /// <summary>
    /// The contents of this node.
    /// Note that the contents have no limit: this is not the standard way to impement a QuadTree
    /// </summary>
    public List<RectangleViewModel> Contents { get; } = new();

    /// <summary>
    /// Query the QuadTree for items that are in the given area
    /// </summary>
    public IEnumerable<RectangleViewModel> Query(RectangleViewModel queryArea)
    {
        // this quad contains items that are not entirely contained by
        // it's four sub-quads. Iterate through the items in this quad 
        // to see if they intersect.
        foreach (var item in Contents.Where(queryArea.IntersectsWith))
        {
            yield return item;
        }

        foreach (var node in _mNodes.Where(node => !node.IsEmpty))
        {
            // Case 1: search area completely contained by sub-quad
            // if a node completely contains the query area, go down that branch
            // and skip the remaining nodes (break this loop)
            if (node.Bounds.Contains(queryArea))
            {
                foreach (var t in node.Query(queryArea))
                    yield return t;
                break;
            }

            // Case 2: Sub-quad completely contained by search area 
            // if the query area completely contains a sub-quad,
            // just add all the contents of that quad and it's children 
            // to the result set. You need to continue the loop to test 
            // the other quads
            if (queryArea.Contains(node.Bounds))
            {
                foreach (var t in node.SubTreeContents)
                    yield return t;
                continue;
            }

            // Case 3: search area intersects with sub-quad
            // traverse into this quad, continue the loop to search other
            // quads
            if (node.Bounds.IntersectsWith(queryArea))
            {
                foreach (var t in node.Query(queryArea))
                    yield return t;
            }
        }
    }

    /// <summary>
    /// Insert an item to this node
    /// </summary>
    /// <param name="item"></param>
    public void Insert(RectangleViewModel item)
    {
        // // if the item is not contained in this quad, there's a problem
        // if (!m_bounds.Contains(item))
        //     throw new ArgumentException("feature is out of the bounds of this quadtree node");

        // if the subnodes are null create them. may not be sucessfull: see below
        // we may be at the smallest allowed size in which case the subnodes will not be created
        //if (m_nodes.Count == 0)
        //    CreateSubNodes();

        if (_mNodes.Count == 0 && Contents.Count >= Tree.NodeCapacity)
        {
            CreateSubNodes();
            MoveContentsToSubNodes();
        }

        if (Contents.Count > Tree.NodeCapacity)
        {
            //this node is full, let's try and store T in a subnode, if it's small enough.

            // for each subnode:
            // if the node contains the item, add the item to that node and return
            // this recurses into the node that is just large enough to fit this item
            foreach (var node in _mNodes.Where(node => node.Bounds.Contains(item)))
            {
                node.Insert(item);
                return;
            }
        }

        //add, even if we are over capacity.
        Contents.Add(item);
    }

    private void MoveContentsToSubNodes()
    {
        Contents.RemoveAll(t =>
        {
            foreach (var n in _mNodes.Where(n => n.Bounds.Contains(t)))
            {
                n.Insert(t);
                return true;
            }

            return false;
        });
    }

    public void ForEach(QuadTree.QtAction action)
    {
        action(this);

        // draw the child quads
        foreach (var node in _mNodes)
            node.ForEach(action);
    }

    public IEnumerable<QuadTreeNode> Nodes
    {
        get
        {
            foreach (var node in _mNodes)
            {
                yield return node;
                foreach (var sub in node._mNodes)
                    yield return sub;
            }
        }
    }

    /// <summary>
    /// Internal method to create the subnodes (partitions space)
    /// </summary>
    private void CreateSubNodes()
    {
        // the smallest subnode has an area 
        if ((_mBounds.Height * _mBounds.Width) <= Tree.MinNodeSize)
            return;

        var halfWidth = (_mBounds.Width / 2f);
        var halfHeight = (_mBounds.Height / 2f);

        _mNodes.Add(new QuadTreeNode(new RectangleViewModel(_mBounds.Location, new Size(halfWidth, halfHeight)),
            Tree));
        _mNodes.Add(new QuadTreeNode(
            new RectangleViewModel(new Vector(_mBounds.Left, _mBounds.Top + halfHeight),
                new Size(halfWidth, halfHeight)), Tree));
        _mNodes.Add(new QuadTreeNode(
            new RectangleViewModel(new Vector(_mBounds.Left + halfWidth, _mBounds.Top),
                new Size(halfWidth, halfHeight)), Tree));
        _mNodes.Add(new QuadTreeNode(
            new RectangleViewModel(new Vector(_mBounds.Left + halfWidth, _mBounds.Top + halfHeight),
                new Size(halfWidth, halfHeight)), Tree));
    }
}