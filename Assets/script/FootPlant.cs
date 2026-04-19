using UnityEngine;

public class FootPlant : MonoBehaviour
{
    [Header("State")]
    public bool isPlanted = false;
    public Transform currentFootHold;

    [Header("References")]
    public Transform hipPivot;

    [Header("Settings")]
    public float maxLegStretch = 1.8f;
    public float detachInputThreshold = 0.7f;
    public float minLockTime = 0.12f;

    [Header("External Input")]
    public bool useExternalInput = false;
    public Vector2 externalInput;

    [Header("Keyboard Input")]
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    private Transform candidateFootHold;
    private float plantedTime = -999f;

    void Update()
    {
        Vector2 inputDir = GetInputDirection();

        // 还没踩住时，碰到候选点就自动吸附
        if (candidateFootHold != null && !isPlanted)
        {
            isPlanted = true;
            currentFootHold = candidateFootHold;
            plantedTime = Time.time;
        }

        if (isPlanted && currentFootHold != null)
        {
            // 脚锁在点上
            transform.position = currentFootHold.position;

            // 1. 腿拉太长自动脱离
            if (hipPivot != null)
            {
                float dist = Vector2.Distance(hipPivot.position, currentFootHold.position);
                if (dist > maxLegStretch)
                {
                    ReleaseFoot();
                    return;
                }
            }

            // 2. 玩家明显想把脚挪走时自动脱离
            // 给一点最小锁定时间，避免刚踩住立刻抖掉
            if (Time.time - plantedTime > minLockTime)
            {
                Vector2 footToHip = Vector2.zero;

                if (hipPivot != null)
                {
                    footToHip = ((Vector2)hipPivot.position - (Vector2)currentFootHold.position).normalized;
                }

                // 如果输入方向和“回收这条腿”的方向比较一致，就脱离
                if (inputDir.magnitude > 0.2f)
                {
                    float dot = Vector2.Dot(inputDir.normalized, footToHip);
                    if (dot > detachInputThreshold)
                    {
                        ReleaseFoot();
                        return;
                    }
                }
            }
        }
    }

    Vector2 GetInputDirection()
    {
        if (useExternalInput)
        {
            return externalInput;
        }

        Vector2 inputDir = Vector2.zero;
        if (Input.GetKey(upKey)) inputDir += Vector2.up;
        if (Input.GetKey(downKey)) inputDir += Vector2.down;
        if (Input.GetKey(leftKey)) inputDir += Vector2.left;
        if (Input.GetKey(rightKey)) inputDir += Vector2.right;

        return inputDir;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HandHold"))
        {
            candidateFootHold = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HandHold"))
        {
            if (candidateFootHold == other.transform)
            {
                candidateFootHold = null;
            }
        }
    }

    public void ReleaseFoot()
    {
        isPlanted = false;
        currentFootHold = null;
        candidateFootHold = null;
    }
}