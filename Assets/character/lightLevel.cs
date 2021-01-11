using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightLevel : MonoBehaviour
{
    public float maxLightSize = 50.0f;
    public float minLightSize = 10.0f;
    public Transform fullLight;
    public Transform halfLight;
    
    void Start() {
        fullLight.localScale = new Vector3(30.0f, 30.0f, 1.0f);
        halfLight.localScale = new Vector3(40.0f, 40.0f, 1.0f);
    }

    void Update() {
        
    }
}
