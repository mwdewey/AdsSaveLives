using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class BackgroundGen : MonoBehaviour {

    public GameObject webimgObject;
    public AudioClip happySound;
    public AudioClip meowSound;
    public AudioClip badSound;

    private List<string> urls;
    private List<GameObject> imgs;
    private float width = 1920;
    private float height = 1080;
    private GameObject currentImage;
    private AudioSource audio;

	void Start () {
        urls = new List<string>();
        imgs = new List<GameObject>();
        audio = GetComponent<AudioSource>();

        InitAds();
        GenBackground();
    }

	void Update () {
	
	}

    void GenBackground()
    {
        foreach (GameObject img in imgs) Destroy(img);
        imgs.Clear();
        urls.Clear();
        for (var i = 0; i < 20; i++) StartCoroutine(AddUrl());
    }

    void BuildBackground()
    {
        var i = 0;
        foreach(string url in urls)
        {
            print(url);
            GameObject img = Instantiate(webimgObject);
            RectTransform trans = img.GetComponent<RectTransform>();
            imgs.Add(img);


            img.GetComponent<WebImage>().SetImage(url);
            trans.sizeDelta = new Vector2(width/3, height/3);
            
            img.transform.SetParent(transform);
            trans.localScale = new Vector3(1, 1, 1);
            if (i < 3) trans.localPosition = new Vector3(i*width/3,0);
            else if(i < 6) trans.localPosition = new Vector3((i - 3) * width / 3, -height/3);
            else trans.localPosition = new Vector3((i - 6) * width / 3, -height / 3 * 2);

            Button button = img.AddComponent<Button>();
            button.onClick.AddListener(delegate { OnClick(img, button); });

            i++;
        }
    }

    IEnumerator AddUrl()
    {
        if (urls.Count > 8) yield break;

        WWW www = new WWW("http://random.cat/meow");
        yield return www;

        string formatted = www.text.Replace("{\"file\":\"http:\\/\\/random.cat\\/i\\/","").Replace("\"}","");

        if(!formatted.Contains(".gif")) urls.Add("http://random.cat/i/" + formatted);
        if (urls.Count == 9) BuildBackground();
    }

    void OnClick(GameObject img, Button button)
    {
        currentImage = img;
        img.GetComponent<RectTransform>().localPosition = new Vector3(0,0);
        img.GetComponent<Animator>().SetTrigger("expand");
        button.enabled = false;

        foreach (GameObject image in imgs) if (image != img) image.SetActive(false);

        audio.PlayOneShot(meowSound, 1);

        Invoke("ShowAdResponse", 2);
    }

    IEnumerator InitAds()
    {
        if (!Advertisement.isSupported)
        {
            print("Ads not supported.");
            yield break;
        }

        while (!Advertisement.isInitialized || !Advertisement.IsReady())
        {
            yield return new WaitForSeconds(0.5f);
        }

        print("Ads initialized.");
    }
    
    public void ShowAdResponse()
    {
        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        Advertisement.Show(null, options);
    }

    private void HandleShowResult(ShowResult result)
    {
        GameObject textBoxObj = transform.parent.Find("Middle Text Box").gameObject;
        Text textBox = textBoxObj.transform.Find("Text").GetComponent<Text>();

        textBoxObj.SetActive(true);
        switch (result)
        {
            case ShowResult.Finished:
                textBox.text = "The Cat Survived!";
                audio.PlayOneShot(happySound, 1);
                break;
            case ShowResult.Skipped:
                textBox.text = "The Cat Died :(";
                audio.PlayOneShot(badSound, 1);
                break;
            case ShowResult.Failed:
                textBox.text = "The Cat Died :(";
                audio.PlayOneShot(badSound, 1);
                break;
        }

        Invoke("Reset", 2f);
    }

    void Reset()
    {
        transform.parent.Find("Middle Text Box").gameObject.SetActive(false);
        GenBackground();
    }

}
