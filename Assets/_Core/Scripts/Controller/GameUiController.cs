using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameUiController : MonoBehaviour
{
    private Controls playerControls;
    [SerializeField]
    private GameObject mainMenu, infoTab;

    private void Awake()
    {
        playerControls = new Controls();
        playerControls.GameUI.Enable();

        playerControls.GameUI.Menu.performed += ToggleMainMenu;
        playerControls.GameUI.OpenInfo.performed += OpenInfoTab;
        playerControls.GameUI.OpenInfo.canceled += CloseInfoTab;
    }

    private void OpenInfoTab(InputAction.CallbackContext obj)
    {
        infoTab.SetActive(true);
    }

    private void CloseInfoTab(InputAction.CallbackContext obj)
    {
        infoTab.SetActive(false);
    }

    private void ToggleMainMenu(InputAction.CallbackContext obj)
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
    }
}
