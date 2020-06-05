using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEffect : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        StartCoroutine(waitDisableEffect());
    }

    public void SetBubbleEffect(Vector2 _pos, Vector2 _scale, Color _color)
    {
        transform.position = _pos;
        transform.localScale = _scale;

        spriteRenderer.color = _color;

        animator.SetTrigger("isEffect");
    }

    IEnumerator waitDisableEffect()
    {
        yield return new WaitForSeconds(0.6f);
        gameObject.SetActive(false);
    }
}
