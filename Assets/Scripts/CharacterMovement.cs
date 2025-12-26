using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    private PlayerAnimatorController anim;
    public float moveSpeed = 5f;
    private InputReader inputReader;
    private CharacterController controller;
    private Vector3 input;

    [SerializeField] private bool isGrounded = false;

    private float _groundedOffset = -0.14f;
    [SerializeField] private float _gravityMultiplier = 2f;
    [SerializeField] private LayerMask _groundLayerMask;

    private Vector3 targetVelocity;
    public bool enableMove = false;

    private void Awake()
    {
        // DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        inputReader = GetComponent<InputReader>();
        anim = GetComponent<PlayerAnimatorController>();
        EnableMovement(true);
    }

    public void EnableMovement(bool enable)
    {
        enableMove = enable;
        inputReader.enabled = enable;
    }

    void Update()
    {
        if (!enableMove)
        {
            return;
        }

        Vector3 isometricMovement = new Vector3(inputReader._moveComposite.x, 0, inputReader._moveComposite.y);

        float horizontal = inputReader._moveComposite.x;
        float vertical = inputReader._moveComposite.y;

        //No jump, force player to ground
        ApplyGravity();
        if (horizontal != 0 || vertical != 0)
        {
            anim.SetState(1);
            Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
            targetVelocity = new Vector3(moveDirection.x, targetVelocity.y, moveDirection.z);

            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 9f);
        }
        else
        {
            anim.SetState(0);
        }
        GroundedCheck();
    }

    private void ApplyGravity()
    {
        targetVelocity.y += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
        Vector3 grav = new Vector3(0, targetVelocity.y, 0);
        controller.Move(grav * Time.deltaTime);
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(
            controller.transform.position.x,
            controller.transform.position.y - _groundedOffset,
            controller.transform.position.z
        );
        isGrounded = Physics.CheckSphere(spherePosition, controller.radius, _groundLayerMask, QueryTriggerInteraction.Ignore);
    }

}
