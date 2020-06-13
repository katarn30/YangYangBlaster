using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinEffect : MonoBehaviour
{
    public Rigidbody2D rig;
    Coroutine dellayCoinCor;
    public bool isGetCoin = false;

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

            if (dellayCoinCor == null)
            {
                dellayCoinCor = StartCoroutine(waitDisable());
            }
        }

        if (GameManager.Instance.isGameOver == true || GameManager.Instance.isStageClear == true || isGetCoin == true)
        {
            if (gameObject.activeInHierarchy == true)
            {
                GameClearCoinEffect();
            }
        }                     
    }

    IEnumerator waitDisable()
    {
        yield return new WaitForSeconds(1f);

        //Vector3 screenPos = Camera.main.WorldToViewportPoint(UIManager.Instance.inGameUI.coinText.transform.position);
        //screenPos.x *= 720;
        //screenPos.y *= 1280;
        //transform.position = new Vector3(screenPos.x, -4.5f);

        //GameManager.Instance.GetCoin(100);
        isGetCoin = true;

        dellayCoinCor = null;

        //gameObject.SetActive(false);
    }

    public void SetCoinEffect(Vector2 _pos)
    {
        transform.position = _pos;

        isGetCoin = false;

        int ranX = Random.Range(-1, 2);
        int ranY = Random.Range(2, 4);

        rig.bodyType = RigidbodyType2D.Dynamic;

        rig.AddForce(new Vector2(ranX, ranY), ForceMode2D.Impulse);               
    }

    public void GameClearCoinEffect()
    {
        transform.DOMove(PlayerManager.Instance.transform.position, 0.3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.GetCoin(100);

            if (dellayCoinCor != null)
            {
                StopCoroutine(dellayCoinCor);
                dellayCoinCor = null;
            }

            gameObject.SetActive(false);
        }
    }
}
