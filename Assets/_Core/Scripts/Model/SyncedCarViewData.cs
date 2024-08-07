using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SyncedCarViewData : INetworkStruct
{
    public Color CarBodyColor;

    public Vector4 CarPrintValues;
    public int SpoilerIndex;

    public SyncedCarViewData(CarViewData carViewData)
    {
        CarBodyColor = carViewData.CarBodyColor;
        CarPrintValues = carViewData.CarPrintValues;
        SpoilerIndex = carViewData.SpoilerIndex;
    }
}
