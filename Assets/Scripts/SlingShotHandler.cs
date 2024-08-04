using DG.Tweening;
using System.Collections;
using UnityEngine;

// Class for handling the SlingShot object
public class SlingShotHandler : MonoBehaviour
{

    [Header("Line Renderers")]
    [SerializeField] private LineRenderer _leftLineRenderer; // LineRenderer for the left sling shot string
    [SerializeField] private LineRenderer _rightLineRenderer; // LineRenderer for the right sling shot string

    [Header("Transform Refs")]
    [SerializeField] private Transform _leftStartingPosition; // Transform for the left anchor point
    [SerializeField] private Transform _rightStartingPosition; // Transform for the right anchor point
    [SerializeField] private Transform _centerPosition; // Transform representing the center position of the slingshot
    [SerializeField] private Transform _idlePosition; // Transform representing the idle position of the slingshot
    [SerializeField] private Transform _elasticTransform;

    [Header("SlingShot Stats")]
    [SerializeField] private float _maxDistance; // Maximum distance the slingshot string can be pulled
    [SerializeField] private float _shotForce = 9f;
    [SerializeField] private float _timeBeforeNextBird = 2f;
    [SerializeField] private float _elasticDevider;
    [SerializeField] private AnimationCurve _elasticCurve;
    [SerializeField] private float _maxAnimationTime = 1f;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea _slingShotArea;
    [SerializeField] private CameraManager _cameraManager;

    [Header("Bird")]
    [SerializeField] private AngryBird _angryBirdPrefab;
    [SerializeField] private float _angryBirdPositionOffset = 0.285f;

    [Header("Sounds")]
    [SerializeField] private AudioClip _elasticPulledClip;
    [SerializeField] private AudioClip[] _elasticReleasedClips;

    private AudioSource _audioSource;

    private AngryBird _spawnedAngryBird;

    private Vector2 _slingShotLinesPosition; // Current position of the slingshot strings
    private Vector2 _direction;
    private Vector2 _directionNormalized;


    private bool _clickedWithArea;
    private bool _birdOnSlingShot;

    //Run before anything
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        //disable lines at start
        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;

        SpawnAngryBird();

    }

    // Update is called once per frame
    private void Update()
    {

        if (InputManager.WasLeftMouseButtonPresses && _slingShotArea.IsWithinSlingShotArea())
        {
            _clickedWithArea = true;

            if (_birdOnSlingShot)
            {
                SoundManager.instance.PlayClip(_elasticPulledClip, _audioSource);
                _cameraManager.SwitchToFollowCam(_spawnedAngryBird.transform);
            }

        }

        // If the left mouse button is pressed, draw the slingshot strings
        if (InputManager.IsLeftMouseButtonPressed && _clickedWithArea && _birdOnSlingShot)
        {
            DrawSlingShot();
            PositionAndRotateAngryBird();  //Initially helps the bird keep up with the slingshot
        }

        if (InputManager.WasLeftMouseButtonReleased && _birdOnSlingShot && _clickedWithArea)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                _clickedWithArea = false;
                _birdOnSlingShot = false;

                _spawnedAngryBird.LaunchBird(_direction, _shotForce);

                SoundManager.instance.PlayRandom(_elasticReleasedClips, _audioSource);

                GameManager.instance.UseShot();
                //SetLines(_centerPosition.position);
                AnimateSlingShot();
                //prevent spawn after this shot puts us at the limit
                if (GameManager.instance.HasEnoughShots())
                {
                    StartCoroutine(SpawnAngryBirdAfterTime());
                }

            }
        }
    }

    #region SlingShot Methods

    private void DrawSlingShot()

    {

        // Get the current mouse position in world space
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePositon);

        // Calculate the position of the slingshot lines, clamped to the maximum distance
        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position, _maxDistance);

        // Update the positions of the slingshot strings
        SetLines(_slingShotLinesPosition);

        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector3 position)
    {
        //enable the line renderers if not enabled already
        if (!_rightLineRenderer.enabled && !_leftLineRenderer.enabled)
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }

        // Set the start position of the left sling shot string to the mouse position
        _leftLineRenderer.SetPosition(0, position);
        // Set the end position of the left sling shot string to the left anchor point
        _leftLineRenderer.SetPosition(1, _leftStartingPosition.position);

        // Set the start position of the right sling shot string to the mouse position
        _rightLineRenderer.SetPosition(0, position);
        // Set the end position of the right sling shot string to the right anchor point
        _rightLineRenderer.SetPosition(1, _rightStartingPosition.position);
    }

    #endregion

    #region AngryBird Methods

    public void SpawnAngryBird()
    {
        _elasticTransform.DOComplete();
        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPostion = (Vector2)_idlePosition.position + dir * _angryBirdPositionOffset;
        SetLines(_idlePosition.position);
        _spawnedAngryBird = Instantiate(_angryBirdPrefab, spawnPostion, Quaternion.identity);
        _spawnedAngryBird.transform.right = dir;

        _birdOnSlingShot = true;
    }

    private void PositionAndRotateAngryBird()
    {
        _spawnedAngryBird.transform.position = _slingShotLinesPosition + _directionNormalized * _angryBirdPositionOffset;
        _spawnedAngryBird.transform.right = _directionNormalized;


    }

    private IEnumerator SpawnAngryBirdAfterTime()
    {
        yield return new WaitForSeconds(_timeBeforeNextBird);
        SpawnAngryBird();
        _cameraManager.SwictToIdleCam();
    }

    #endregion

    #region Animate SlingShot

    private void AnimateSlingShot()
    {
        _elasticTransform.position = _leftLineRenderer.GetPosition(0);

        float dist = Vector2.Distance(_elasticTransform.position, _centerPosition.position);
        float time = dist / _elasticDevider;

        _elasticTransform.DOMove(_centerPosition.position, time).SetEase(_elasticCurve);
        StartCoroutine(AnimateSlingShotLines(_elasticTransform, time));

    }

    private IEnumerator AnimateSlingShotLines(Transform trans, float time)
    {

        float elapsedTime = 0f;

        while (elapsedTime < time && elapsedTime < _maxAnimationTime)
        {
            elapsedTime += Time.deltaTime;
            SetLines(trans.position);
            yield return null;
        }
    }

    #endregion

}
