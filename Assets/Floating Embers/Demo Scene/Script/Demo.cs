using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public GameObject ojbWindzone;
    public GameObject[] prefabs;

    public void SwitchWindzone(bool selected)
    {
        ojbWindzone.SetActive(selected);
    }

    public void dropDown_Click(int index)
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabs[i].SetActive(false);
        }

        prefabs[index].SetActive(true);
    }
}
