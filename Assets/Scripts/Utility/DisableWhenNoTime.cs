using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWhenNoTime : MonoBehaviour
{
    private void Update()
    {
        this.gameObject.SetActive(Time.timeScale!=0);   
    }
}
