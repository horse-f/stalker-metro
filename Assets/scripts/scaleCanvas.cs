using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class scaleCanvas : MonoBehaviour {
    [Min(1.0f)]public float standardSize = 30.0f;
    public Camera cam;
    private RawImage canvasImage;

    void Start() {
        canvasImage = gameObject.GetComponent<RawImage>();
        if(cam) {
            float size = cam.orthographicSize / standardSize;
            canvasImage.rectTransform.localScale = new Vector3(size, size, 1.0f);
        }
    }

    void Update() {
        
    }
}
