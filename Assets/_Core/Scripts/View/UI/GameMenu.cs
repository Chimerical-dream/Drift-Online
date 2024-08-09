using ChimeraGames.Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{

    public void OnContinueClick()
    {
        gameObject.SetActive(false);
    }

    public void OnQuitClick()
    {
        FusionConnection.Instance.LeaveLobby();
    }

}
