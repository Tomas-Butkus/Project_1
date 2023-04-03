using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Options:")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed = 0.1f;
    [SerializeField] private bool followCamera = false;
    [SerializeField] private bool controlCamera = true;

    private void Update()
    {
        CheckCameraModeInput();
    }

    private void FixedUpdate()
    {
        if(controlCamera && !followCamera)
        {
            MoveCamera();
        }
        else if(followCamera && !controlCamera)
        {
            FollowTarget();
        }
    }

    private void MoveCamera()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        transform.position += new Vector3(xInput, yInput, 0) * smoothSpeed * Time.deltaTime;
    }

    private void FollowTarget()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }

    private void CheckCameraModeInput()
    {
        if(Input.GetKeyDown(KeyCode.F) && controlCamera)
        {
            followCamera = true;
            controlCamera = false;
        }
        else if(Input.GetKeyDown(KeyCode.F) && followCamera)
        {
            followCamera = false;
            controlCamera = true;
        }
    }
}
