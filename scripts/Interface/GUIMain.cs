using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

public class GUIMain : MonoBehaviour
{
    private int _curScrWidth = 0;
    private int _curScrHeight = 0;
    void Start() {
        
    }
    void OnGUI() {
        if (Screen.width != _curScrWidth || Screen.height != _curScrHeight)
        {
            _curScrHeight = Screen.height;
            _curScrWidth = Screen.width;
            Data._inspectorWidth = Screen.width / 4;
        }
    }
}

