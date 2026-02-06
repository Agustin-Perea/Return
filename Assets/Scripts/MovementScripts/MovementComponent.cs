using UnityEngine;
using UnityEngine.InputSystem;
public class MovementComponent : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D _rb2D;
    private Animator _animator;

    private Vector2 _moveDirection = Vector2.zero;

    public InputActionReference moveAction;
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
        _moveDirection = moveAction.action.ReadValue<Vector2>();

        _rb2D.linearVelocity = _moveDirection * speed;
        _updateAnimator();
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
