using UnityEngine;

public class AngryBird : MonoBehaviour
{

    [SerializeField] private AudioClip _hitClip;
    //create a rigidbody2d comp var and assign it to our comp
    private Rigidbody2D _rb;
    private CircleCollider2D _circleColirder;
    private bool _shouldFaceVelocityDirection;

    private bool _hasBeenLaunched;

    private AudioSource _audioSource;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _circleColirder = GetComponent<CircleCollider2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _rb.isKinematic = true; //remove gravity
        _circleColirder.enabled = false; //disabless collision

    }

    private void FixedUpdate()
    {
        if (_hasBeenLaunched && _shouldFaceVelocityDirection)
        {
            transform.right = _rb.velocity;
        }
    }

    public void LaunchBird(Vector2 direction, float force)
    {
        _shouldFaceVelocityDirection = true;

        _rb.isKinematic = false; //applies gravity
        _circleColirder.enabled = true; //applies collision 

        //apply force 

        _rb.AddForce(direction * force, ForceMode2D.Impulse);

        _hasBeenLaunched = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _shouldFaceVelocityDirection = false;
        SoundManager.instance.PlayClip(_hitClip, _audioSource);
        Destroy(this); //destroy script to save resources
    }


}
