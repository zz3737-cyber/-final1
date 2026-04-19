using UnityEngine;

public class FourDirectionLimb : MonoBehaviour
{
    [Header("References")]
    public Transform pivot;
    public Transform endPoint;
    public HandGrip handGrip;
    public FootPlant footPlant;

    [Header("Limb Settings")]
    public float length = 1.2f;
    public float rotateSpeed = 360f;

    [Header("Start Pose")]
    public float startAngle = 0f;
    public float currentAngle = 0f;

    [Header("Keyboard Input")]
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    [Header("External Input")]
    public bool useExternalInput = false;
    public Vector2 externalInput;

    [Header("Visual")]
    public float thickness = 0.2f;
    public float rotationOffset = -90f;
    public float inputDeadZone = 0.2f;

    void Start()
    {
        currentAngle = startAngle;
        SnapEndPointToCurrentAngle();
        UpdateLimbVisual();
    }

    void Update()
    {
        if (pivot == null || endPoint == null) return;

        bool handLocked = handGrip != null && handGrip.isGripping && handGrip.currentHold != null;
        bool footLocked = footPlant != null && footPlant.isPlanted && footPlant.currentFootHold != null;

        // 没抓住/没踩住时，才允许输入控制末端点
        if (!handLocked && !footLocked)
        {
            HandleInputAndMoveEndPoint();
        }

        UpdateLimbVisual();
    }

    void HandleInputAndMoveEndPoint()
    {
        Vector2 inputDir = Vector2.zero;

        if (useExternalInput)
        {
            inputDir = externalInput;
        }
        else
        {
            if (Input.GetKey(upKey)) inputDir += Vector2.up;
            if (Input.GetKey(downKey)) inputDir += Vector2.down;
            if (Input.GetKey(leftKey)) inputDir += Vector2.left;
            if (Input.GetKey(rightKey)) inputDir += Vector2.right;
        }

        if (inputDir.magnitude > inputDeadZone)
        {
            inputDir.Normalize();

            float targetAngle = Mathf.Atan2(inputDir.y, inputDir.x) * Mathf.Rad2Deg;

            currentAngle = Mathf.MoveTowardsAngle(
                currentAngle,
                targetAngle,
                rotateSpeed * Time.deltaTime
            );
        }

        SnapEndPointToCurrentAngle();
    }

    void SnapEndPointToCurrentAngle()
    {
        if (pivot == null || endPoint == null) return;

        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);

        endPoint.position = pivot.position + dir * length;
    }

    void UpdateLimbVisual()
    {
        if (pivot == null || endPoint == null) return;

        Vector3 dir = endPoint.position - pivot.position;

        if (dir.sqrMagnitude < 0.0001f) return;

        float actualLength = dir.magnitude;
        Vector3 normalizedDir = dir.normalized;

        // 白条放在中点
        transform.position = pivot.position + normalizedDir * (actualLength * 0.5f);

        // 白条朝向末端
        float visualAngle = Mathf.Atan2(normalizedDir.y, normalizedDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, visualAngle + rotationOffset);

        // 白条长度
        transform.localScale = new Vector3(thickness, actualLength * 0.5f, 1f);

        // 同步当前角度，避免抓住/踩住后角度记录丢失
        currentAngle = Mathf.Atan2(normalizedDir.y, normalizedDir.x) * Mathf.Rad2Deg;
    }
}