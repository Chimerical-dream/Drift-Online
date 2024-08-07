using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[System.Serializable]
public struct CarViewData
{
    public Color CarBodyColor;
    public Texture2D CarPrintImage
    {
        get
        {
            if (imageBytes == null || imageBytes.Length < 1)
            {
                return null;
            }

            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(imageBytes);
            return texture2D;
        }

        set
        {
            if(value == null)
            {
                imageBytes = null;
            }
            else
            {
                imageBytes = value.EncodeToPNG();
            }
        }
    }

    public byte[] ImageBytes => imageBytes;

    [SerializeField] //to save in json
    private byte[] imageBytes;
    public Vector4 CarPrintValues;
    public int SpoilerIndex;

    public CarViewData(bool defaultValues = true)
    {
        CarBodyColor = new Color32(0xC5, 0x78, 0x37, 0xFF);
        imageBytes = null;
        CarPrintValues = Vector4.one;
        SpoilerIndex = 0;
    }
}
