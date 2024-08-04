using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int MaxNumberOfShots = 3;
    private int _usedNumberOfShots;

    private IconHandler _iconHandler;

    private List<Baddie> _baddies = new List<Baddie>();

    [SerializeField] public float _secondsToWaitBeforeDeathCheck = 3f;
    [SerializeField] public GameObject _restartScreenObject;
    [SerializeField] public SlingShotHandler _slingShotHandler;
    [SerializeField] public Image _nextGameImg;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        _iconHandler = FindObjectOfType<IconHandler>();

        Baddie[] baddies = FindObjectsOfType<Baddie>();

        for (int i = 0; i < baddies.Length; i++)
        {
            _baddies.Add(baddies[i]);
        }

        _nextGameImg.enabled = false;

    }

    public void UseShot()
    {
        _usedNumberOfShots++;
        _iconHandler.UseShot(_usedNumberOfShots);
        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {
        if (_usedNumberOfShots < MaxNumberOfShots)
        {
            return true;
        }

        return false;
    }

    private void CheckForLastShot()
    {
        if (_usedNumberOfShots == MaxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(_secondsToWaitBeforeDeathCheck);

        if (_baddies.Count == 0)
        {
            WinGame();
        }

        else
        {

            RestartGame();

        }
    }

    public void RemoveBaddie(Baddie baddie)
    {
        _baddies.Remove(baddie);
        CheckForAllDeadBaddies();
    }

    private void CheckForAllDeadBaddies()
    {
        if (_baddies.Count == 0)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        _restartScreenObject.SetActive(true);
        _slingShotHandler.enabled = false;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels = SceneManager.sceneCountInBuildSettings;

        if (currentSceneIndex + 1 < maxLevels)
        {
            _nextGameImg.enabled = true;
        }
    }

    public void RestartGame()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
