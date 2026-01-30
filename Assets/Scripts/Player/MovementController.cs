using UnityEngine;

public class MovementController : MonoBehaviour
{
    public InputReader inputReader;

    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 4.0f;
    [SerializeField] private float jumpForce = 10.0f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private float stickToGroundForce = 5.0f;

    [SerializeField] private LayerMask groundLayer = ~0;
    [SerializeField] private float rayLength = 0.1f;
    [SerializeField] private float raySphereRadius = 0.1f;

    private CharacterController m_characterController;

    private RaycastHit m_hitInfo;

    [Space, Header("DEBUG")]
    [SerializeField] private Vector3 m_finalMoveDirection;
    [SerializeField] private Vector3 m_finalMoveVector;

    [Space]
    [SerializeField] public float m_currentSpeed;

    [Space]
    [SerializeField] private float m_finalRayLength;

    [Space]
    public bool m_isGrounded;
    [SerializeField] private bool m_previouslyGrounded;
    [Space]
    [SerializeField] private float m_inAirTimer;

    public float KillHeight = -10f;

    public bool jumpPressed;

    private void Start()
    {
        m_characterController = GetComponent<CharacterController>();

        m_finalRayLength = rayLength + m_characterController.center.y;

        m_isGrounded = true;
        m_previouslyGrounded = true;

        m_inAirTimer = 0f;
    }

    private void Update()
    {
        if (transform.position.y <= KillHeight)
        {
            //GetComponent<Health>().Kill();
        }

        if (m_characterController)
        {
            // Check if the player is grounded.
            CheckIfGrounded();

            // Calculate player movement.
            CalculateDirection();
            CalculateSpeed();
            CalculateFinalMovement();

            // Move the player.
            ApplyGravity();
            ApplyMovement();
        }
    }

    private void CheckIfGrounded()
    {
        // Manually check for grounded because the CharacterController default is less reliable.
        Vector3 t_origin = transform.position + m_characterController.center;
        bool t_hitGround = Physics.SphereCast(t_origin, raySphereRadius, Vector3.down, out m_hitInfo, m_finalRayLength, groundLayer);

        // Draw the groundcheck for convenience.
        Debug.DrawRay(t_origin, Vector3.down * rayLength, Color.red);
        m_isGrounded = t_hitGround;
    }

    private bool CheckIfRoof()
    {
        Vector3 t_origin = transform.position;
        bool t_hitRoof = Physics.SphereCast(t_origin, raySphereRadius, Vector3.up, out _, rayLength, groundLayer);
        return t_hitRoof;
    }

    public void CalculateDirection()
    {
        Vector3 t_desiredDirection = transform.forward;
        Vector3 t_flatDirection = FlattenVectorOnSlopes(t_desiredDirection);

        m_finalMoveDirection = t_flatDirection;
    }

    private Vector3 FlattenVectorOnSlopes(Vector3 t_flattenedVector)
    {
        // Adjust movement on slopes to keep speed consistent.
        if (m_isGrounded)
            t_flattenedVector = Vector3.ProjectOnPlane(t_flattenedVector, m_hitInfo.normal);

        return t_flattenedVector;
    }

    private void CalculateFinalMovement()
    {
        Vector3 t_finalVector = m_currentSpeed * m_finalMoveDirection;

        m_finalMoveVector.x = t_finalVector.x;
        m_finalMoveVector.z = t_finalVector.z;

        if (m_characterController.isGrounded)
            m_finalMoveVector.y += t_finalVector.y;
    }

    private void CalculateSpeed()
    {
        //m_currentSpeed = transform.fo == Vector2.zero ? 0.0f : walkingSpeed;
    }

    private void OnJumpEvent()
    {
        if (m_characterController.isGrounded && jumpPressed == false)
        {
            jumpPressed = true;
        }
    }

    private void HandleJump()
    {
        if (jumpPressed)
        {
            m_finalMoveVector.y = jumpForce;

            m_previouslyGrounded = true;
            m_isGrounded = false;

            jumpPressed = false;
        }
    }

    private void ApplyGravity()
    {
        // If grounded, add a little bit of extra downward force just in case.
        if (m_characterController.isGrounded)
        {
            m_inAirTimer = 0f;
            m_finalMoveVector.y = -stickToGroundForce;

            HandleJump();
        }
        else
        {
            // If collided with a ceiling during air time, stop the player from sticking to the roof.
            if (CheckIfRoof())
                m_finalMoveVector.y = -stickToGroundForce;

            m_inAirTimer += Time.deltaTime;
            m_finalMoveVector += gravityMultiplier * Time.deltaTime * Physics.gravity;
        }
    }

    private void ApplyMovement()
    {
        m_characterController.Move(m_finalMoveVector * Time.deltaTime);
    }
}