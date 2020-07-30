using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public static CameraControl CamControl;
    public bool ControlEnabled = true;
    public float TSpeed; //Traverse Speed
    public float RSpeedX; //Rotation Speed X and Y
    public float RSpeedY;
    float ZSpeed; //Zoom Speed
    public float ZSpeedMultiplier; //Zoom Speed multiplier

    GameObject CamFocal;
    GameObject CamFocalX;
    // Use this for initialization
    void Start () {
        CamFocal = transform.parent.parent.gameObject;
        CamFocalX = transform.parent.gameObject;
        CamControl = this;
	}

	void Update () {

        ZSpeed = transform.position.z * ZSpeedMultiplier;

        if (ControlEnabled && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            //Traverse
            Vector2 movementInput = Controller.InputControls.InGame.CameraMovement.ReadValue<Vector2>() * TSpeed;
            CamFocal.transform.Translate(new Vector3(movementInput.x, 0, movementInput.y));

            //TODO: Update to new input system
            //Zoom
            //if (Input.GetAxis("Mouse ScrollWheel") != 0)
            //{
            //    transform.Translate(0, 0, -Input.GetAxis("Mouse ScrollWheel") * ZSpeed);
            //}

            //Rotate; vertical clamped.
            if (Controller.InputControls.InGame.CameraRotateEnable.ReadValue<float>() > 0)
            {
                Vector2 mouseInput = Controller.InputControls.InGame.CameraRotation.ReadValue<Vector2>();
                CamFocal.transform.Rotate(0, mouseInput.x * RSpeedX, 0, Space.World);
                Vector3 FocalAngle = CamFocalX.transform.eulerAngles;
                CamFocalX.transform.eulerAngles = new Vector3(Mathf.Clamp(FocalAngle.x + -mouseInput.y * RSpeedY, 275, 355), FocalAngle.y, FocalAngle.z);
            } 
        }
    }
}
