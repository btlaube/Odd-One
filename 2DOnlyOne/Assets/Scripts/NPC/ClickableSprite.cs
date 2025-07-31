using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class ClickableSprite : MonoBehaviour
{
    public Color hoverColor = Color.yellow;
    public Color clickColor = Color.red;
    public UnityEvent onClick;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private bool isHovered = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit == GetComponent<Collider2D>())
        {
            if (!isHovered)
            {
                isHovered = true;
                spriteRenderer.color = hoverColor;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StopAllCoroutines();
                StartCoroutine(ClickFlash());
                onClick.Invoke();
            }
        }
        else if (isHovered)
        {
            isHovered = false;
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator ClickFlash()
    {
        spriteRenderer.color = clickColor;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = hoverColor;
    }
}
