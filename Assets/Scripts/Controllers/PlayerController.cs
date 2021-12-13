using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool smoothTransition = false;
    public float transitionSpeed = 10f;
    public float transitionRotationSpeed = 500f;

    Vector3 targetGridPos, prevtargetGridPos, targetRotation;
    public AudioSource sfx;
    public AudioClip step_sound;

    private void Start()
    {
        targetGridPos = transform.position;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (!Physics.Raycast(transform.position, targetGridPos - transform.position, 0.6f)) {
            if (prevtargetGridPos != targetGridPos)
                sfx.PlayOneShot(step_sound);
            prevtargetGridPos = targetGridPos;
            Vector3 targetPosition = targetGridPos;

            if (!smoothTransition)
                transform.position = targetPosition;
            else
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * transitionSpeed);
        }
        else
            targetGridPos = prevtargetGridPos;

        if (targetRotation.y > 270f && targetRotation.y < 361f) targetRotation.y = 0f;
        if (targetRotation.y < 0f) targetRotation.y = 270f;
        if (!smoothTransition)
            transform.rotation = Quaternion.Euler(targetRotation);
        else
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * transitionRotationSpeed);
    }

    public void RotateLeft() { if (AtRest) targetRotation -= Vector3.up * 90f; }
    public void RotateRight() { if (AtRest) targetRotation += Vector3.up * 90f; }
    public void MoveForward() { if (AtRest) targetGridPos += transform.forward; }
    public void MoveBackward() { if (AtRest) targetGridPos -= transform.forward; }
    public void MoveLeft() { if (AtRest) targetGridPos -= transform.right; }
    public void MoveRight() { if (AtRest) targetGridPos += transform.right; }

    public bool AtRest {
        get {
            return (Vector3.Distance(transform.position, targetGridPos) < 0.01f) &&
                    (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.01);
        }
    }
}
