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
    private RaycastHit hitInfo;
    private float finalRayLength;

    private Vector3 finalMoveDirection;
    private Vector3 velocity;
    private bool isGrounded;

    public float KillHeight = -10f;
    public Vector3 WorldMoveDirection => finalMoveDirection;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;

        finalRayLength = rayLength + controller.center.y;

        isGrounded = true;
    }

    private void Update()
    {
        if (transform.position.y <= KillHeight)
        {
            GetComponent<Health>().Kill();
        }

        if (controller)
        {
            CheckIfGrounded();
            CalculateMovement();

            ApplyGravity();
            ApplyMovement();
        }
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

        Debug.DrawRay(origin, Vector3.down * finalRayLength, isGrounded ? Color.green : Color.red);
    }

    private void CalculateMovement()
    {
        Vector3 moveInput = inputReader.InputVector;

        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        Vector3 camForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;

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
        if (controller.isGrounded)
        {
            velocity.y = -stickToGroundForce;
        }
        else
        {
            velocity += gravityMultiplier * Time.deltaTime * Physics.gravity;
        }
    }

    private void ApplyMovement()
    {
        controller.Move(velocity * Time.deltaTime);
    }
}