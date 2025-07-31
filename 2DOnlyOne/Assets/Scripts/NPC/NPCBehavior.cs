using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NPCController))]
public class NPCBehavior : MonoBehaviour
{
    [Header("Detection Ranges")]
    public float noticeRange = 5f;
    public float interactRange = 2f;

    private NPCController npcController;
    private InteractableItem currentTarget;

    [Header("Notice and Interact Anchor")]
    public Transform noticeAnchor;

    [Header("Interaction Timing")]
    public float interactionDuration = 2f;
    public float interactionCooldown = 3f;
    private float interactCooldownTimer = 0f;

    [Header("Notice Timing")]
    public float noticeCooldown = 1.5f;
    private float noticeCooldownTimer = 0f;

    private bool isInteracting = false;

    private Animator animator;
    private GameState gameState;

    void Awake()
    {
        animator = GetComponent<Animator>();
        npcController = GetComponent<NPCController>();
    }

    void Start()
    {
        gameState = GameStateManager.Instance.gameState;
        SetUpNPC();
    }

    public void SetUpNPC()
    {
        SetNPCDifficulty();
        RandomizeColor();
    }

    public void RandomizeColor()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Hue: [0, 1] covers all colors on the color wheel
            float hue = Random.Range(0f, 1f);

            // Saturation: keep it somewhat high for vibrant colors, e.g., [0.5, 0.9]
            float saturation = Random.Range(0.3f, 0.4f);

            // Value (Brightness): avoid extremes, e.g., [0.6, 1.0]
            float value = Random.Range(0.3f, 0.4f);

            sr.color = Color.HSVToRGB(hue, saturation, value);
        }
    }

    public void SetNPCDifficulty()
    {
        npcController.idleChance = 0.75f - (gameState.level * 0.1f);
    }

    void Update()
    {
        if (isInteracting)
            return;

        if (interactCooldownTimer > 0f)
            interactCooldownTimer -= Time.deltaTime;

        if (noticeCooldownTimer > 0f)
            noticeCooldownTimer -= Time.deltaTime;

        if (currentTarget != null)
        {
            float dist = Vector2.Distance(noticeAnchor.position, currentTarget.transform.position);

            if (dist <= interactRange)
            {
                // Stop and interact if close enough
                if (currentTarget.TryClaim())
                {
                    npcController.StopCompletely();
                    npcController.enabled = false;
                    StartCoroutine(InteractThenCooldown(interactionDuration));
                    // npcController.ResetMovement();
                    // Debug.Log($"{name} reached and started interacting with {currentTarget.name}");
                }
                else
                {
                    // If item is already claimed, forget about it
                    // Debug.Log($"{name} got unlucky");
                    npcController.StopCompletely();
                    // npcController.ResetMovement();
                    
                    // Reset interactable object
                    currentTarget = null;
                }
            }

            return; // skip DetectAndReact if already heading to a target
        }

        if (interactCooldownTimer <= 0f && noticeCooldownTimer <= 0f)
        {
            DetectAndReact();
            interactCooldownTimer = interactionCooldown;
            noticeCooldownTimer = noticeCooldown;
        }
    }

    void DetectAndReact()
    {
        if (currentTarget != null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(noticeAnchor.position, noticeRange);
        float closestDist = Mathf.Infinity;
        InteractableItem bestTarget = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Interactable"))
            {
                float distance = Vector2.Distance(noticeAnchor.position, hit.transform.position);
                InteractableItem item = hit.GetComponent<InteractableItem>();

                if (item != null && !item.IsInUse && distance < closestDist)
                {
                    closestDist = distance;
                    bestTarget = item;
                }
            }
        }

        if (bestTarget != null)
        {
            npcController.MoveTo(bestTarget.transform.position);
            currentTarget = bestTarget;
            noticeCooldownTimer = noticeCooldown;
            Debug.Log($"{name} is moving toward {bestTarget.name}");
        }
    }

    public void StopInteraction()
    {
        if (currentTarget != null)
        {
            currentTarget.Release();
            currentTarget = null;
            npcController.enabled = true;
        }
    }
    
    public IEnumerator InteractThenCooldown(float duration)
    {
        isInteracting = true;

        if (animator != null)
            animator.SetBool("IsInteracting", true);

        yield return new WaitForSeconds(duration);

        StopInteraction();

        interactCooldownTimer = interactionCooldown;
        isInteracting = false;

        if (animator != null)
            animator.SetBool("IsInteracting", false);
        npcController.ResetMovement();
        npcController.ForceDirectionChange();
    }

    // Called by clickable sprite when clicked
    public void Accuse()
    {
        GameStateManager.Instance.FailLevel();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(noticeAnchor.position, noticeRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(noticeAnchor.position, interactRange);
    }

    public void Dance()
    {
        animator.SetBool("IsDancing", true);
        npcController.enabled = false;
    }

}
