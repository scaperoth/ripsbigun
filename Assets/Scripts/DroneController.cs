using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [SerializeField]
    float _droneMoveSpeed = 1.5f;
    [SerializeField]
    float _floatSpeed = 1f;
    [SerializeField]
    float _floatMagnitude = 1f;
    [SerializeField]
    float _gravityModifier = 1f;
    [SerializeField]
    float _maxZBounds = 0;
    [SerializeField]
    float _minZBounds = -1.6f;
    [SerializeField]
    float _turningTime = .3f;
    [SerializeField]
    GameObject _player;

    bool _grounded = false;
    bool _turning = false;
    float _lastTurnTime = 0f;
    Vector3 _currentTarget;
    bool _targetSet = false;

    Transform _transform;
    Rigidbody _rb;
    Animator _animator;
    SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentTarget = _player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // initial settings for character:
        // gravity, movement, etc.
        ApplyGravity();
        Vector3 newPosition = FLoat();

        Behavior(newPosition);
    }

    void Behavior(Vector3 currentPos)
    {
        float posX = currentPos.x;
        Vector3 playerPos = _player.transform.position;

        if (_turning)
        {
            if (_lastTurnTime + _turningTime < Time.time)
            {
                _turning = false;
                _animator.SetBool("turn", false);
                _spriteRenderer.flipX = !_spriteRenderer.flipX;
            }
            return;
        }

        if (!_targetSet)
        {
            if (posX < playerPos.x)
            {
                _currentTarget = playerPos + (Vector3.right * 2f);
                _spriteRenderer.flipX = true;
            }
            else
            {
                _currentTarget = playerPos + (Vector3.left * 2f);
                _spriteRenderer.flipX = false;
            }

            _targetSet = true;
            return;
        }


        _currentTarget = new Vector3(_currentTarget.x, _currentTarget.y, playerPos.z);
        float distanceToTarget = Vector3.Distance(currentPos, _currentTarget);
        if (distanceToTarget > .6f)
        {
            Vector3 move = Vector3.MoveTowards(currentPos, _currentTarget, _droneMoveSpeed * Time.deltaTime);
            _transform.position = new Vector3(move.x, currentPos.y, move.z);
        }
        else if (distanceToTarget < .6f)
        {
            _animator.SetBool("turn", true);
            _turning = true;
            _targetSet = false;
            _lastTurnTime = Time.time;
        }
    }

    /// <summary>
    /// apply gravity when character not grounded
    /// </summary>
    void ApplyGravity()
    {
        if (!_grounded)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, -_gravityModifier, _rb.velocity.z);
        }
        else
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        }
    }

    /// <summary>
    /// float character using sine wave and
    /// the transform position
    /// </summary>
    Vector3 FLoat()
    {
        // float
        return _transform.position + (transform.up * Mathf.Sin(Time.time * _floatSpeed) * _floatMagnitude * Time.deltaTime);
    }


    /// <summary>
    /// trigger enter
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        // if you hit the floor plane, you're grounded
        if (other.name == "FloorPlane")
        {
            _grounded = true;
        }
    }

    /// <summary>
    /// trigger exit
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        // if you leave the floor plane, you're not grounded
        if (other.name == "FloorPlane")
        {
            _grounded = false;
        }
    }
}
