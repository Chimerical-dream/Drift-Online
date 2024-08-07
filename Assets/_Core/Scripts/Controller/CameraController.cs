using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private CinemachineVirtualCamera followCam;


    private void Awake()
    {
        CarController.AnnounceLocalPlayer.AddListener(OnAnnounceLocalPlayer);
    }

    private void OnAnnounceLocalPlayer(CarController carController)
    {
        followCam.m_Follow = carController.CameraFollowPoint;
        followCam.m_LookAt = carController.CameraFollowPoint;
    }
}
