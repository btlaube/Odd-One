using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    [Range(0f, 1f)] public float idleChance = 0.3f;

    [Header("Position Offsets")]
    public Transform destinationOffset;

    [Header("Behavior Settings")]
    public Vector2 moveDurationRange = new Vector2(1f, 3f);
    public Vector2 idleDurationRange = new Vector2(1f, 2f);
    public float sprintChance = 0.25f;

    [Header("Movement Bounds")]
    public Vector2 xBounds = new Vector2(-10f, 10f);
    public Vector2 yBounds = new Vector2(-5f, 5f);

    [Header("External Goals")]
    private Vector3? externalTarget = null;
    public bool overrideMovement = false;

    [Header("Ground Check")]
    public Transform groundCheck;

    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.right;
    private bool isSprinting;
    private float stateTimer;
    private bool isMoving;
    private Animator animator;
    private bool previousIdle = true;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        PickNewState();
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;
        bool isCurrentlyMoving = false;

        if (overrideMovement && externalTarget != null)
        {
            Vector3 direction = (externalTarget.Value - transform.position).normalized;
            MoveInDirection(direction);
            isCurrentlyMoving = true;
        }
        else
        {
            if (stateTimer <= 0f)
                PickNewState();

            if (moveInput != Vector2.zero)
            {
                Vector3 move = new Vector3(moveInput.x, moveInput.y, 0f);
                MoveInDirection(move.normalized);
                isCurrentlyMoving = true;
            }
        }

        if (animator != null)
            animator.SetBool("IsWalking", isCurrentlyMoving);
    }

    // Whether the NPC is currently idle (not moving)
    public bool IsIdle()
    {
        return !isMoving; // or whatever condition your controller uses
    }

    // Whether the idle state has changed (optional for optimization)
    public bool IsIdleStateChanged()
    {
        bool currentIdle = IsIdle();
        bool changed = currentIdle != previousIdle;
        previousIdle = currentIdle;
        return changed;
    }

    private void PickNewState()
    {
        isMoving = Random.value > idleChance;

        if (isMoving)
        {
            float smallTurnChance = 1f;

            if (moveInput != Vector2.zero && Random.value < smallTurnChance)
            {
                float angle = Random.Range(-20f, 20f);
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                moveInput = rotation * moveInput;
            }
            else
            {
                moveInput = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            }

            lastDirection = moveInput;
            isSprinting = Random.value < sprintChance;
            stateTimer = Random.Range(moveDurationRange.x, moveDurationRange.y);
        }
        else
        {
            moveInput = Vector2.zero;
            isSprinting = false;
            stateTimer = Random.Range(idleDurationRange.x, idleDurationRange.y);
        }
    }

    private void MoveInDirection(Vector3 direction)
    {
        // Debug.Log($"{name} moving in {direction}");
        if (direction == Vector3.zero || !enabled)
            return;

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 move = direction * currentSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + move;

        float clampedX = Mathf.Clamp(newPos.x, xBounds.x, xBounds.y);
        float clampedY = Mathf.Clamp(newPos.y, yBounds.x, yBounds.y);
        bool hitBorder = false;

        if (groundCheck != null)
        {
            Vector3 groundPos = groundCheck.position;
            hitBorder = HasHitBorder(groundPos);
        }

        newPos.x = clampedX;
        newPos.y = clampedY;
        transform.position = newPos;

        if (direction != Vector3.zero)
            lastDirection = new Vector2(direction.x, direction.y).normalized;

        // Flip scale on X if moving left or right
        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (direction.x > 0 ? 1 : -1);
            transform.localScale = scale;
        }

        if (hitBorder)
        {
            // Debug.Log($"{name} on the edge");
            StopCompletely();
            // ResetMovement();
            ForceDirectionChange();
            // stateTimer = 0f;
        }
    }

    public void MoveTo(Vector3 target)
    {
        Vector3 adjustedTarget = target;

        if (destinationOffset != null)
        {
            Vector3 offsetVector = destinationOffset.position - transform.position;
            adjustedTarget = target - offsetVector;
        }

        externalTarget = adjustedTarget;
        overrideMovement = true;
    }

    public void StopOverrideMovement()
    {
        overrideMovement = false;
        externalTarget = null;
    }

    public bool HasHitBorder(Vector3 position)
    {
        return position.x <= xBounds.x || position.x >= xBounds.y ||
               position.y <= yBounds.x || position.y >= yBounds.y;
    }

    public void StopCompletely()
    {
        // Debug.Log($"{name} stoppped");
        StopOverrideMovement();
        ResetMovement();
    }

    public void ResetMovement()
    {
        moveInput = Vector2.zero;
        stateTimer = 0f;

        if (animator != null)
            animator.SetBool("IsWalking", false);
    }

    public void ForceDirectionChange(float maxDeviationAngle = 20f, float distance = 2f)
    {
        if (groundCheck == null)
        {
            Debug.LogWarning($"{name} has no groundCheck assigned.");
            return;
        }

        // Debug.Log($"{name} forced turn");

        Vector2 baseDirection = -lastDirection.normalized;
        float deviation = Random.Range(-maxDeviationAngle, maxDeviationAngle);
        Quaternion rotation = Quaternion.Euler(0, 0, deviation);
        Vector2 offsetDir = (Vector2)(rotation * baseDirection);

        Vector3 newTarget = groundCheck.position + new Vector3(offsetDir.x, offsetDir.y, 0f) * distance;
        newTarget.x = Mathf.Clamp(newTarget.x, xBounds.x, xBounds.y);
        newTarget.y = Mathf.Clamp(newTarget.y, yBounds.x, yBounds.y);

        // Begin override movement
        MoveTo(newTarget);

        // Delay before cancelling override
        StartCoroutine(StopOverrideWhenClose());
    }

    private IEnumerator StopOverrideWhenClose(float threshold = 0.1f)
    {
        while (externalTarget.HasValue)
        {
            if (Vector2.Distance(transform.position, externalTarget.Value) <= threshold)
            {
                StopOverrideMovement();
                PickNewState(); // resume normal behavior
                yield break;
            }
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            new Vector3((xBounds.x + xBounds.y) / 2, (yBounds.x + yBounds.y) / 2, 0f),
            new Vector3(xBounds.y - xBounds.x, yBounds.y - yBounds.x, 0.1f)
        );

        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
    }
}
