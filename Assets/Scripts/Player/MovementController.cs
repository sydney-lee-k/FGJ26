using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 4.0f;

    [Header("Ground Settings")]
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private float stickToGroundForce = 5.0f;
    [Space]
    [SerializeField] private LayerMask groundLayer = ~0;
    [SerializeField] private float rayLength = 0.1f;
    [SerializeField] private float raySphereRadius = 0.1f;

    private CharacterController controller;
    private Camera cam;
    private Health health;

    private RaycastHit hitInfo;
    private float finalRayLength;

    private Vector3 finalMoveDirection;
    private Vector3 velocity;
    private bool isGrounded;

    private Vector3 camForward;
    private Vector3 camRight;

    public float KillHeight = -10f;
    public Vector3 WorldMoveDirection => finalMoveDirection;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        cam = Camera.main;

        finalRayLength = rayLength + controller.center.y;
    }

    private void Update()
    {
        if (transform.position.y <= KillHeight)
        {
            health.Kill();
        }

        CheckIfGrounded();
        CalculateMovement();

        ApplyGravity();
        ApplyMovement();
    }

    private void LateUpdate()
    {
        UpdateCameraDirection();
    }

    private void CheckIfGrounded()
    {
        Vector3 origin = transform.position + controller.center;

        isGrounded = Physics.SphereCast(
            origin,
            raySphereRadius,
            Vector3.down,
            out hitInfo,
            finalRayLength,
            groundLayer
        );

#if UNITY_EDITOR
        Debug.DrawRay(origin, Vector3.down * finalRayLength, isGrounded ? Color.green : Color.red);
#endif
    }

    private void UpdateCameraDirection()
    {
        camForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        camRight = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
    }

    private void CalculateMovement()
    {
        Vector3 moveInput = inputReader.InputVector;

        if (moveInput.sqrMagnitude < 0.0001f)
        {
            finalMoveDirection = Vector3.zero;
            velocity.x = 0f;
            velocity.z = 0f;
            return;
        }

        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        Vector3 desiredDirection = camForward * moveInput.y + camRight * moveInput.x;

        if (isGrounded)
            desiredDirection = Vector3.ProjectOnPlane(desiredDirection, hitInfo.normal);

        finalMoveDirection = desiredDirection.normalized;

        velocity.x = finalMoveDirection.x * walkingSpeed;
        velocity.z = finalMoveDirection.z * walkingSpeed;
    }

    private void ApplyGravity()
    {
        // If grounded, add a little bit of extra downward force just in case.
        if (isGrounded)
        {
            velocity.y = -stickToGroundForce;
        }
        else
        {
            velocity.y += gravityMultiplier * Physics.gravity.y * Time.deltaTime;
        }
    }

    private void ApplyMovement()
    {
        controller.Move(velocity * Time.deltaTime);
    }
}