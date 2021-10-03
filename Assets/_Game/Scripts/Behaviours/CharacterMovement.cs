using _Game.Scripts.Behaviours;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    // Start is called before the first frame update

    private CharacterController _characterController;
    [SerializeField] private ElementContainerComponent elementContainerComponent;
    [SerializeField] Animator animator;
    [Space(20)]
    [ReadOnly]
    [SerializeField] private MovementState _movementState = MovementState.Walking;
    [SerializeField] private float _slowedSpeed;
    [SerializeField] private float _slowedTime;
    [SerializeField] private float _stunnedTime;
    [SerializeField] private float _standardSpeed;
    [ReadOnly]
    [SerializeField] private Vector3 movementDirection;
    [Space(20)]
    [SerializeField] public bool canDash;
    [ReadOnly]
    [SerializeField] private bool dashFlag;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime;
    [ReadOnly]
    [SerializeField] private float _dashCooldownTimer;
    [SerializeField] private float _dashCooldown;
    [Space(20)]
    [ReadOnly]
    [SerializeField] private float _stateTime = 0f;
    [SerializeField] private Transform _animTargetPivot;
    public AnimationCurve curve;
    [SerializeField] public float wiggle;
    [Space(20)]
    [ReadOnly]
    [SerializeField] private bool dropFlag;

    private float startY = 0;

    public enum MovementState
    {
        Walking,
        Slowed,
        Stunned,
        Dashing
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        startY = transform.position.y;
    }

    private void FixedUpdate()
    {
        if (dashFlag)
        {
            dashFlag = false;
            SwitchState(MovementState.Dashing);
            animator.SetTrigger("DashTrigger");
            elementContainerComponent.DashDrop();
        }
        if (dropFlag)
        {
            elementContainerComponent.Drop();
            dropFlag = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            dashFlag = true;
            canDash = false;
            _dashCooldownTimer = _dashCooldown;
        }
        if (_dashCooldownTimer > 0)
        {
            _dashCooldownTimer -= Time.deltaTime;
        }
        else
        {
            canDash = true;
        }


        //DropItem
        if (Input.GetKeyDown(KeyCode.R))
        {
            dropFlag = true;
        }
        //SlowTest
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchState(MovementState.Slowed);
        }
        //StunnedTest
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchState(MovementState.Stunned);
        }

        var movementDirection = UpdateMovementPosition();
        RotateToDirection(movementDirection);
    }


    private Vector3 UpdateMovementPosition()
    {
        var horizontalMovement = Input.GetAxis("Horizontal");
        var verticalMovement = Input.GetAxis("Vertical");

        movementDirection = new Vector3(horizontalMovement, 0, verticalMovement);
        movementDirection = Vector3.ClampMagnitude(movementDirection, 1);

        if (movementDirection == Vector3.zero)
            canDash = false;
        else
            canDash = true;

        Vector3 animDir = movementDirection;

        movementDirection = movementDirection * GetCurrentMovementSpeed() * 0.01f;
        movementDirection += Physics.gravity * Time.deltaTime;

        _characterController.Move(movementDirection);

        //stackAnimation
        _animTargetPivot.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(0, -15, animDir.magnitude), 0, wiggle));

        return movementDirection;
    }

    public void SwitchState(MovementState _to)
    {
        _movementState = _to;
        _stateTime = 0f;
    }

    private float GetCurrentMovementSpeed()
    {
        switch (_movementState)
        {
            case MovementState.Slowed:
                float _sSpeed = Mathf.Lerp(_slowedSpeed, _standardSpeed, curve.Evaluate(Mathf.Clamp01(_stateTime / _slowedTime)));
                _stateTime += Time.deltaTime;
                if (_stateTime > _slowedTime)
                    SwitchState(MovementState.Walking);

                return _sSpeed;

            case MovementState.Stunned:
                _stateTime += Time.deltaTime;
                if (_stateTime > _stunnedTime)
                    SwitchState(MovementState.Walking);

                return 0f;

            case MovementState.Dashing:
                float _dSpeed = Mathf.Lerp(_dashSpeed, _standardSpeed, curve.Evaluate(Mathf.Clamp01(_stateTime / _dashTime)));
                _stateTime += Time.deltaTime;

                if (_stateTime > _dashTime)
                    SwitchState(MovementState.Walking);

                return _dSpeed;

            case MovementState.Walking:
            default:
                return _standardSpeed;
        }
    }


    private void RotateToDirection(Vector3 movementDirection)
    {
        Vector3 direction = movementDirection.normalized;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 8);
        }
    }

}
