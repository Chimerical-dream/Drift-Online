using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChimeraGames.UI.MainMenu
{
    public class NavigationBar : MonoBehaviour
    {
        [SerializeField]
        private Tab mainMenu, tuningScreen, onlineScreen;
        [SerializeField]
        private Button mainMenuBtn, tuningScreenBtn, onlineScreenBtn;
        private Button selectedBtn;

        private void Awake()
        {
            selectedBtn = mainMenuBtn;
            OnMainMenuBtnClick();
        }

        public void OnMainMenuBtnClick()
        {
            selectedBtn.interactable = true;
            selectedBtn = mainMenuBtn;
            selectedBtn.interactable = false;
            mainMenu.Show();
        }

        public void OnTuningScreenBtnClick()
        {
            selectedBtn.interactable = true;
            selectedBtn = tuningScreenBtn;
            selectedBtn.interactable = false;
            tuningScreen.Show();
        }

        public void OnOnlineScreenBtnClick()
        {
            selectedBtn.interactable = true;
            selectedBtn = onlineScreenBtn;
            selectedBtn.interactable = false;
            onlineScreen.Show();
        }

    }
}
