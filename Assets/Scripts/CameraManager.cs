using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _idleCam;
    [SerializeField] private CinemachineVirtualCamera _followCam;

    public void Awake()
    {
        SwictToIdleCam();

    }

    public void SwictToIdleCam()
    {
        _idleCam.enabled = true;
        _followCam.enabled = false;
    }

    public void SwitchToFollowCam(Transform followTransform)
    {
        _followCam.Follow = followTransform;
        _idleCam.enabled = false;
        _followCam.enabled = true;
    }
}
