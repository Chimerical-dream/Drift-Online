using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static bool wasInit = false;
    private static SaveData saveData;
    public static SaveData SaveData => saveData;
    public static CarViewData CarViewData => saveData.CarViewData;

    private const string SaveKey = "saveData";

    private void Awake()
    {
        if (wasInit)
        {
            return;
        }


        ChimeraGames.UI.MainMenu.TuningScreen.OnCarColorChanged.AddListener(OnCarColorChanged);
        ChimeraGames.UI.MainMenu.TuningScreen.OnCarPrintChanged.AddListener(OnCarPrintChanged);
        ChimeraGames.UI.MainMenu.TuningScreen.OnCarPrintValuesChanged.AddListener(OnCarPrintValuesChanged);
        ChimeraGames.UI.MainMenu.TuningScreen.OnCarSpoilerChanged.AddListener(OnCarSpoilerChanged);

        ChimeraGames.UI.MainMenu.OnlineScreen.OnPlayerNicknameChanged.AddListener(OnNicknameChanged);

        ReadSave();

        wasInit = true;
    }

    private void OnNicknameChanged(string name)
    {
        SaveData.Nickname = name;
        WriteSave();
    }

    private void OnCarColorChanged(Color color)
    {
        saveData.CarViewData.CarBodyColor = color;
        WriteSave();
    }

    private void OnCarPrintChanged(Texture2D texture)
    {
        saveData.CarViewData.CarPrintImage = texture;
        WriteSave();

    }

    private void OnCarPrintValuesChanged(Vector4 values)
    {
        saveData.CarViewData.CarPrintValues = values;
        WriteSave();

    }

    private void OnCarSpoilerChanged(int ind)
    {
        saveData.CarViewData.SpoilerIndex = ind;
        WriteSave();

    }

    private void ReadSave()
    {
        string save = PlayerPrefs.GetString(SaveKey, "");
        if (save.Equals(""))
        {
            saveData = new SaveData();
            return;
        }

        saveData = (SaveData)JsonUtility.FromJson(save, typeof(SaveData));
    }

    public void WriteSave()
    {
        string save = JsonUtility.ToJson(saveData);

        PlayerPrefs.SetString(SaveKey, save);
    }
}
