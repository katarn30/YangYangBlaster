using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckUIController : MonoBehaviour
{
    [Header("Deck UI")]
    public List<GameObject> plusButton = new List<GameObject>();
    public List<Image> deckMercenary = new List<Image>();

    public void SetDeckUI()
    {
        Debug.Log(GameDataManager.Instance.userData.mercenaryDataList.Count);

        if(GameDataManager.Instance.userData.mercenaryDataList.Count == 0)
        {
            plusButton[0].gameObject.SetActive(true);
            deckMercenary[0].gameObject.SetActive(false);

            plusButton[1].gameObject.SetActive(true);
            deckMercenary[1].gameObject.SetActive(false);
        }
        else if (GameDataManager.Instance.userData.mercenaryDataList.Count == 1)
        {
            plusButton[0].gameObject.SetActive(false);
            deckMercenary[0].gameObject.SetActive(true);
            deckMercenary[0].sprite = GameDataManager.Instance.userData.mercenaryDataList[0].catImage;

            plusButton[1].gameObject.SetActive(true);
            deckMercenary[1].gameObject.SetActive(false);
        }
        else if (GameDataManager.Instance.userData.mercenaryDataList.Count >= 2)
        {
            plusButton[0].gameObject.SetActive(false);
            deckMercenary[0].gameObject.SetActive(true);
            deckMercenary[0].sprite = GameDataManager.Instance.userData.mercenaryDataList[0].catImage;

            plusButton[1].gameObject.SetActive(false);
            deckMercenary[1].gameObject.SetActive(true);
            deckMercenary[1].sprite = GameDataManager.Instance.userData.mercenaryDataList[1].catImage;
        }     
    }
}
