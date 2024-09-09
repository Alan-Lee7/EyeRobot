using UnityEngine;
using UnityEngine.AI;

public class Pathfind : MonoBehaviour
{
    [SerializeField] Transform target;
    NavMeshAgent agent;
    private Vector3 lastPosition;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        lastPosition = agent.transform.position;
    }

    void Update()
    {
    agent.SetDestination(target.position);
if (pathComplete())
{
    //At the target destination
}
else
{
    Vector3 movementDirection = agent.transform.position - lastPosition;
    lastPosition = agent.transform.position;
    movementDirection.Normalize();
    Vector3 forward = agent.transform.forward;
    Vector3 right = agent.transform.right;
    Vector3 up = agent.transform.up;
    
    float forwardDot = Vector3.Dot(forward, movementDirection);
    float rightDot = Vector3.Dot(right, movementDirection);
    float upDot = Vector3.Dot(up, movementDirection);

    if (Mathf.Abs(forwardDot) > 0.7f)
    {
        if (forwardDot > 0.7f)
        {
            if (Mathf.Abs(rightDot) > 0.7f)
            {
                if (rightDot > 0.7f)
                {
                    Debug.Log("Moving Forward-Right");
                }
                else
                {
                    Debug.Log("Moving Forward-Left");
                }
            }
            else if (Mathf.Abs(upDot) > 0.7f)
            {
                if (upDot > 0.7f)
                {
                    Debug.Log("Moving Forward-Up");
                }
                else
                {
                    Debug.Log("Moving Forward-Down");
                }
            }
            else
            {
                Debug.Log("Moving Forward");
            }
        }
        else if (forwardDot < -0.7f)
        {
            if (Mathf.Abs(rightDot) > 0.7f)
            {
                if (rightDot > 0.7f)
                {
                    Debug.Log("Moving Backward-Right");
                }
                else
                {
                    Debug.Log("Moving Backward-Left");
                }
            }
            else if (Mathf.Abs(upDot) > 0.7f)
            {
                if (upDot > 0.7f)
                {
                    Debug.Log("Moving Backward-Up");
                }
                else
                {
                    Debug.Log("Moving Backward-Down");
                }
            }
            else
            {
                Debug.Log("Moving Backward");
            }
        }
    }
    else
    {
        if (Mathf.Abs(rightDot) > 0.7f)
        {
            if (rightDot > 0.7f)
            {
                Debug.Log("Moving Right");
            }
            else
            {
                Debug.Log("Moving Left");
            }
        }
        else if (Mathf.Abs(upDot) > 0.7f)
        {
            if (upDot > 0.7f)
            {
                Debug.Log("Moving Up");
            }
            else
            {
                Debug.Log("Moving Down");
            }
        }
    }
}

    }

    private bool pathComplete()
    {
        return !agent.hasPath && agent.velocity.sqrMagnitude <= 0.01f;
    }

}
