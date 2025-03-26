using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    public enum Status { Success, Failure, Running }

    public readonly string name;

    public readonly List<Node> children = new();
    protected int currentChild;

    public Node(string name = "Node")
    {
        this.name = name;
    }

    public void AddChild(Node child) => children.Add(child);

    // Process the next child
    public virtual Status Process() => children[currentChild].Process();

    // Resets the index and iterates all of the children to reset them too
    public virtual void Reset()
    {
        currentChild = 0;
        foreach (var child in children)
        {
            child.Reset();
        }
    }

}
