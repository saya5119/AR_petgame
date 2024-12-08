using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float walkingSpeed = 0.1f;
    [SerializeField] private MeshFilter ground;
    private Vector3 _destination = Vector3.zero;

    void Start()
    {
        animator = GetComponent<Animator>();
        SetNewDestination();
    }

    void Update()
    {
        var distance = Vector3.Distance(transform.position, _destination);

        if (distance <= 1f)
        {
            SetNewDestination();
            return;
        }

        MoveTowardsDestination(distance);
    }

    private void SetNewDestination()
    {
        if (ground == null) return;

        var vertices = ground.sharedMesh.vertices;
        if (vertices.Length == 0) return;

        var randomVertex = vertices[Random.Range(0, vertices.Length)];
        _destination = ground.transform.TransformPoint(randomVertex);

        animator.SetBool("walking", true);
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
