using UnityEngine;

public class HandGrip : MonoBehaviour
{
    [Header("Keyboard Grip")]
    public KeyCode gripKey = KeyCode.Space;

    [Header("External Grip")]
    public bool useExternalGrip = false;
    public bool externalGripHeld = false;

    [Header("State")]
    public bool isGripping = false;
    public Transform currentHold;

    private Transform candidateHold;

    void Update()
    {
        bool gripHeld = useExternalGrip ? externalGripHeld : Input.GetKey(gripKey);

        if (gripHeld && candidateHold != null)
        {
            isGripping = true;
            currentHold = candidateHold;
        }

        if (!gripHeld)
        {
            isGripping = false;
            currentHold = null;
        }

        if (isGripping && currentHold != null)
        {
            transform.position = currentHold.position;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HandHold"))
        {
            candidateHold = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HandHold") && candidateHold == other.transform)
        {
            candidateHold = null;
        }
    }
}