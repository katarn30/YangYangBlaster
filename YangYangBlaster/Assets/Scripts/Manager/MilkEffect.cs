using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilkEffect : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Rigidbody2D rig;
    MilkItem milkItem;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= GameManager.Instance.minScreenPos.x)
        {
            transform.position = new Vector2(GameManager.Instance.minScreenPos.x, transform.position.y);
        }
        else if (transform.position.x >= GameManager.Instance.maxScreenPos.x)
        {
            transform.position = new Vector2(GameManager.Instance.maxScreenPos.x, transform.position.y);
        }

        if (transform.position.y <= -4.5f)
        {
            rig.bodyType = RigidbodyType2D.Kinematic;
            rig.velocity = Vector2.zero;
            transform.position = new Vector2(transform.position.x, -4.5f);
        }
    }

    public void SetMilkEffect(MilkItem _milkItem, Vector2 _pos)
    {
        transform.position = _pos;

        int ranX = Random.Range(-1, 2);
        int ranY = Random.Range(2, 4);

        rig.bodyType = RigidbodyType2D.Dynamic;
        milkItem = _milkItem;

        rig.AddForce(new Vector2(ranX, ranY), ForceMode2D.Impulse);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Get Milk");

            GameManager.Instance.SetMilkItem(milkItem);

            gameObject.SetActive(false);
        }
    }
}
