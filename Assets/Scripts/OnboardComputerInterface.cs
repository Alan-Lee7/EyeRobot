using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class OnboardComputerInterface : MonoBehaviour
{
    private static String baseUrl = "http://192.168.4.1";
    int JoyX = 100, JoyY = 100, rollAngle = 0, pitchAngle = 0, AccelX = 0, AccelY = 0, AccelZ = 0, ButtonZ = 0, ButtonC = 0;
    [SerializeField] Transform target;
    NavMeshAgent agent;
    private Vector3 lastPosition;
    public GameObject arUser;
    private const int TARGET_DISPLACEMENT_SPEED = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(UpdateLeaseScheduler), 0, 2);
        InvokeRepeating(nameof(SetAgentPosition), 0, 2);
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        lastPosition = agent.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (JoyX < 40)
        {
            // StartCoroutine(SendCommand("left"));
            MoveTargetLeft();
        }

        if (JoyY > 210)
        {
            // StartCoroutine(SendCommand("forward"));
            MoveTargetForward();
        }

        if (JoyX > 210)
        {
            // StartCoroutine(SendCommand("right"));
            MoveTargetRight();
        }

        if (IsPathComplete())
        {
            StartCoroutine(SendCommand("stop"));
        }
        
        if (ButtonZ > 0 || ButtonC > 0)
        {
            target.position = transform.position;
        }
        
        agent.SetDestination(target.position);

        Vector3 movementDirection = agent.transform.position - lastPosition;
        lastPosition = agent.transform.position;
        movementDirection.Normalize();
        Vector3 forward = agent.transform.forward;
        Vector3 right = agent.transform.right;
        Vector3 up = agent.transform.up;

        var angle = Vector3.SignedAngle(movementDirection, agent.transform.forward, agent.transform.up);
        
        if (Math.Abs(angle) <= 22.5)
        {
            // Forward
            StartCoroutine(SendCommand("forward"));
        } 
        else if (Math.Abs(angle) <= 67.5) 
        {
            // Forward-Right or Forward-Left
            if (angle > 0)
                StartCoroutine(SendCommand("forwardright"));
            else
                StartCoroutine(SendCommand("forwardleft"));
        } 
        else if (Math.Abs(angle) <= 112.5)
        {
            // Right or Left
            if (angle > 0)
                StartCoroutine(SendCommand("right"));
            else
                StartCoroutine(SendCommand("left"));
        }
        else if (Math.Abs(angle) <= 157.5)
        {
            // Backward-Right or Backward-Left
            if (angle > 0)
                StartCoroutine(SendCommand("backwardright"));
            else
                StartCoroutine(SendCommand("backwardleft"));
        } 
        else 
        {
            // Backward
            StartCoroutine(SendCommand("backward"));
        }

        // float forwardDot = Vector3.Dot(forward, movementDirection);
        // float rightDot = Vector3.Dot(right, movementDirection);
        // float upDot = Vector3.Dot(up, movementDirection);

        // if (Mathf.Abs(forwardDot) > 0.7f)
        // {
        //     if (forwardDot > 0.7f)
        //     {
        //         if (Mathf.Abs(rightDot) > 0.7f)
        //         {
        //             if (rightDot > 0.7f)
        //             {
        //                 Debug.Log("Moving Forward-Right");
        //             }
        //             else
        //             {
        //                 Debug.Log("Moving Forward-Left");
        //             }
        //         }
        //         else if (Mathf.Abs(upDot) > 0.7f)
        //         {
        //             if (upDot > 0.7f)
        //             {
        //                 Debug.Log("Moving Forward-Up");
        //             }
        //             else
        //             {
        //                 Debug.Log("Moving Forward-Down");
        //             }
        //         }
        //         else
        //         {
        //             Debug.Log("Moving Forward");
        //             StartCoroutine(SendCommand("forward"));
        //         }
        //     }
        //     else if (forwardDot < -0.7f)
        //     {
        //         if (Mathf.Abs(rightDot) > 0.7f)
        //         {
        //             if (rightDot > 0.7f)
        //             {
        //                 Debug.Log("Moving Backward-Right");
        //             }
        //             else
        //             {
        //                 Debug.Log("Moving Backward-Left");
        //             }
        //         }
        //         else if (Mathf.Abs(upDot) > 0.7f)
        //         {
        //             if (upDot > 0.7f)
        //             {
        //                 Debug.Log("Moving Backward-Up");
        //             }
        //             else
        //             {
        //                 Debug.Log("Moving Backward-Down");
        //             }
        //         }
        //         else
        //         {
        //             Debug.Log("Moving Backward");
        //             StartCoroutine(SendCommand("backward"));
        //         }
        //     }
        // }
        // else
        // {
        //     if (Mathf.Abs(rightDot) > 0.7f)
        //     {
        //         if (rightDot > 0.7f)
        //         {
        //             Debug.Log("Moving Right");
        //             StartCoroutine(SendCommand("right"));
        //         }
        //         else
        //         {
        //             Debug.Log("Moving Left");
        //             StartCoroutine(SendCommand("left"));
        //         }
        //     }
        //     else if (Mathf.Abs(upDot) > 0.7f)
        //     {
        //         if (upDot > 0.7f)
        //         {
        //             Debug.Log("Moving Up");
        //         }
        //         else
        //         {
        //             Debug.Log("Moving Down");
        //             StartCoroutine(SendCommand("down"));
        //         }
        //     }
        // }
    }

    private void SetAgentPosition()
    {
        agent.transform.position = arUser.transform.position;
    }

    private void MoveTargetRight()
    {
        Vector3 targetMoveVelocity = TARGET_DISPLACEMENT_SPEED * Vector3.right;
        target.position += targetMoveVelocity * Time.deltaTime;
    }

    private void MoveTargetLeft()
    {
        Vector3 targetMoveVelocity = TARGET_DISPLACEMENT_SPEED * Vector3.left;
        target.position += targetMoveVelocity * Time.deltaTime;
    }

    private void MoveTargetForward()
    {
        Vector3 targetMoveVelocity = TARGET_DISPLACEMENT_SPEED * Vector3.forward;
        target.position += targetMoveVelocity * Time.deltaTime;
    }


    private bool IsPathComplete()
    {
        return !agent.hasPath && agent.velocity.sqrMagnitude <= 0.05f;
    }

    static IEnumerator SendCommand(String command)
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get($"{OnboardComputerInterface.baseUrl}/{command}");
        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();
            
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("HTTP Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log("Received: " + webRequest.downloadHandler.text);
                break;
        }
    }

    void UpdateLeaseScheduler()
    {
        StartCoroutine(UpdateLeash());
    }
    
    IEnumerator UpdateLeash()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get($"{OnboardComputerInterface.baseUrl}/leash");
        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();
            
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("HTTP Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.Success:
                
                String response = webRequest.downloadHandler.text;
                Debug.Log("Received: " + response);

                String[] dataStr = response.Split(',');
                JoyX = int.Parse(dataStr[0]);
                JoyY = int.Parse(dataStr[1]);
                rollAngle = int.Parse(dataStr[2]);
                pitchAngle = int.Parse(dataStr[3]);
                AccelX = int.Parse(dataStr[4]);
                AccelY = int.Parse(dataStr[5]);
                AccelZ = int.Parse(dataStr[6]);
                ButtonZ = int.Parse(dataStr[7]);
                ButtonC = int.Parse(dataStr[8]);
                
                break;
        }
    }

}
