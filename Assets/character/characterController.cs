using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class characterController : MonoBehaviour {
    public float accel = 30.0f;
    public float sprintAccel = 60.0f;
    
    private Rigidbody2D rigidbody2d;
    private bool sprinting = false;

    private void Start() {
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update() {
        input();
    }

    private void FixedUpdate() {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Move(moveInput);
        if(rigidbody2d.velocity.magnitude == 0) {
            sprinting = false;
        }
    }

    private void input() {
        if(Input.GetButtonDown("Inventory")) {
            // implement this
            Debug.Log("Inventory");
        } 
        if(Input.GetButtonDown("Map")) {
            // implement this
            Debug.Log("Map");
        }
        sprinting = Input.GetButtonDown("Sprint") ? !sprinting : sprinting;
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
        // Debug.Log("force: " + (transform.up + transform.right) * input.normalized * a);
        rigidbody2d.AddForce((transform.up + transform.right) * input.normalized * a);
    }
}
