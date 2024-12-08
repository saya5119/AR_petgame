using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Character : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float walkingSpeed = 0.1f;
    [SerializeField] private ARPlane arPlane; // AR Planeの参照を追加
    private Mesh groundMesh;
    private Vector3 _destination = Vector3.zero;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (arPlane != null)
        {
            groundMesh = arPlane.GetComponent<MeshFilter>()?.mesh;
        }
        SetNewDestination();
    }

    void Update()
    {
        if (groundMesh == null) return;
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
        if (groundMesh == null) return;

        var vertices = groundMesh.vertices;
        if (vertices.Length == 0) return;

        var randomVertex = vertices[Random.Range(0, vertices.Length)];
        _destination = arPlane.transform.TransformPoint(randomVertex);
    }

    private void MoveTowardsDestination(float distance)
    {
        float transformLerp = (Time.deltaTime * walkingSpeed) / distance;
        transform.position = Vector3.Lerp(transform.position, _destination, transformLerp);

        Vector3 lookAtTarget = new Vector3(_destination.x, transform.position.y, _destination.z);
        Vector3 direction = lookAtTarget - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
        }
    }
}
