using Godot;

namespace AstroRaider2.Utility.NodeTree;

public static class NodeUtilities
{

    public static TLocalNode FindUniqueNode<TLocalNode>(Node currentNode, ulong nodeIdentifier) where TLocalNode : Node
    {
        return _InternalRecursiveFindUniqueNode<TLocalNode>(currentNode, nodeIdentifier);
    }
    
    public static TLocalNode FindUniqueNode<TLocalNode>(Node currentNode, string uniqueNodeName) where TLocalNode : Node
    {
        string searchName = uniqueNodeName;
        if (searchName.StartsWith("@"))
        {
            searchName = searchName.Substring(1);
        }

        return _InternalRecursiveFindUniqueNode<TLocalNode>(currentNode, searchName);
    }
    
    private static TLocalNode _InternalRecursiveFindUniqueNode<TLocalNode>(Node currentNode, string uniqueNodeName) where TLocalNode : Node
    {
        if (string.Equals(currentNode.Name, uniqueNodeName))
        {
            if (currentNode is TLocalNode returnNode)
            {
                return returnNode;
            }
            
            GD.PrintErr($"Found a node with the given name {uniqueNodeName}, but it's type is not correct.");
        }
        
        foreach (var nextNode in currentNode.GetChildren())
        {
            TLocalNode returnNode = _InternalRecursiveFindUniqueNode<TLocalNode>(nextNode, uniqueNodeName);
            if (returnNode != null)
            {
                return returnNode;
            }
        }

        return null;
    }
    
    private static TLocalNode _InternalRecursiveFindUniqueNode<TLocalNode>(Node currentNode, ulong uniqueNodeId) where TLocalNode : Node
    {
        
        if (string.Equals(currentNode.GetInstanceId(), uniqueNodeId))
        {
            if (currentNode is TLocalNode returnNode)
            {
                return returnNode;
            }
            
            GD.PrintErr($"Found a node with the given id {uniqueNodeId}, but it's type is not correct.");
        }
        
        foreach (var nextNode in currentNode.GetChildren())
        {
            TLocalNode returnNode = _InternalRecursiveFindUniqueNode<TLocalNode>(nextNode, uniqueNodeId);
            if (returnNode != null)
            {
                return returnNode;
            }
        }

        return null;
    }

}