using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarViewSetupper : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer carmeshRenderer;
    [SerializeField]
    private Transform spoilerParent;
    [SerializeField]
    private Spoiler currentSpoiler;
    [SerializeField]
    private SpoilersData spoilersData;

    public void SetColor(Color color)
    {
        carmeshRenderer.material.color = color;
        currentSpoiler?.ChangeColor(color);
    }

    public void SetPrint(Texture2D texture2D)
    {
        carmeshRenderer.material.mainTexture = texture2D;
    }

    public void SetPrint(byte[] imageBytes)
    {
        Texture2D texture2D = new Texture2D(1, 1);
        texture2D.LoadImage(imageBytes);
        carmeshRenderer.material.mainTexture = texture2D;
    }

    public void SetPrintValues(Vector4 values)
    {
        carmeshRenderer.material.mainTextureScale = new Vector2(values.x, values.y);
        carmeshRenderer.material.mainTextureOffset = new Vector2(values.z, values.w);
    }

    public void SetSpoiler(int ind)
    {
        if(currentSpoiler != null)
        {
            Destroy(currentSpoiler.gameObject);
        }

        if (spoilersData.Spoilers[ind] == null)
        {
            currentSpoiler = null;
            return;
        }

        currentSpoiler =  Instantiate(spoilersData.Spoilers[ind], spoilerParent);


        currentSpoiler.ChangeColor(SaveSystem.CarViewData.CarBodyColor);
    }
}
