using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// The following is the camera controller script for my capstone project "minutes to map"
/// </summary>
public class CameraController : MonoBehaviour
{

    //The following variables were used for testing/prototyping purposes
    public GameObject TestingUI;
    public TMPro.TextMeshProUGUI DistanceText;
    public TMPro.TextMeshProUGUI MinAngleText;
    public TMPro.TextMeshProUGUI MaxAngleText;
    public Slider MinAngleSlider;
    public Slider MaxAngleSlider;
    public Slider distanceSlider;
    public TMPro.TextMeshProUGUI CameraModeText;

    // The rest of the variables were not for testing purposes

    public GameObject cameraTarget;
    public GameObject cameraPivot;
    [SerializeField]
    Cinemachine.CinemachineVirtualCamera virtualCamera;

    public InterfaceManager interfaceManager;

    private const float SENS_MODIFIER = 20;

    public float followSpeed = 10;
    public float MouseSensitivity = 5;
    public float GamepadSensitivity = 5;

    public float turnSmoothing = .1f;

    public float minAngle = 50;
    public float maxAngle = 65;

    float smoothX;
    float smoothY;
    float smoothxVelocity;
    float smoothyVelocity;

    public float followSmoothing = .1f;
    Vector3 followSmoothVelocity;

    public bool lockon;
    public float lookAngle;
    public float tiltAngle;

    //All of the code in the following region of this script was created so that we could fine tune the type of camera behavior we wanted, and was not used in the final game 
    #region CAMERA_PROTOTYPING
    public enum CameraMode
    {
        Free,
        TopDown,
        Testing,
    };
    CameraMode cameraMode = CameraMode.Free;


    public void SwitchCameraMode()
    {
        if (cameraMode == CameraMode.TopDown)
        {
            TestingUI.SetActive(false);
            minAngle = -10f;
            maxAngle = 65f;
            Cinemachine.CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(Cinemachine.CinemachineCore.Stage.Body);
            if(componentBase is Cinemachine.Cinemachine3rdPersonFollow)
            {
                (componentBase as Cinemachine.Cinemachine3rdPersonFollow).CameraDistance = 15;
            }
            CameraModeText.text = "Camera Mode: Free";
            cameraMode = CameraMode.Free;
        }else if (cameraMode == CameraMode.Free)
        {
            TestingUI.SetActive(true);
            minAngle = -10f;
            maxAngle = 65f;
            Cinemachine.CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(Cinemachine.CinemachineCore.Stage.Body);
            if (componentBase is Cinemachine.Cinemachine3rdPersonFollow)
            {
                (componentBase as Cinemachine.Cinemachine3rdPersonFollow).CameraDistance = 15;
            }
            CameraModeText.text = "Camera Mode: Testing";
            cameraMode = CameraMode.Testing;
        } 
        else if (cameraMode == CameraMode.Testing)
        {
            TestingUI.SetActive(false);
            minAngle = 50f;
            maxAngle = 65f;

            Cinemachine.CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(Cinemachine.CinemachineCore.Stage.Body);
            if (componentBase is Cinemachine.Cinemachine3rdPersonFollow)
            {
                (componentBase as Cinemachine.Cinemachine3rdPersonFollow).CameraDistance = 20;
            }
            CameraModeText.text = "Camera Mode: Top-down";
            cameraMode = CameraMode.TopDown;
        }


    }

    public void HandleDistanceSliderUpdate()
    {
        if (cameraMode == CameraMode.Testing)
        {

            Cinemachine.CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(Cinemachine.CinemachineCore.Stage.Body);
            if (componentBase is Cinemachine.Cinemachine3rdPersonFollow)
            {
                
                if (distanceSlider.value >= 5 && distanceSlider.value <= 30)
                {
                    (componentBase as Cinemachine.Cinemachine3rdPersonFollow).CameraDistance = distanceSlider.value;
                }
                
            }

        }
    }

    public void HandleMaxAngleSlider()
    {
        if (cameraMode == CameraMode.Testing)
        {
            if (MaxAngleSlider.value >= minAngle && MaxAngleSlider.value <= 65)
            {
                maxAngle = MaxAngleSlider.value;
            }
        }
    }
    public void HandleMinAngleSlider()
    {
        if (cameraMode == CameraMode.Testing)
        {
            if (MaxAngleSlider.value >= -5f && MaxAngleSlider.value <= maxAngle)
            {
                minAngle = MaxAngleSlider.value;
            }
        }
    }
   
    private void UpdateTestingTexts()
    {
        if (cameraMode == CameraMode.Testing)
        {
            Cinemachine.CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(Cinemachine.CinemachineCore.Stage.Body);
            if (componentBase is Cinemachine.Cinemachine3rdPersonFollow)
            {
                DistanceText.text = "Distance: " + (componentBase as Cinemachine.Cinemachine3rdPersonFollow).CameraDistance;
            }

            MaxAngleText.text = "Max Angle: " + maxAngle;
            MinAngleText.text = "Min Angle: " + minAngle;

        }

    }
    #endregion CAMERA_PROTOTYPING


    //-----The rest of the code was used in the final game------

    // Update is called once per frame
    void LateUpdate()
    {
        if (InputManager.instance)
        {
            InputManager input = InputManager.instance;
            Vector2 moveV2 = input.GetMouseDelta(MouseSensitivity);
            float v = moveV2.y;
            float h = moveV2.x;
            float targetSpeed = 1;
            Vector2 gamepadMove = input.gamepadAimMove;

            if (gamepadMove.x != 0 || gamepadMove.y != 0)
            {
                gamepadMove.Normalize();
                v = gamepadMove.y;
                h = gamepadMove.x;
                targetSpeed = GamepadSensitivity * Time.deltaTime * SENS_MODIFIER;
            }  
            HandleRotations(v, h, targetSpeed);
        }
        Follow();
        UpdateTestingTexts();
    }

    void Follow()
    {
        Vector3 desiredPosition = cameraPivot.transform.position;
        Vector3 smoothedPosition = Vector3.SmoothDamp(cameraTarget.transform.position, desiredPosition, ref followSmoothVelocity, followSmoothing);
        cameraTarget.transform.position = smoothedPosition;
        cameraTarget.transform.position = cameraPivot.transform.position;
    }

    void HandleRotations( float vertical, float horizontal, float targetspeed)
    {
        if (interfaceManager.isInMenu == false)
        {
            if (turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, horizontal, ref smoothxVelocity, turnSmoothing, float.MaxValue);
                smoothY = Mathf.SmoothDamp(smoothY, vertical, ref smoothyVelocity, turnSmoothing, float.MaxValue);
            }
            else
            {
                smoothX = horizontal;
                smoothY = vertical;
            }
            lookAngle += smoothX * targetspeed;
            tiltAngle -= smoothY * targetspeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            cameraTarget.transform.localRotation = Quaternion.Euler(tiltAngle / 1.5f, lookAngle, 0);
        }


    }
}

