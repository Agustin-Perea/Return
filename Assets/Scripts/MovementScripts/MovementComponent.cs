using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class MovementComponent : MonoBehaviour
{
    public float speed = 5f;
    public bool canMove = true;
    private Rigidbody2D _rb2D;
    private Animator _animator;

    private Vector2 _moveDirection = Vector2.zero;

    public InputActionReference moveAction;
    public UnityEvent movementPerformed;

    public Vector3 lastPosition { get; private set; }

    EventBinding<PlayerDisabeMovement> playerDisableMoveEventBinding;
    EventBinding<PlayerEnableMovement> playerEnableMoveEventBinding;

    void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (canMove) {
            _moveDirection = moveAction.action.ReadValue<Vector2>();

            if (_moveDirection.magnitude > 0f)
            {
                movementPerformed.Invoke();
                lastPosition = transform.position;
            }
            
            _rb2D.linearVelocity = _moveDirection * speed;
            
            
            
            _updateAnimator();
        }

    }
    void OnEnable()
    {
        playerDisableMoveEventBinding = new EventBinding<PlayerDisabeMovement>(StopMove);
        EventBus<PlayerDisabeMovement>.Register(playerDisableMoveEventBinding);

        playerEnableMoveEventBinding = new EventBinding<PlayerEnableMovement>(EnableMove);
        EventBus<PlayerEnableMovement>.Register(playerEnableMoveEventBinding);
    }
    void OnDisable()
    {
        EventBus<PlayerDisabeMovement>.Deregister(playerDisableMoveEventBinding);
        EventBus<PlayerEnableMovement>.Deregister(playerEnableMoveEventBinding);
    }
    public void StopMove()
    {
        canMove = false;
        _animator.speed = 0f;
        _moveDirection = Vector2.zero;
        _rb2D.linearVelocity = Vector2.zero;
    }
    public void EnableMove()
    {
        canMove = true;
        _animator.speed = 1f;
    }

    private void _updateAnimator()
    {
        if (_moveDirection.magnitude > 0)
        {

            _animator.Play("Walk");
            _animator.SetFloat("Axis_X", _moveDirection.x);
            _animator.SetFloat("Axis_Y", _moveDirection.y);
        }else
        {
            _animator.Play("Idle");
        }


    }


}
