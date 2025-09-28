using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Balls : NetworkBehaviour
{
    [Networked] public Vector3 NetworkPosition { get; set; }
    [Networked] public Quaternion NetworkRotation { get; set; }
    [Networked] public Vector3 NetworkVelocity { get; set; }
    [Networked] public Vector3 NetworkAngularVelocity { get; set; }

    private Rigidbody rb;
    private bool isInitialized = false;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();

        if (Object.HasStateAuthority)
        {
            NetworkPosition = transform.position;
            NetworkRotation = transform.rotation;
            if (rb != null)
            {
                NetworkVelocity = rb.velocity;
                NetworkAngularVelocity = rb.angularVelocity;
            }
        }

        isInitialized = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!isInitialized || rb == null) return;

        if (Object.HasStateAuthority)
        {
            NetworkPosition = transform.position;
            NetworkRotation = transform.rotation;
            NetworkVelocity = rb.velocity;
            NetworkAngularVelocity = rb.angularVelocity;
        }
        else
        {
            transform.position = NetworkPosition;
            transform.rotation = NetworkRotation;
            rb.velocity = NetworkVelocity;
            rb.angularVelocity = NetworkAngularVelocity;
        }
    }

    public override void Render()
    {
        if (!isInitialized || Object.HasStateAuthority) return;

        float alpha = 1f;

        transform.position = Vector3.Lerp(transform.position, NetworkPosition, alpha);
        transform.rotation = Quaternion.Lerp(transform.rotation, NetworkRotation, alpha);
    }
}
