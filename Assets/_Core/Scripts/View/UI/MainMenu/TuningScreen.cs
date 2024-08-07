using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.IO;

namespace ChimeraGames.UI.MainMenu
{
    public class TuningScreen : Tab
    {
        public static UnityEvent<Color> OnCarColorChanged = new UnityEvent<Color>();
        public static UnityEvent<int> OnCarSpoilerChanged = new UnityEvent<int>();
        public static UnityEvent<Texture2D> OnCarPrintChanged = new UnityEvent<Texture2D>();
        public static UnityEvent<Vector4> OnCarPrintValuesChanged = new UnityEvent<Vector4>();
        [SerializeField]
        private Text carPartText;
        [SerializeField]
        private Slider carPartSlider, colorSliderR, colorSliderG, colorSliderB;
        [SerializeField]
        private Image printImage;
        [SerializeField]
        private InputField inputPrintScaleX, inputPrintScaleY, inputPrintOffsetX, inputPrintOffsetY;
        [SerializeField]
        private SpoilersData spoilersData;


        private void Awake()
        {
            carPartSlider.onValueChanged.AddListener(OnCarPartChanged);
            colorSliderR.onValueChanged.AddListener(OnColorSliderChanged);
            colorSliderG.onValueChanged.AddListener(OnColorSliderChanged);
            colorSliderB.onValueChanged.AddListener(OnColorSliderChanged);

            inputPrintScaleX.onValueChanged.AddListener(ChangeCarPrintValues);
            inputPrintScaleY.onValueChanged.AddListener(ChangeCarPrintValues);
            inputPrintOffsetX.onValueChanged.AddListener(ChangeCarPrintValues);
            inputPrintOffsetY.onValueChanged.AddListener(ChangeCarPrintValues);


            carPartSlider.maxValue = spoilersData.Spoilers.Length - 1;
            carPartSlider.value = SaveSystem.CarViewData.SpoilerIndex;
            var color = SaveSystem.CarViewData.CarBodyColor;
            colorSliderR.value = color.r;
            colorSliderG.value = color.g;
            colorSliderB.value = color.b;
            var texture = SaveSystem.CarViewData.CarPrintImage;
            if(texture != null)
            {
                printImage.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * .5f);
            }
            Vector4 values = SaveSystem.CarViewData.CarPrintValues;
            inputPrintScaleX.SetTextWithoutNotify(values.x.ToString());
            inputPrintScaleY.SetTextWithoutNotify(values.y.ToString());
            inputPrintOffsetX.SetTextWithoutNotify(values.z.ToString());
            inputPrintOffsetY.SetTextWithoutNotify(values.w.ToString());
        }

        private void ChangeCarPrintValues(string arg0)
        {
            OnCarPrintValuesChanged.Invoke(new Vector4(
                float.Parse(inputPrintScaleX.text),
                float.Parse(inputPrintScaleY.text),
                float.Parse(inputPrintOffsetX.text),
                float.Parse(inputPrintOffsetY.text)));
        }

        private void OnColorSliderChanged(float arg0)
        {
            OnCarColorChanged.Invoke(new Color(colorSliderR.value, colorSliderG.value, colorSliderB.value));
        }

        private void OnCarPartChanged(float value)
        {
            carPartText.text = ((int) carPartSlider.value).ToString();
            OnCarSpoilerChanged.Invoke(((int)carPartSlider.value));
        }

        public void OnChooseImageClick()
        {
            FileBrowser.ShowLoadDialog(OnImageChosen, OnCancelChoosing, FileBrowser.PickMode.Files);
        }

        private void OnCancelChoosing()
        {
            if (!isOpen)
            {
                return;
            }
        }

        private void OnImageChosen(string[] paths)
        {
            if (!isOpen)
            {
                return;
            }

            if(paths.Length < 1 || 
                !(paths[0].EndsWith(".jpg") || paths[0].EndsWith(".png")))
            {
                return;
            }

            byte[] bytes = File.ReadAllBytes(paths[0]);
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(bytes);

            if (texture2D != null)
            {
                printImage.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.one * .5f);
            }

            OnCarPrintChanged.Invoke(texture2D);
        }

        public void OnResetImageClick()
        {
            OnCarPrintChanged.Invoke(null);
            printImage.sprite = null;
        }
    }
}
