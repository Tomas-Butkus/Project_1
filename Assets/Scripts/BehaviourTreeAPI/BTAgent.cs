using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BTAgent : MonoBehaviour
{
    protected PathfindingGrid pathfindingGrid;
    protected Pathfinding pathfinding;
    protected Rigidbody2D rigidbody2D;

    private Animator animator;

    [SerializeField] private float movementSpeed = 1f;

    private Coroutine pathfindingCoroutine;
    private List<PathfindingNode> pathfindingNodes = null;

    public BehaviourTree tree;

    public enum ActionState { IDLE, WORKING };
    public ActionState state = ActionState.IDLE;

    public BTNode.Status treeStatus = BTNode.Status.RUNNING;

    private WaitForSeconds waitForSeconds; // processing delay of behaviour tree leaves

    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        pathfinding = GetComponent<Pathfinding>();
        pathfindingGrid = FindObjectOfType<PathfindingGrid>();
        tree = new BehaviourTree();
        waitForSeconds = new WaitForSeconds(0.5f);
        StartCoroutine("Behave");
    }

    public BTNode.Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, transform.position);

        if (state == ActionState.IDLE)
        {
            pathfinding.FindPath(transform.position, destination);
            pathfindingNodes = pathfinding.pathfindingGrid.GetComponent<PathfindingGrid>().path;

            pathfindingCoroutine = StartCoroutine(FollowNodes(pathfindingNodes));
            animator.SetFloat("Speed", 1);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(pathfinding.pathfindingGrid.GetComponent<PathfindingGrid>().path.LastOrDefault().worldPosition, destination) > 1)
        {
            state = ActionState.IDLE;
            return BTNode.Status.FAILURE;
        }
        else if(distanceToTarget <= 1)
        {
            rigidbody2D.velocity = new Vector2(0, 0);
            pathfindingNodes = null;
            StopCoroutine(pathfindingCoroutine);

            animator.SetFloat("Speed", 0);
            state = ActionState.IDLE;
            return BTNode.Status.SUCCESS;
        }
        return BTNode.Status.RUNNING;
    }

    public BTNode.Status PerformRepair()
    {
        return BTNode.Status.SUCCESS;
    }

    private IEnumerator FollowNodes(List<PathfindingNode> nodeList)
    {
        foreach (PathfindingNode node in nodeList)
        {
            Vector3 direction = node.worldPosition - transform.position;

            while (Vector3.Distance(node.worldPosition, transform.position) >= 0.01f)
            {
                rigidbody2D.velocity = direction * movementSpeed;
                ChangeDirection();

                yield return null;
            }
        }
    }

    private void ChangeDirection()
    {
        if (rigidbody2D.velocity.x > 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
        }
    }

    private IEnumerator Behave()
    {
        while (true)
        {
            treeStatus = tree.Process();
            yield return waitForSeconds;
        }
    }
}
