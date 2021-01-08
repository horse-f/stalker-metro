using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public struct ViewCastInfo {
    public bool hit;
    public float distance;
    public Vector2 point;
    public float angle;
    public Collider2D collider;

    public ViewCastInfo(bool _hit, float _dist, Vector2 _point, float _angle, Collider2D _collider) {
        hit = _hit;
        point = _point;
        distance = _dist;
        angle = _angle;
        collider = _collider;
    }
}

public class utils {

    public static List<GameObject> GetObjectsInLayer(int layer) {
        List<GameObject> objs = new List<GameObject>();
        Debug.Log("Hello: " + layer);
        foreach(GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
            // Debug.Log("go:" + go.name + ", " + go.layer);
            Debug.Log("> " + go.layer);
            // if(EditorUtility.IsPersistent(go.transform.root.gameObject) && 
            //     !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave) &&
            //     Mathf.Pow(2, go.layer) == layer.value) {
            //     objs.Add(go);
            // }
            if(go.layer == layer) {
                objs.Add(go);
            }
        }
        return objs;
    }

    public static ViewCastInfo ViewCast(Transform transform, float globalAngle, float viewRadius, LayerMask viewObstaclesMask) {
        Vector2 dir = utils.DirFromAngle(transform, globalAngle);
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, dir, viewRadius, viewObstaclesMask);

        if(hit2D.collider) {
            return new ViewCastInfo(true, hit2D.distance, hit2D.point, globalAngle, hit2D.collider);
        } else {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            return new ViewCastInfo(false, viewRadius, pos + dir.normalized * viewRadius, globalAngle, null);
        }
    }

    public static Vector2 DirFromAngle(Transform transform, float globalAngle, bool isGlobal=false) {
        if(!isGlobal) {
            globalAngle += transform.eulerAngles.y;
        }
        return new Vector2(Mathf.Sin(globalAngle * Mathf.Deg2Rad), Mathf.Cos(globalAngle * Mathf.Deg2Rad));
    }
}
