using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NPCController))]
public class ImposterBehavior : MonoBehaviour
{
    [Header("Timing")]
    public float oddBehaviorInterval = 5f;
    public float behaviorVariance = 3f;

    // [Header("Visual Oddities")]
    [Header("Imposter Oddities")]
    public bool glitchMovement = true;
    public bool flashColor = true;
    public float flashColorDuration = 1f;
    public Color oddColor = Color.magenta;
    // public bool blinkless = true;
    public bool mimicOthers = true;
    public bool hitWalls = true;
    public bool lessIdle = true;
    public float lowIdleChance = 0.2f;


    private bool missionComplete;
    private NPCController npcController;
    private SpriteRenderer sr;
    private Animator animator;
    private AudioHandler audioHandler;
    private GameState gameState;
    private int oddity;

    void Awake()
    {        
        npcController = GetComponent<NPCController>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioHandler = GetComponent<AudioHandler>();
    }

    void Start()
    {
        gameState = GameStateManager.Instance.gameState;
        SetUpImposter();
        StartCoroutine(OddBehaviorRoutine());
    }

    public void SetUpImposter()
    {
        RandomizeImposterOddity();
        SetImposterDifficulty();
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

    public void RandomizeImposterOddity()
    {
        glitchMovement = false;
        flashColor = false;
        lessIdle = false;
        oddity = Random.Range(0, 3);

        switch (oddity)
        {
            case 0:
                glitchMovement = true;                
                break;
            case 1:
                flashColor = true;
                break;
            case 2:
                lessIdle = true;
                npcController.idleChance = lowIdleChance;
                npcController.moveDurationRange = new Vector2(3f, 6f);
                npcController.idleDurationRange = new Vector2(0f, 0f);
                npcController.walkSpeed = 1f;
                break;
        }
    }

    public void SetImposterDifficulty()
    {
        oddBehaviorInterval = 3 + gameState.level;
        if (!lessIdle) npcController.idleChance = 0.75f - (gameState.level * 0.1f);
    }

    IEnumerator OddBehaviorRoutine()
    {
        while (!missionComplete)
        {
            yield return new WaitForSeconds(Random.Range(oddBehaviorInterval - behaviorVariance, oddBehaviorInterval + behaviorVariance));
            TriggerOddBehavior();
        }
    }

    // Trigger behaviors for glitch, color flash, and mimic
    void TriggerOddBehavior()
    {
        int behavior = Random.Range(0, 2);

        Debug.Log($"Did odd behavior: {oddity}");
        switch (oddity)
        {
            case 0:
                StartCoroutine(GlitchMove());
                audioHandler.Play("Oddity");
                break;
            case 1:
                StartCoroutine(FlashColor(flashColorDuration));
                audioHandler.Play("Oddity");
                break;
        }
    }

    IEnumerator GlitchMove()
    {
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        transform.position += offset;
        yield return new WaitForSeconds(0.1f);
        transform.position -= offset;
    }

    IEnumerator FlashColor(float duration)
    {
        Color original = sr.color;
        // Color inverted = new Color(1f - original.r, 1f - original.g, 1f - original.b, original.a);
        
        // sr.color = inverted;
        RandomizeColor();
        yield return new WaitForSeconds(duration);
        sr.color = original;
    }


    // Called by clickable sprite when clicked
    public void Accuse()
    {
        GameStateManager.Instance.PassLevel();
    }

}
