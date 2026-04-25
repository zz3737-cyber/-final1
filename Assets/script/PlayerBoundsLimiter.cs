using UnityEngine;

public class PlayerBoundsLimiter : MonoBehaviour
{
    [Header("Body")]
    public Rigidbody2D bodyRb;

    [Header("Limb End Points")]
    public Transform leftHandEnd;
    public Transform rightHandEnd;
    public Transform leftFootEnd;
    public Transform rightFootEnd;

    [Header("Bounds")]
    public bool useBounds = true;

    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 5f;

    [Header("Body Clamp")]
    public bool clampBody = true;

    [Header("Limb Clamp")]
    public bool clampHands = true;
    public bool clampFeet = true;

    void LateUpdate()
    {
        if (!useBounds) return;

        if (clampBody)
        {
            ClampBody();
        }

        if (clampHands)
        {
            ClampPoint(leftHandEnd);
            ClampPoint(rightHandEnd);
        }

        if (clampFeet)
        {
            ClampPoint(leftFootEnd);
            ClampPoint(rightFootEnd);
        }
    }

    void ClampBody()
    {
        if (bodyRb == null) return;

        Vector2 pos = bodyRb.position;
        Vector2 vel = bodyRb.linearVelocity;

        bool changed = false;

        if (pos.x < minX)
        {
            pos.x = minX;
            if (vel.x < 0f) vel.x = 0f;
            changed = true;
        }
        else if (pos.x > maxX)
        {
            pos.x = maxX;
            if (vel.x > 0f) vel.x = 0f;
            changed = true;
        }

        if (pos.y < minY)
        {
            pos.y = minY;
            if (vel.y < 0f) vel.y = 0f;
            changed = true;
        }
        else if (pos.y > maxY)
        {
            pos.y = maxY;
            if (vel.y > 0f) vel.y = 0f;
            changed = true;
        }

        if (changed)
        {
            bodyRb.position = pos;
            bodyRb.linearVelocity = vel;
        }
    }

    void ClampPoint(Transform point)
    {
        if (point == null) return;

        Vector3 pos = point.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        point.position = pos;
    }
}