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
	
	// Update is called once per frame
	void Update () {

        ZSpeed = transform.position.z * ZSpeedMultiplier;

        if (ControlEnabled)
        {
            //Traverse
            if (Input.GetAxis("Horizontal") != 0)
            {
                CamFocal.transform.Translate(Input.GetAxis("Horizontal") * TSpeed, 0, 0);
            }
            if (Input.GetAxis("Vertical") != 0)
            {
                CamFocal.transform.Translate(0, 0, Input.GetAxis("Vertical") * TSpeed);
            }

            //Zoom
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                transform.Translate(0, 0, -Input.GetAxis("Mouse ScrollWheel") * ZSpeed);
            }

            //Rotate; vertical clamped.
            if (Input.GetKey(KeyCode.Mouse2))
            {

                CamFocal.transform.Rotate(0, Input.GetAxis("Mouse X") * RSpeedX, 0, Space.World);
                Vector3 FocalAngle = CamFocalX.transform.eulerAngles;
                CamFocalX.transform.eulerAngles = new Vector3(Mathf.Clamp(FocalAngle.x + -Input.GetAxis("Mouse Y") * RSpeedY, 275, 355), FocalAngle.y, FocalAngle.z);
            } 
        }
    }
}
