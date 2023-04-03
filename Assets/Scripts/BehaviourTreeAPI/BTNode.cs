using System.Collections.Generic;

public class BTNode
{
    public enum Status { SUCCESS, RUNNING, FAILURE };
    public Status status;
    public List<BTNode> children = new List<BTNode>();
    public int currentChild = 0;
    public string name;
    public int sortOrder;

    public BTNode() { }

    public BTNode(string n)
    {
        name = n;
    }

    public BTNode(string n, int order)
    {
        name = n;
        sortOrder = order;
    }

    public void Reset()
    {
        foreach (BTNode n in children)
        {
            n.Reset();
        }
        currentChild = 0;
    }

    public virtual Status Process()
    {
        return children[currentChild].Process();
    }

    public void AddChild(BTNode n)
    {
        children.Add(n);
    }
}
