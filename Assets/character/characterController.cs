using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class characterController : MonoBehaviour {
    public float accel = 30.0f;
    public float sprintAccel = 60.0f;
    public float cameraSize = 15.0f;
    public Transform aimPoint = null;
    public float zoomPadding = 2.0f;
    public float zoomMax = 8.0f;
    public float zoomSize = 13.0f;
    public float zoomSpeed = 8.0f;
    
    private Rigidbody2D rigidbody2d;
    private bool sprinting = false;
    private bool aiming = false;
    private bool aimingOnce = false;
    private Camera mainCamera;
    private float zoomRadius = 0.0f;
    // private Vector3 currentZoomPoint;
    private LerpInfo zoomLerp = new LerpInfo();

    private void Start() {
        mainCamera = Camera.main;
        mainCamera.orthographicSize = cameraSize;
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        zoomLerp.speed = zoomSpeed;
    }

    private void Update() {
        GetInput();
        Aim();
        zoomRadius = GetZoomRadius();
    }

    private void FixedUpdate() {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Move(moveInput);
        if(rigidbody2d.velocity.magnitude == 0) {
            sprinting = false;
        }
    }

    private void GetInput() {
        // aiming = Input.GetButtonDown("ADS") ? !aiming : aiming;
        aiming = Input.GetButton("ADS");
        Debug.Log("Aiming: " + aiming);
        sprinting = Input.GetButtonDown("Sprint") ? !sprinting : sprinting;
        if(Input.GetButtonDown("Inventory")) {
            Debug.Log("Inventory");
        } 
        if(Input.GetButtonDown("Map")) {
            Debug.Log("Map");
        }
        if(Input.GetButtonDown("Fire")) {
            Debug.Log("Pew");
        }
        if(aiming) {
            Debug.Log("AimDownSights");
            aimingOnce = true;
            AimDownSights();
        } else if (!aiming && aimingOnce) {
            Debug.Log("ResetAim");
            aimingOnce = false;
            ResetAim();
        }
    }

    private void Aim() {
        Vector3 mousePoint = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        mousePoint.z = 0;
        if(aimPoint) {
            aimPoint.transform.LookAt(mousePoint + Vector3.right * aimPoint.transform.position.z);
            Debug.DrawRay(aimPoint.position, aimPoint.forward * 6, Color.yellow);
        }
    }

    private float GetZoomRadius() {
        // find how to scale the zoom radius based on mouse position scaled to screen size?
        return zoomMax;
    }

    private void Move(Vector2 input) {
        float a = accel;
        if(sprinting) {
            if(Mathf.Abs(input.x) == 0 && Mathf.Abs(input.y) == 0) {
                sprinting = false;
            } else {
                a = sprintAccel;
            }
        }
        rigidbody2d.AddForce((transform.up + transform.right) * input.normalized * a);
    }

    private void AimDownSights() {
        Vector3 zoomPoint = GetZoomPoint();
        zoomPoint.z = -10;
        // if(zoomPoint != zoomLerp.end) {
        //     zoomLerp.end = zoomPoint;
        //     zoomLerp.dist = Vector3.Distance(zoomLerp.start, zoomLerp.end);
        //     zoomLerp.distCovered = 0.0f;
        //     zoomLerp.startTime = Time.time;
        // } else {
        //     zoomLerp.distCovered = (Time.time - zoomLerp.startTime) * zoomLerp.speed;
        // }
        // float frac = zoomLerp.distCovered / zoomLerp.dist;
        // mainCamera.transform.position = Vector3.Lerp(zoomLerp.start, zoomLerp.end, frac);
        mainCamera.transform.position = new Vector3(zoomPoint.x, zoomPoint.y, -10);
        mainCamera.orthographicSize = zoomSize;
    }

    private Vector3 GetZoomPoint() {
        // return new Vector2(0,0);
        return transform.position + aimPoint.forward.normalized * zoomRadius;
    }

    private void ResetAim() {
        // lerp camera back to default position
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        mainCamera.orthographicSize = cameraSize;
    }
}
