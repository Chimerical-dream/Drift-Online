using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ChimeraGames.UI.MainMenu
{
    public class MainMenu : Tab
    {
        public void OnQuitClick()
        {
            Application.Quit();
        }
    }
}
