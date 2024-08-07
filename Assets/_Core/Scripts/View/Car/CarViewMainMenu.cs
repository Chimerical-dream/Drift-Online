using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarViewMainMenu : MonoBehaviour
{
    [SerializeField]
    private CarViewSetupper carViewSetupper;


    private void Awake()
    {
        ChimeraGames.UI.MainMenu.TuningScreen.OnCarColorChanged.AddListener(OnCarColorChanged);
        ChimeraGames.UI.MainMenu.TuningScreen.OnCarPrintChanged.AddListener(OnCarPrintChanged);
        ChimeraGames.UI.MainMenu.TuningScreen.OnCarPrintValuesChanged.AddListener(OnCarPrintValuesChanged);
        ChimeraGames.UI.MainMenu.TuningScreen.OnCarSpoilerChanged.AddListener(OnCarSpoilerChanged);

        carViewSetupper.SetSpoiler(SaveSystem.CarViewData.SpoilerIndex);
        carViewSetupper.SetPrintValues(SaveSystem.CarViewData.CarPrintValues);
        carViewSetupper.SetPrint(SaveSystem.CarViewData.CarPrintImage);
        carViewSetupper.SetColor(SaveSystem.CarViewData.CarBodyColor);
    }

    private void OnCarSpoilerChanged(int id)
    {
        carViewSetupper.SetSpoiler(id);
    }

    private void OnCarPrintValuesChanged(Vector4 values)
    {
        carViewSetupper.SetPrintValues(values);
    }

    private void OnCarPrintChanged(Texture2D texture)
    {
        carViewSetupper.SetPrint(texture);
    }

    private void OnCarColorChanged(Color color)
    {
        carViewSetupper.SetColor(color);
    }
}
