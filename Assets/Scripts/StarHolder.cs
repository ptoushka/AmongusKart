using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarHolder : MonoBehaviour
{
    public GameObject[] Stars;

    public void SetAmount(int amount)
    {
        for (int i = 0; i < Stars.Length; i++) Stars[i].SetActive(i < amount);
    }
}
