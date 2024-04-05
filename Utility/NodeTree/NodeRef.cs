using System;
using Godot;

namespace AstroRaider2.Utility.NodeTree;

public readonly struct NodeRef<TNode> where TNode : Node
{
    private readonly Node _searchScopeNode;
    private readonly NodePath _nodePath;
    private readonly TNode _node;

    public NodeRef(Node searchScopeNode, string nodePath)
    {
        _searchScopeNode = searchScopeNode;
        _nodePath = nodePath;
        _node = null;
        
        if (!nodePath.StartsWith("@"))
        {
            _node = _searchScopeNode.GetNode<TNode>(_nodePath);
        }
        else
        {
            _node = NodeUtilities.FindUniqueNode<TNode>(_searchScopeNode, nodePath.Substring(1));
        }
        
        if (_node == null)
        {
            throw new ArgumentException($"Node not found: {_nodePath}");
        }
    }

    public bool IsValidReference()
    {
        return _node != null;
    }

    public void ExecuteOnValidReference(Action actionFunction)
    {
        if (IsValidReference())
        {
            actionFunction();
        }
    }
    

    public TNode Node => _node;

    public NodePath NodePath => _nodePath;

    public Node SearchScopeNode => _searchScopeNode;

    public TChildNode Child<TChildNode>(string nodePath) where TChildNode : Node
    {
        return Node.GetNode<TChildNode>(nodePath);
    }
}
