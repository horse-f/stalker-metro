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
        foreach(GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
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

    // public static ViewCastInfo ViewCast(Transform transform, float globalAngle, float viewRadius, LayerMask viewObstaclesMask, float depth) {
    //     Vector2 dir = utils.DirFromAngle(transform, globalAngle);
    //     RaycastHit2D hit2D = Physics2D.Raycast(transform.position, dir, viewRadius, viewObstaclesMask);
    //     if(hit2D.collider) {
    //         return new ViewCastInfo(true, hit2D.distance, hit2D.point - hit2D.normal.normalized * depth, globalAngle, hit2D.collider);
    //     }
    //     Vector2 position = new Vector2(transform.position.x, transform.position.y);
    //     return new ViewCastInfo(false, viewRadius, position + dir.normalized * viewRadius, globalAngle, null);
    // }

    public static ViewCastInfo ViewCast(Transform transform, float globalAngle, float viewRadius, LayerMask viewObstaclesMask, float depth) {
        Vector2 dir = utils.DirFromAngle(transform, globalAngle);
        RaycastHit2D hit2d = Physics2D.Raycast(transform.position, dir, viewRadius, viewObstaclesMask);
        if(hit2d.collider) {
            Vector2 pos = new Vector2(hit2d.point.x, hit2d.point.y) + dir.normalized * depth;
            RaycastHit2D backHit = Physics2D.Raycast(pos, -dir, depth, viewObstaclesMask);
            if(backHit.collider) {
                return new ViewCastInfo(
                    true, 
                    Distance(new Vector2(transform.position.x, transform.position.y), backHit.point), 
                    backHit.point, globalAngle, 
                    backHit.collider
                );
            }
        }
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        return new ViewCastInfo(false, viewRadius, position + dir.normalized * viewRadius, globalAngle, null);
    }

    // public static ViewCastInfo ViewCastExit(Transform transform, float globalAngle, float viewRadius, LayerMask viewObstaclesMask, float ignoreDepth = 0.0f) {
    //     Vector2 dir = utils.DirFromAngle(transform, globalAngle);
    //     RaycastHit2D hit2D = getRayExit(transform.position, dir, viewRadius, viewObstaclesMask, ignoreDepth);
    //     if(hit2D.collider) {
    //         return new ViewCastInfo(true, hit2D.distance, hit2D.point, globalAngle, hit2D.collider);
    //     } else {
    //         Vector2 pos = new Vector2(transform.position.x, transform.position.y);
    //         return new ViewCastInfo(false, viewRadius, pos + dir.normalized * viewRadius, globalAngle, null);
    //     }
    // }

    // public static RaycastHit2D getRayExit(Vector2 pos, Vector2 dir, float dist, LayerMask layerMask, float ignoreDepth = 0.0f) {
    //     Vector2 startPoint = pos + dir * dist;
    //     RaycastHit2D frontHit = Physics2D.Raycast(pos, dir, dist, layerMask);
    //     RaycastHit2D[] backHits = Physics2D.RaycastAll(startPoint, -dir, dist, layerMask);
    //     if(backHits.Length > 0) {
    //         if(ignoreDepth == 0.0f) {
    //             return backHits[backHits.Length - 1];
    //         }
    //         for(int i = backHits.Length - 1; i >= 0; i--) {
    //             if(Distance(frontHit.point, backHits[i].point) >= ignoreDepth) {
    //                 return backHits[i];
    //             }
    //         }
    //         return backHits[0];
    //     } else {
    //         return new RaycastHit2D();
    //     }
    // }

    public static Vector2 DirFromAngle(Transform transform, float globalAngle, bool isGlobal=false) {
        if(!isGlobal) {
            globalAngle += transform.eulerAngles.y;
        }
        return new Vector2(Mathf.Sin(globalAngle * Mathf.Deg2Rad), Mathf.Cos(globalAngle * Mathf.Deg2Rad));
    }

    // public static float AngleFromDir3(Transform transform, Vector3 Dir, bool isGlobal=false) {
    //     if(!isGlobal) {
    //         Dir = transform.TransformDirection(Dir);
    //     }
    //     return Mathf.Asin(Dir.x) * Mathf.Rad2Deg;
    // }

    public static float Distance(Vector2 A, Vector2 B) {
        Vector2 BA = B - A;
        return Mathf.Sqrt(BA.x * BA.x + BA.y * BA.y);
    }

    public static float Distance(Vector3 A, Vector3 B) {
        Vector3 BA = B - A;
        return Mathf.Sqrt(BA.x * BA.x + BA.y * BA.y + BA.z * BA.z);
    }
}