using System;
using System.Collections.Generic;
using Godot;

namespace AstroRaider2.Utility.NodeLists;

public static class ListOfNode2dExtensions
{

    public static TNodeType FindClosestNode<TNodeType>(this List<TNodeType> nodeList, Vector2 distanceFromPoint,
        Predicate<TNodeType> predicate = null, double maxDistance = double.MaxValue)
        where TNodeType : Node2D
    {
        TNodeType closestNode = null;
        double closestNodeDistance = Double.MaxValue;

        foreach (var nextNode in nodeList)
        {
            if (predicate == null || predicate(nextNode))
            {
                double distanceTo = distanceFromPoint.DistanceTo(nextNode.GlobalPosition);
                if (distanceTo < closestNodeDistance && distanceTo < maxDistance)
                {
                    closestNodeDistance = distanceTo;
                    closestNode = nextNode;
                }
            }
        }

        return closestNode;
    }
}