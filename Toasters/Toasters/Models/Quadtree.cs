using System;
using System.Collections.Generic;
using Toasters.ViewModels;

namespace Toasters.Models;

public class QuadTree
{
    /// <summary>
    /// The root QuadTreeNode
    /// </summary>
    QuadTreeNode m_root;

    /// <summary>
    /// The bounds of this QuadTree
    /// </summary>
    RectangleViewModel m_rectangle;

    /// <summary>
    /// An delegate that performs an action on a QuadTreeNode
    /// </summary>
    /// <param name="obj"></param>
    public delegate void QTAction(QuadTreeNode obj);

    private int nodeCapacity = 5;

    public int NodeCapacity
    {
        get { return this.nodeCapacity; }
        set { nodeCapacity = value; }
    }

    private int minNodeSize = 10;

    public int MinNodeSize
    {
        get { return this.minNodeSize; }
        set { minNodeSize = value; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rectangle"></param>
    public QuadTree(RectangleViewModel rectangle)
    {
        m_rectangle = rectangle;
        m_root = new QuadTreeNode(m_rectangle, this);
    }

    /// <summary>
    /// Get the count of items in the QuadTree
    /// </summary>
    public int Count
    {
        get { return m_root.Count; }
    }

    /// <summary>
    /// Insert the feature into the QuadTree
    /// </summary>
    /// <param name="item"></param>
    public void Insert(RectangleViewModel item)
    {
        m_root.Insert(item);
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
        return m_root.Query(area);
    }

    public IEnumerable<QuadTreeNode> Nodes
    {
        get
        {
            yield return m_root;
            foreach (var r in m_root.Nodes)
                yield return r;
        }
    }

    /// <summary>
    /// Do the specified action for each item in the quadtree
    /// </summary>
    /// <param name="action"></param>
    public void ForEach(QTAction action)
    {
        m_root.ForEach(action);
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

/// <summary>
/// The QuadTreeNode
/// </summary>
/// <typeparam name="T"></typeparam>
public class QuadTreeNode
{
    /// <summary>
    /// Construct a quadtree node with the given bounds 
    /// </summary>
    /// <param name="area"></param>
    public QuadTreeNode(RectangleViewModel bounds, QuadTree tree)
    {
        Tree = tree;
        m_bounds = bounds;
    }

    public QuadTree Tree { get; set; }

    /// <summary>
    /// The area of this node
    /// </summary>
    RectangleViewModel m_bounds;

    /// <summary>
    /// The contents of this node.
    /// Note that the contents have no limit: this is not the standard way to impement a QuadTree
    /// </summary>
    List<RectangleViewModel> m_contents = new List<RectangleViewModel>();

    /// <summary>
    /// The child nodes of the QuadTree
    /// </summary>
    List<QuadTreeNode> m_nodes = new List<QuadTreeNode>(4);

    /// <summary>
    /// Is the node empty
    /// </summary>
    public bool IsEmpty
    {
        get { return this.Contents.Count == 0 && (m_bounds.IsEmpty || m_nodes.Count == 0); }
    }

    /// <summary>
    /// Area of the quadtree node
    /// </summary>
    public RectangleViewModel Bounds
    {
        get { return m_bounds; }
    }

    /// <summary>
    /// Total number of nodes in the this node and all SubNodes
    /// </summary>
    public int Count
    {
        get
        {
            var count = 0;

            foreach (QuadTreeNode node in m_nodes)
                count += node.Count;

            count += this.Contents.Count;

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
            foreach (QuadTreeNode node in m_nodes)
            foreach (var t in node.SubTreeContents)
                yield return t;

            foreach (var t in this.Contents)
                yield return t;
        }
    }

    public List<RectangleViewModel> Contents
    {
        get { return m_contents; }
    }

    /// <summary>
    /// Query the QuadTree for items that are in the given area
    /// </summary>
    /// <param name="queryArea"></pasram>
    /// <returns></returns>
    public IEnumerable<RectangleViewModel> Query(RectangleViewModel queryArea)
    {
        // this quad contains items that are not entirely contained by
        // it's four sub-quads. Iterate through the items in this quad 
        // to see if they intersect.
        foreach (var item in this.Contents)
        {
            if (queryArea.IntersectsWith(item))
                yield return item;
        }

        foreach (QuadTreeNode node in m_nodes)
        {
            if (node.IsEmpty)
                continue;

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

        if (m_nodes.Count == 0 && this.Contents.Count >= Tree.NodeCapacity)
        {
            CreateSubNodes();
            MoveContentsToSubNodes();
        }

        if (this.Contents.Count > Tree.NodeCapacity)
        {
            //this node is full, let's try and store T in a subnode, if it's small enough.

            // for each subnode:
            // if the node contains the item, add the item to that node and return
            // this recurses into the node that is just large enough to fit this item
            foreach (QuadTreeNode node in m_nodes)
            {
                if (node.Bounds.Contains(item))
                {
                    node.Insert(item);
                    return;
                }
            }
        }

        //add, even if we are over capacity.
        this.Contents.Add(item);
    }

    void MoveContentsToSubNodes()
    {
        Contents.RemoveAll(t =>
        {
            foreach (var n in m_nodes)
            {
                 if (n.Bounds.Contains(t))
                {
                    n.Insert(t);
                    return true;
                }
            }

            return false;
        });
    }

    public void ForEach(QuadTree.QTAction action)
    {
        action(this);

        // draw the child quads
        foreach (QuadTreeNode node in this.m_nodes)
            node.ForEach(action);
    }

    public IEnumerable<QuadTreeNode> Nodes
    {
        get
        {
            foreach (var node in this.m_nodes)
            {
                yield return node;
                foreach (var sub in node.m_nodes)
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
        if ((m_bounds.Height * m_bounds.Width) <= Tree.MinNodeSize)
            return;

        var halfWidth = (m_bounds.Width / 2f);
        var halfHeight = (m_bounds.Height / 2f);

        m_nodes.Add(new QuadTreeNode(new RectangleViewModel(m_bounds.Location, new Size(halfWidth, halfHeight)),
            Tree));
        m_nodes.Add(new QuadTreeNode(
            new RectangleViewModel(new Vector(m_bounds.Left, m_bounds.Top + halfHeight),
                new Size(halfWidth, halfHeight)), Tree));
        m_nodes.Add(new QuadTreeNode(
            new RectangleViewModel(new Vector(m_bounds.Left + halfWidth, m_bounds.Top),
                new Size(halfWidth, halfHeight)), Tree));
        m_nodes.Add(new QuadTreeNode(
            new RectangleViewModel(new Vector(m_bounds.Left + halfWidth, m_bounds.Top + halfHeight),
                new Size(halfWidth, halfHeight)), Tree));
    }
}