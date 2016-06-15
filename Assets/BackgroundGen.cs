using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class BackgroundGen : MonoBehaviour {

    // web image prefab and sound clips
    public GameObject webimgObject;
    public AudioClip happySound;
    public AudioClip meowSound;
    public AudioClip badSound;

    private List<string> urls;                    // list to store all kitten picture urls
    private List<GameObject> imgs;                // list containing references to all web images
    private float width = 1920, height = 1080;
    private GameObject currentImage;              // last image the user selected
    private AudioSource audio;

	void Start () {
        urls = new List<string>();
        imgs = new List<GameObject>();
        audio = GetComponent<AudioSource>();

        InitAds();        // starts the ad service
        GenBackground();  // fills in background with cat images
    }

	void Update () {
	
	}

    // gets all cat urls
    void GenBackground()
    {
        // destroys old web images
        foreach (GameObject img in imgs) Destroy(img);

        // empty lists holding urls and web images
        imgs.Clear();
        urls.Clear();

        // start coroutines to get more urls
        for (var i = 0; i < 20; i++) StartCoroutine(AddUrl());
    }

    // creates cat background
    void BuildBackground()
    {
        // a web image is created for each url
        var i = 0;
        foreach(string url in urls)
        {
            // create web image
            GameObject img = Instantiate(webimgObject);
            RectTransform trans = img.GetComponent<RectTransform>();
            imgs.Add(img);

            // modify size
            img.GetComponent<WebImage>().SetImage(url);
            trans.sizeDelta = new Vector2(width/3, height/3);
            
            // correct positioning and parent
            img.transform.SetParent(transform);
            trans.localScale = new Vector3(1, 1, 1);
            if (i < 3) trans.localPosition = new Vector3(i*width/3,0);
            else if(i < 6) trans.localPosition = new Vector3((i - 3) * width / 3, -height/3);
            else trans.localPosition = new Vector3((i - 6) * width / 3, -height / 3 * 2);

            // add button componeent
            Button button = img.AddComponent<Button>();
            button.onClick.AddListener(delegate { OnClick(img, button); });

            i++;
        }
    }

    IEnumerator AddUrl()
    {
        // stop if enough valid urls have been found
        if (urls.Count > 8) yield break;

        WWW www = new WWW("http://random.cat/meow");
        yield return www;

        // scraping the picture path
        string formatted = www.text.Replace("{\"file\":\"http:\\/\\/random.cat\\/i\\/","").Replace("\"}","");

        // add url if valid, so if not a gif
        if(!formatted.Contains(".gif")) urls.Add("http://random.cat/i/" + formatted);

        // initiate building when enough urls have been found
        if (urls.Count == 9) BuildBackground();
    }

    // web image onclick function
    void OnClick(GameObject img, Button button)
    {
        // start expanding selected image
        currentImage = img;
        img.GetComponent<RectTransform>().localPosition = new Vector3(0,0);
        img.GetComponent<Animator>().SetTrigger("expand");
        button.enabled = false;

        // disable all other images
        foreach (GameObject image in imgs) if (image != img) image.SetActive(false);

        // play meow sound
        audio.PlayOneShot(meowSound, 1);

        // start ad one second after the user clicks
        Invoke("ShowAdResponse", 2);
    }

    // method to initiate the advertisement service
    IEnumerator InitAds()
    {
        // check if current platform supprots Unity Ads
        if (!Advertisement.isSupported)
        {
            print("Ads not supported.");
            yield break;
        }

        // wait for service to start up
        while (!Advertisement.isInitialized || !Advertisement.IsReady())
        {
            yield return new WaitForSeconds(0.5f);
        }

        print("Ads initialized.");
    }
    
    // shows an ad with a callback method
    public void ShowAdResponse()
    {
        // creating and assigning callback to ad
        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        // pops up the ad, with the zoneid left null
        Advertisement.Show(null, options);
    }

    // callback method for ads
    private void HandleShowResult(ShowResult result)
    {
        GameObject textBoxObj = transform.parent.Find("Middle Text Box").gameObject;  // obj container
        Text textBox = textBoxObj.transform.Find("Text").GetComponent<Text>();        // center textbox that displayes results

        // enables center text box, displays win results, plays sound
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

        // after 2 seconds, reset the game
        Invoke("Reset", 2f);
    }

    // resets the game back to initial conditions
    void Reset()
    {
        transform.parent.Find("Middle Text Box").gameObject.SetActive(false);
        GenBackground();
    }

}
