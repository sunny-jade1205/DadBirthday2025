using Fusion;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class NetworkBall : NetworkBehaviour
{
    [SerializeField]
    private Rigidbody targetRigidbody;

    [Networked]
    public Vector3 NetworkedPosition { get; set; }

    [Networked]
    public Quaternion NetworkedRotation { get; set; }

    [Networked]
    public Vector3 NetworkedVelocity { get; set; }

    private void Awake()
    {
        if (targetRigidbody == null)
        {
            targetRigidbody = GetComponent<Rigidbody>();
        }
    }

    public override void Spawned()
    {
        if (targetRigidbody == null)
        {
            return;
        }

        targetRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        targetRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (Object.HasStateAuthority)
        {
            targetRigidbody.isKinematic = false;
        }
        else
        {
            targetRigidbody.isKinematic = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (targetRigidbody == null)
        {
            return;
        }

        if (Object.HasStateAuthority)
        {
            NetworkedPosition = targetRigidbody.position;
            NetworkedRotation = targetRigidbody.rotation;
            NetworkedVelocity = targetRigidbody.velocity;
        }
    }

    public override void Render()
    {
        if (targetRigidbody == null)
        {
            return;
        }
        if (!Object.HasStateAuthority)
        {
            targetRigidbody.MovePosition(NetworkedPosition);
            targetRigidbody.MoveRotation(NetworkedRotation);
        }
    }
}

