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
    [ReadOnly]
    [SerializeField] private float _currentSpeed;
    [Space(20)]
    [SerializeField] private float _slowedSpeed;
    [SerializeField] private float _slowedTime;
    [Space(20)]
    [SerializeField] private float _stunnedTime;
    [Space(20)]
    [SerializeField] private float _standardSpeed;
    [Space(20)]
    [SerializeField] public bool canDash;
    [ReadOnly]
    [SerializeField] private bool dashFlag;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime;
    [ReadOnly]
    [SerializeField] private float _dashCooldownTimer;
    [SerializeField] private float _dashCooldown;
    [ReadOnly]
    [SerializeField] private float _stateTime = 0f;
    [SerializeField] private Transform _animTargetPivot;
    [SerializeField] private Vector3 movementDirection;
    [SerializeField] public float wiggle;

    [SerializeField] private MovementState _movementState = MovementState.Walking;

    public AnimationCurve curve;

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
    }

    private void FixedUpdate()
    {
        if (dashFlag)
        {
            dashFlag = false;
            SwitchState(MovementState.Dashing);
            _dashCooldownTimer = _dashCooldown;
            animator.SetTrigger("DashTrigger");
            elementContainerComponent.DashDrop();
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
        }
        if (_dashCooldownTimer > 0)
        {
            _dashCooldownTimer -= Time.deltaTime;
        }
        else
        {
            canDash = true;
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

        _characterController.Move(movementDirection * GetCurrentMovementSpeed() * 0.01f);

        //stackAnimation
        _animTargetPivot.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(0, -15, movementDirection.magnitude), 0, wiggle));

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


    Vector3 direction, slerpedDirection;
    Vector3 lastDirection= Vector3.zero;
    [SerializeField] float turnAnimTime;
    float turnTimer = 0f;
    private void RotateToDirection(Vector3 movementDirection)
    {
        direction = movementDirection.normalized;

        if (direction != lastDirection && direction != Vector3.zero)
        {
            lastDirection = direction;
            turnTimer = 0f;
        }

        slerpedDirection = Vector3.Slerp(lastDirection, direction, Mathf.Clamp01(turnTimer / turnAnimTime));

        Vector3 rotateTowards = transform.position + slerpedDirection;

        Debug.DrawLine(transform.position, rotateTowards, Color.blue);



        turnTimer += Time.deltaTime;
        transform.LookAt(transform.position + movementDirection);
    }

}
