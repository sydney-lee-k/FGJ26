using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using Debug = UnityEngine.Debug;

public class FieldOfView : MonoBehaviour
{
    
    [System.Serializable] public class ViewSettings
    {
        public float viewAngle = 90;
        public float viewRadius = 10;
        public float viewCircleRadius = 2;

        public ViewSettings() { }
        public ViewSettings(ViewSettings copiedSettings)
        {
            viewAngle = copiedSettings.viewAngle;
            viewRadius = copiedSettings.viewRadius;
            viewCircleRadius = copiedSettings.viewCircleRadius;
        }
    }
    
    
    [SerializeField] public ViewSettings currentViewSettings = new ViewSettings();
    [SerializeField] private ViewSettings normalViewSettings = new ViewSettings();
    [SerializeField] private ViewSettings aimViewSettings = new ViewSettings();
    [SerializeField] private ViewSettings focusViewSettings = new ViewSettings();

    [System.Serializable]public enum ViewMode
    {
        normal,
        aim,
        focus
    }
    public ViewMode currentView = ViewMode.normal;
    [SerializeField] private float normalLerpTime = 3;
    [SerializeField] private float focusLerpTime = 3;
    [SerializeField] private float aimLerpTime = 3;
    private ViewMode previousView = ViewMode.normal;
    private ViewSettings previousViewSettings;
    private float viewShiftTimer = -1;
    
    
    [SerializeField] private float detectionOffset = 1f;
    [SerializeField] private float detectionTimer = 0.2f;
    private float curDetectionTimer;

    [Header("Layers")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("View Mesh Settings")]
    [SerializeField] private float meshResolution = 1;
    [SerializeField] private int edgeResolveIterations = 1;
    [SerializeField] private float edgeDistanceThreshold = 1;
    [SerializeField] private MeshFilter viewMeshFilter;
    
    [SerializeField] private float yVisualOffset;

    //[HideInInspector]
    public List<Transform> visibleTargets = new();

    private Mesh viewMesh;

    // Increase targetBuffer if more than 64 target colliders can exist inside the viewRadius.
    private readonly List<Vector3> viewPoints = new();
    private readonly Collider[] targetBuffer = new Collider[64];

    private void Start()
    {
        viewMesh = new Mesh
        {
            name = "View Mesh"
        };

        viewMesh.MarkDynamic();
        viewMeshFilter.mesh = viewMesh;
        curDetectionTimer = detectionTimer;

        currentViewSettings = normalViewSettings;
    }

    private void Update()
    {
        if (curDetectionTimer > 0)
        {
            curDetectionTimer -= Time.deltaTime;
        }
        else
        {
            curDetectionTimer = detectionTimer;
            FindVisibleTargets();
        }


        if (currentView != previousView)
        {
            previousView = currentView;
            previousViewSettings = currentViewSettings;
            viewShiftTimer = 0;
        }

        if (viewShiftTimer is <= 1 and >= 0)
        {
            LerpView();
        }
    }

    private void LerpView()
    {
        ViewSettings lerpViewSettings = new ViewSettings(currentViewSettings);
        
        Debug.Log("Preview: " + previousViewSettings.viewAngle + " + " + previousViewSettings.viewRadius + ", " + previousViewSettings.viewCircleRadius);
        Debug.Log("normalViewSettings: " + normalViewSettings.viewAngle + " + " + normalViewSettings.viewRadius + ", " + normalViewSettings.viewCircleRadius);
        
        switch (currentView)
        {
            case ViewMode.normal: 
                lerpViewSettings.viewAngle = Mathf.Lerp(previousViewSettings.viewAngle, normalViewSettings.viewAngle, Mathf.SmoothStep(0.0f, 1.0f, viewShiftTimer));
                lerpViewSettings.viewRadius = Mathf.Lerp(previousViewSettings.viewRadius, normalViewSettings.viewRadius, Mathf.SmoothStep(0.0f, 1.0f, viewShiftTimer));
                lerpViewSettings.viewCircleRadius = Mathf.Lerp(previousViewSettings.viewCircleRadius, normalViewSettings.viewCircleRadius, Mathf.SmoothStep(0.0f, 1.0f, viewShiftTimer));
                viewShiftTimer += Time.deltaTime/normalLerpTime;
                break;
            case ViewMode.aim: 
                lerpViewSettings.viewAngle = Mathf.Lerp(previousViewSettings.viewAngle, aimViewSettings.viewAngle, Mathf.SmoothStep(0.0f, 1.0f, viewShiftTimer));
                lerpViewSettings.viewRadius = Mathf.Lerp(previousViewSettings.viewRadius, aimViewSettings.viewRadius, Mathf.SmoothStep(0.0f, 1.0f, viewShiftTimer));
                lerpViewSettings.viewCircleRadius = Mathf.Lerp(previousViewSettings.viewCircleRadius, aimViewSettings.viewCircleRadius, Mathf.SmoothStep(0.0f, 1.0f, viewShiftTimer));
                viewShiftTimer += Time.deltaTime/aimLerpTime;
                break;
            case ViewMode.focus: 
                lerpViewSettings.viewAngle = Mathf.Lerp(previousViewSettings.viewAngle, focusViewSettings.viewAngle, Mathf.SmoothStep(0.0f, 1.0f, viewShiftTimer));
                lerpViewSettings.viewRadius = Mathf.Lerp(previousViewSettings.viewRadius, focusViewSettings.viewRadius, Mathf.SmoothStep(0.0f, 1.0f, viewShiftTimer));
                lerpViewSettings.viewCircleRadius = Mathf.Lerp(previousViewSettings.viewCircleRadius, focusViewSettings.viewCircleRadius, Mathf.SmoothStep(0.0f, 1.0f, viewShiftTimer));
                viewShiftTimer += Time.deltaTime/focusLerpTime;
                break;
        }

        currentViewSettings = lerpViewSettings;
        
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }
    
