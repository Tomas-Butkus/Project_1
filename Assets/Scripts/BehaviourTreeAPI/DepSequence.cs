public class DepSequence : BTNode
{
    BehaviourTree dependancy;
    public DepSequence(string n, BehaviourTree d)
    {
        name = n;
        dependancy = d;
    }

    public override Status Process()
    {
        if (dependancy.Process() == Status.FAILURE)
        {
            //agent.ResetPath();
            // Reset all children
            foreach (BTNode n in children)
            {
                n.Reset();
            }
            return Status.FAILURE;
        }

        Status childstatus = children[currentChild].Process();
        if (childstatus == Status.RUNNING) return Status.RUNNING;
        if (childstatus == Status.FAILURE)
            return childstatus;

        currentChild++;
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.SUCCESS;
        }

        return Status.RUNNING;
    }
}
