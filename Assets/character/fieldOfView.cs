﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ViewCastInfo {
    public bool hit;
    public float distance;
    public Vector2 point;
    public float angle;

    public ViewCastInfo(bool _hit, float _dist, Vector2 _point, float _angle) {
        hit = _hit;
        point = _point;
        distance = _dist;
        angle = _angle;
    }
}

public struct EdgeInfo {
    public Vector2 pointA;
    public Vector2 pointB;

    public EdgeInfo(Vector2 _pa, Vector2 _pb) {
        pointA = _pa;
        pointB = _pb;
    }
}

public struct MeshInfo {
    public int[] triangles;
    public Vector3[] vertices;

    public MeshInfo(int[] _tris, Vector3[] _verts) {
        triangles = _tris;
        vertices = _verts;
    } 
}

public class fieldOfView : MonoBehaviour {
    public float viewRadius = 25.0f;
    [Range(0,360)] public float viewAngle = 360.0f;
    public LayerMask viewObstaclesMask;
    public float meshResolution = 0.5f;
    public float edgeDetectionThreshold = 0.5f;
    public int edgeDetectionIterations = 10;
    public MeshFilter meshFilter;
    public float shadowOffset = 1.0f;

    private Mesh viewMesh;
    // private List<GameObject> viewObstacles;

    void Start() {
        // viewObstacles = FindViewObjects();
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        meshFilter.mesh = viewMesh;
    }

    void LateUpdate() {
        DrawViewField();
    }

    // List<GameObject> FindViewObjects() {
        
    // }

    void DrawViewField() {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngle = viewAngle / stepCount;
        List<Vector2> points = GetViewFieldPoints(stepCount, stepAngle);
        MeshInfo meshInfo = GetMeshInfo(points);
        viewMesh.Clear();
        viewMesh.vertices = meshInfo.vertices;
        viewMesh.triangles = meshInfo.triangles;
        viewMesh.RecalculateNormals();
    }

    List<Vector2> GetViewFieldPoints(int stepCount, float stepAngle) {
        List<Vector2> points = new List<Vector2>();
        ViewCastInfo oldCastInfo = new ViewCastInfo();

        for(int i = 0; i <= stepCount; i++) {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngle * i;
            ViewCastInfo newCastInfo = ViewCast(angle);
            if(i > 0) {
                GetEdgePoints(oldCastInfo,newCastInfo,points);
            }
            // Debug.DrawLine(
            //     transform.position,
            //     new Vector2(transform.position.x, transform.position.y) + DirFromAngle(angle, true) * newCastInfo.distance,
            //     Color.red
            // );
            points.Add(newCastInfo.point);
            oldCastInfo = newCastInfo;
        }

        return points;
    }

    MeshInfo GetMeshInfo(List<Vector2> points) {
        int verticesCount = points.Count + 1;
        Vector3[] vertices = new Vector3[verticesCount];
        int[] tris = new int[(verticesCount - 2) * 3];

        vertices[0] = new Vector3(0,0,0);
        for(int i = 0; i < verticesCount - 1; i++) {
            vertices[i + 1] = transform.InverseTransformPoint(new Vector3(points[i].x, points[i].y, 0));
            vertices[i + 1] += vertices[i + 1].normalized * shadowOffset;
            if(i < verticesCount - 2) {
                tris[i * 3] = 0;
                tris[i * 3 + 1] = i + 1;
                tris[i * 3 + 2] = i + 2; 
            }
        }

        return new MeshInfo(tris,vertices);
    }

    ViewCastInfo ViewCast(float globalAngle) {
        Vector2 dir = DirFromAngle(globalAngle);
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, dir, viewRadius, viewObstaclesMask);

        if(hit2D.collider) {
            return new ViewCastInfo(true, hit2D.distance, hit2D.point, globalAngle);
        } else {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            return new ViewCastInfo(false, viewRadius, pos + dir.normalized * viewRadius, globalAngle);
        }
    }

    Vector2 DirFromAngle(float globalAngle, bool isGlobal=false) {
        if(!isGlobal) {
            globalAngle += transform.eulerAngles.y;
        }
        return new Vector2(Mathf.Sin(globalAngle * Mathf.Deg2Rad), Mathf.Cos(globalAngle * Mathf.Deg2Rad));
    }

    void GetEdgePoints(ViewCastInfo oldCastInfo, ViewCastInfo newCastInfo, List<Vector2> points) {
        bool edgeDetectionThresholdExceeded = Mathf.Abs(oldCastInfo.distance - newCastInfo.distance) > edgeDetectionThreshold;
        if((oldCastInfo.hit && !newCastInfo.hit) || (oldCastInfo.hit && newCastInfo.hit && edgeDetectionThresholdExceeded)) {
            EdgeInfo edgeInfo = FindEdge(oldCastInfo, newCastInfo);
            if(edgeInfo.pointA != Vector2.zero) {
                // Debug.DrawLine(
                //     transform.position,
                //     new Vector3(edgeInfo.pointA.x, edgeInfo.pointA.y, 0),
                //     Color.blue
                // );
                points.Add(edgeInfo.pointA);
            }
            if(edgeInfo.pointB != Vector2.zero) {
                // Debug.DrawLine(
                //     transform.position,
                //     new Vector3(edgeInfo.pointB.x, edgeInfo.pointB.y, 0),
                //     Color.green
                // );
                points.Add(edgeInfo.pointB);
            }
        }
    }

    EdgeInfo FindEdge(ViewCastInfo minCast, ViewCastInfo maxCast) {
        float minAngle = minCast.angle;
        float maxAngle = maxCast.angle;
        Vector2 minPoint = Vector2.zero;
        Vector2 maxPoint = Vector2.zero;

        for(int i = 0; i < edgeDetectionIterations; i++) {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDetectionThresholdExceeded = Mathf.Abs(minCast.distance - newViewCast.distance) > edgeDetectionThreshold;
            if(newViewCast.hit == minCast.hit && !edgeDetectionThresholdExceeded) {
                minAngle = angle;
                minPoint = newViewCast.point;
            } else {
                maxAngle = angle;
                maxPoint = newViewCast.point; 
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }
}
