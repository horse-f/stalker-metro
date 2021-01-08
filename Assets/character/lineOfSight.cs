using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class lineOfSight : MonoBehaviour {
    public float viewRadius = 25.0f;
    [Range(0,360)] public float viewAngle = 360.0f;
    public LayerMask viewObstaclesMask;
    public MeshFilter meshFilter;
    public float shadowOffset = 1.0f;

    private Mesh viewMesh;
    private List<GameObject> viewObjects;
    
    void Start() {
        viewObjects = utils.GetObjectsInLayer(Mathf.FloorToInt(Mathf.Log(viewObstaclesMask,2)));
        Debug.Log("viewObjects: " + viewObjects.Count);
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        meshFilter.mesh = viewMesh;
    }

    void LateUpdate() {
        foreach(GameObject go in viewObjects) {
            SetRendererEnabled(go, false);
        }
        DrawLineOfSight();
    }

    void DrawLineOfSight() {
        List<Vector2> points = new List<Vector2>();
        foreach(GameObject go in viewObjects) {
            List<Vector2> goPoints = GetPointsToObject(go);
            if(goPoints != null) {
                points.InsertRange(points.Count, goPoints);
            }
        }
    }

    List<Vector2> GetPointsToObject(GameObject go) {
        List<Vector2> points = new List<Vector2>();
        Collider2D coll = go.GetComponent<Collider2D>();
        // how do i get the vertices of the collider
        //  what if the collider is not a square
        return points;
    }

    void SetRendererEnabled(GameObject go, bool enable) {
        Renderer r = go.GetComponent<Renderer>();
        if(r) {
            r.enabled = enable;
        }
    }

}
