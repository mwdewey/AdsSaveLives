using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WebImage : MonoBehaviour {

    private RawImage img;

	void Start () {
        img = GetComponent<RawImage>();
    }

    void Update () {
	
	}

    public void SetImage(string url)
    {
        StartCoroutine(SetImageCo(url));
    }

    IEnumerator SetImageCo(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        img.texture = www.texture;
    }

}
