using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ChimeraGames.UI.MainMenu
{
    public class Tab : MonoBehaviour
    {
        private static UnityEvent OnTabOpened = new UnityEvent();
        protected bool isOpen = false;

        public void Show()
        {
            isOpen = true;
            gameObject.SetActive(true);
            OnTabOpened.Invoke();
            OnTabOpened.AddListener(Hide);
        }

        private void Hide()
        {
            isOpen = false;
            OnTabOpened.RemoveListener(Hide);
            gameObject.SetActive(false);
        }
    }
}