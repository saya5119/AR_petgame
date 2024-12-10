using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Character : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float walkingSpeed = 0.1f;
    private ARPlaneManager arPlaneManager;
    private Vector3 _destination;
    private ARPlane arPlane;

    void Start()
    {
        animator = GetComponent<Animator>();
        arPlaneManager = FindObjectOfType<ARPlaneManager>();
        SetNewDestination();
    }

    void Update()
    {
        var distance = Vector3.Distance(transform.position, _destination);

        if (distance <= 1f)
        {
            animator.SetBool("walking", false);
            SetNewDestination();
        }
        else
        {
            animator.SetBool("walking", true);
            MoveTowardsDestination(distance);
        }
    }

    private void SetNewDestination()
    {
        foreach (var plane in arPlaneManager.trackables)
        {
            if (plane.boundary.Length > 0)
            {
                arPlane = plane; // 平面を選択
                break; // 最初に見つかった平面を使用
            }
        }
        
        if (arPlane == null || arPlane.boundary == null || arPlane.boundary.Length == 0) return;

        // 平面のboundaryからランダムな頂点を選択
        var boundary = arPlane.boundary;
        var randomIndex = Random.Range(0, boundary.Length);
        var randomPoint = boundary[randomIndex];

        _destination = arPlane.transform.TransformPoint(randomPoint);
    }

    private void MoveTowardsDestination(float distance)
    {
        float transformLerp = (Time.deltaTime * walkingSpeed) / distance;
        transform.position = Vector3.Lerp(transform.position, _destination, transformLerp);

        Vector3 lookAtTarget = new Vector3(_destination.x, transform.position.y, _destination.z);
        Vector3 direction = lookAtTarget - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
    }
}

