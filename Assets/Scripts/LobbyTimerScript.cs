using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class LobbyTimerScript : NetworkBehaviour
{

    public float timer;
    TextMeshProUGUI tm;

    void Start()
    {

        tm = this.GetComponent<TextMeshProUGUI>();

    }

    void FixedUpdate()
    {
        
        if(timer > 0.4f) timer -= Time.deltaTime;
        else Destroy(this.gameObject);

        tm.text = timer.ToString("0");

    }

    public void SetTimer(float t){

        timer = t;

    }

}
