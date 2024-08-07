using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spoiler : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer[] meshesToDye;
    
    public void ChangeColor(Color color)
    {
        foreach(var mesh in meshesToDye)
        {
            mesh.material.color = color;
        }
    }
}