    private void FindVisibleTargets()
    {
        foreach (Transform visibleTarget in visibleTargets)
        {
            if(visibleTarget) visibleTarget.GetComponentInChildren<NoiseSource>().SourceInView = false;
        }
        visibleTargets.Clear();
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, currentViewSettings.viewRadius+0.5f, targetBuffer, targetMask);

        for (int i = 0; i < targetCount; i++)
        {
            Transform target = targetBuffer[i].transform;

            Vector3 directionToTarget = (target.position + new Vector3(0, detectionOffset, 0)) - transform.position;
            Debug.DrawRay(transform.position, directionToTarget, Color.red);
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget <= 0f) continue;

            directionToTarget /= distanceToTarget;
            
            if (Vector3.Dot(transform.forward, directionToTarget) > Mathf.Cos(currentViewSettings.viewAngle * 0.5f * Mathf.Deg2Rad))
            {
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    target.GetComponentInChildren<NoiseSource>().SourceInView = true;
                }
            }
        }
        
        targetCount = Physics.OverlapSphereNonAlloc(transform.position, currentViewSettings.viewCircleRadius+0.5f, targetBuffer, targetMask);

        for (int i = 0; i < targetCount; i++)
        {
            Transform target = targetBuffer[i].transform;

            Vector3 directionToTarget = (target.position + new Vector3(0, detectionOffset, 0)) - transform.position;
            Debug.DrawRay(transform.position, directionToTarget, Color.red);
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget <= 0f) continue;
            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask)) visibleTargets.Add(target);
            
        }
    }

    private void DrawFieldOfView()
    {
        //Main cone
        int coneStepCount = Mathf.RoundToInt(currentViewSettings.viewAngle * meshResolution);
        int circleStepCount = Mathf.RoundToInt((360-currentViewSettings.viewAngle) * meshResolution);
        

        float coneStepAngleSize = currentViewSettings.viewAngle / coneStepCount;
        float circleStepAngleSize = (360-currentViewSettings.viewAngle) / circleStepCount;

        viewPoints.Clear();
        ViewCastInfo oldViewCast = default;

        // Go through each ray to determine how far the view can extend before hitting the environment.
        for (int i = 0; i <= coneStepCount; i++)
        {
            float angle = transform.eulerAngles.y - currentViewSettings.viewAngle * 0.5f + coneStepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle, currentViewSettings.viewRadius);

            if (i > 0)
            {
                bool edgeThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;

                // When adjacent rays give considerably different results, refine the
                // boundary between them to better follow corners.
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast, currentViewSettings.viewRadius);
                    if (edge.pointA != Vector3.zero)
                        viewPoints.Add(edge.pointA);

                    if (edge.pointB != Vector3.zero)
                        viewPoints.Add(edge.pointB);
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }
        
        for (int i = 0; i <= circleStepCount; i++)
        {
            float angle = transform.eulerAngles.y - currentViewSettings.viewAngle * 0.5f + circleStepAngleSize * (i+coneStepCount);
            ViewCastInfo newViewCast = ViewCast(angle, currentViewSettings.viewCircleRadius);

            if (i > 0)
            {
                bool edgeThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;

                // When adjacent rays give considerably different results, refine the
                // boundary between them to better follow corners.
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast, currentViewSettings.viewCircleRadius);
                    if (edge.pointA != Vector3.zero) viewPoints.Add(edge.pointA);
                    if (edge.pointB != Vector3.zero) viewPoints.Add(edge.pointB);
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }
        int vertexCount = viewPoints.Count + 1;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = new Vector3(0, yVisualOffset, 0);

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + new Vector3(0, yVisualOffset, 0);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    
    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, float range)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;

        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle, range);

            bool edgeThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


    private ViewCastInfo ViewCast(float globalAngle, float range)
    {
        Vector3 direction = DirectionFromAngle(globalAngle, true);

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, range, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        return new ViewCastInfo(false, transform.position + direction * range, currentViewSettings.viewRadius, globalAngle);    
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}