using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    public void GameStartButton()
    {
        GameManager.Instance.ChangeGameState(GameManager.GameState.InGame);
    }
}
