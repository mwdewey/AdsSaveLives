﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WebImage : MonoBehaviour {

    private RawImage img;

	void Start () {
        img = GetComponent<RawImage>();
    }

    void Update () {
	
	}

    // downloads and sets the image in a new thread 
    public void SetImage(string url)
    {
        StartCoroutine(SetImageCo(url));
    }

    // sets texture of raw image to the downlaoded image
    IEnumerator SetImageCo(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        img.texture = www.texture;
    }

}
