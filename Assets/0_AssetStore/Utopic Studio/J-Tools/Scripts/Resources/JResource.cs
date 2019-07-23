using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TypeGameObjectValuePair : System.Object
{
    public ContentType Key;
    public GameObject Value;
}

/// <summary>
/// The type of content that conforms a resource page.
/// </summary>
public enum ContentType
{
    Text,
    Question,
    Assertion,
    Pairs
}

/// <summary>
/// Main class for the dynamic resource.
/// </summary>
[AddComponentMenu("J/Resources/Resource")]
public class JResource : MonoBehaviour {
    public UnityEvent OnShownEvent;
    public UnityEvent OnHiddenEvent;

    public bool bOpenOnStart = false;
    private bool bShown = false;
    private bool bDeferedOpen = false;

    /// <summary>
    /// Internal class that holds the logic for page on this resource
    /// </summary>
    class ContentPage
    {
        /// <summary>
        /// Generates a content page given a JsonObject 
        /// </summary>
        /// <param name="Data">JsonObject holding the data</param>
        /// <returns> All the content pages generated via the parsed json object.</returns>
        public static ContentPage[] ParseFromJsonData(JSONObject Data)
        {
            List<ContentPage> ListC = new List<ContentPage>();
            foreach (JSONObject j in Data.list)
            {
                ListC.Add(new ContentPage(j));
            }

            return ListC.ToArray();
        }

        public ContentPage(JSONObject Data)
        {
            for (int i = 0; i < Data.list.Count; i++)
            {
                string key = (string)Data.keys[i];
                JSONObject j = (JSONObject)Data.list[i];

                //How we parse the json data depends of the type of content type we're receiving
                switch (key)
                {
                    case "data":
                        this.Data = System.Text.RegularExpressions.Regex.Unescape(j.str);
                        break;
                    case "imagen":
                        this.ImagenURL= j.str.Replace("\\","");
                        break;
                    case "tipo":
                        this.Type = ParseType(j.str);
                        break;
                    case "opciones":
                        this.Options = ContentOption.ParseFromJsonData(j);
                        break;
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// Obtains a content type given the raw string data
        /// </summary>
        /// <param name="Type">String with the type to be parsed</param>
        /// <returns>Content type that pairs with the data given, defaults to TEXT</returns>
        private ContentType ParseType(string Type)
        {
            switch(Type)
            {
                case "texto":
                    return ContentType.Text;
                case "pregunta":
                    return ContentType.Question;
                case "vof":
                    return ContentType.Assertion;
                case "pares":
                    return ContentType.Pairs;
                default:
                    return ContentType.Text;
            }
        }

        /// <summary>
        /// Tries to obtain a remote image if its already cached, or being downloading if not
        /// </summary>
        /// <param name="Instigator">Resource who needs to be updated when the image is loaded.</param>
        /// <returns>The cached image, or null if it hasn't been downloaded yet.</returns>
        public Texture2D TryGetImage(JResource Instigator)
        {
            if (Imagen)
            {
                return Imagen;
            }
            else
            {
                //Image not loaded yet will query this object to receive the image when we load it
                BeginLoadImage(Instigator);
                return null;
            }
        }

        /// <summary>
        /// Starts remote loading of an image, if no image is currently being loaded.
        /// </summary>
        /// <param name="Instigator">Resource who needs to be updated when the image is loaded.</param>
        private void BeginLoadImage(JResource Instigator)
        {
            if (!IsLoadingImage)
            {
                CallbackLoader = Instigator;
                IsLoadingImage = true;
                Instigator.StartCoroutine(LoadImage());
            }
        }

        /// <summary>
        /// Async loading of the content page main image.
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadImage()
        {
            Debug.Log("Starting Laod Image from url: " + ImagenURL);
            using (WWW www = new WWW(ImagenURL))
            {
                yield return www;

                if (www.error != null)
                {
                    Debug.LogError(www.error);
                    IsLoadingImage = false;
                }
                else
                {
                    Imagen = www.texture;
                    FinishLoadImage();
                }
            }
        }

        /// <summary>
        /// Calls delegates when an image has completely loaded
        /// </summary>
        private void FinishLoadImage()
        {
            CallbackLoader.OnImageLoaded(Imagen, this);
            IsLoadingImage = false;
            Debug.Log("Image Loaded: " + Imagen);
        }


        /// <summary>
        /// The type of content this page holds
        /// </summary>
        public ContentType Type;

        /// <summary>
        /// General main data field for this page.
        /// </summary>
        public string Data;

        /// <summary>
        /// Url of the support image of this page.
        /// </summary>
        public string ImagenURL;

        /// <summary>
        /// Options for this page, if it has any question to be answered.
        /// </summary>
        public ContentOption[] Options;
        
        /// <summary>
        /// Support image that gets loaded Async from the ImageURL
        /// </summary>
        private Texture2D Imagen;

        /// <summary>
        /// Object that requested the load of the image, and wishes to be notified when it's completed.
        /// </summary>
        private JResource CallbackLoader;

        /// <summary>
        /// Flag to know if we're currently loading the secondary image
        /// </summary>
        private bool IsLoadingImage;
    }

    public class ContentOption
    {
        public static ContentOption[] ParseFromJsonData(JSONObject Data)
        {
            List<ContentOption> ListC = new List<ContentOption>();
            foreach (JSONObject j in Data.list)
            {
                ListC.Add(new ContentOption(j));
            }

            return ListC.ToArray();
        }

        public ContentOption(JSONObject Data)
        {
            for (int i = 0; i < Data.list.Count; i++)
            {
                string key = (string)Data.keys[i];
                JSONObject j = (JSONObject)Data.list[i];

                //Setup the value depending the data type
                switch (key)
                {
                    case "data":
                        this.Data = j.str;
                        break;
                    case "data_secundaria":
                        this.ExtraData = j.str;
                        break;
                    case "tipo":
                        this.Type = j.str;
                        break;
                }
            }
        }

        /// <summary>
        /// Main data for this option
        /// </summary>
        public string Data;

        /// <summary>
        /// Extra data that can be added for supporting the main data. 
        /// </summary>
        public string ExtraData;

        /// <summary>
        /// Type of content option.
        /// </summary>
        public string Type;
    }
    
    /// <summary>
    /// Represents this resource code on the remote webservice
    /// </summary>
    public string Code;

    /// <summary>
    /// Prefabs to use for each Type of content option when generating a page.
    /// </summary>
    public TypeGameObjectValuePair[] BlueprintPrefabs;

    /// <summary>
    /// Indexed dictionary for ease of use.
    /// </summary>
    private Dictionary<ContentType, GameObject> _blueprintPrefabs;

    /// <summary>
    /// Main title for this resource, gets rendered as a header.
    /// </summary>
    private string Title;

    /// <summary>
    /// Description for this resource. Data only, doesn't get rendered.
    /// </summary>
    private string Description;

    /// <summary>
    /// Ordered list of content pages that conform this resource
    /// </summary>
    private ContentPage[] Pages;

    /// <summary>
    /// Temporary image to use when we're still loading an image.
    /// </summary>
    public Texture2D LoadingImage;

    //The detail zoom to use
    //private ZoomInDetail DetailZoom; //@TODO: this will be removed most likely

    /// <summary>
    /// Index of the current page we're showing right now
    /// </summary>
    private int CurrentPage;

    //The UI to show
    public GameObject UI;

    //UI Title
    private UnityEngine.UI.Text TitleText;

    //UI Detail
    private UnityEngine.UI.Text DetailText;

    //UI Image Wrapper
    private GameObject ImageWrapper;

    //UI Image
    private UnityEngine.UI.Image ContentImage;

    //UI Image Wrapper
    private GameObject OptionListWrapper;

    /// <summary>
    /// The listview controller of the option list
    /// </summary>
    private J.JListViewController ListController;

    //Generates the data from the json string given
    public void SetupFromJsonData(JSONObject Data)
    {
        for (int i = 0; i < Data.list.Count; i++)
        {
            string key = (string)Data.keys[i];
            JSONObject j = (JSONObject)Data.list[i];

            //Setup the value depending of which 
            switch(key)
            {
                case "nombre":
                    this.Title = j.str;
                    break;
                case "descripcion":
                    this.Description = j.str;
                    break;
                case "contenidos":
                    //This is an array
                    Pages = ContentPage.ParseFromJsonData(j);
                    break;
                default:
                    break;
            }
            
        }
        
        //The title is common, so we should set it up here
        TitleText.text = Title;

        //Check if we're open, we should reset the current page
        if (bShown)
            GoToPage(CurrentPage);
    }

    void Awake()
    {
        //DetailZoom = GetComponent<ZoomInDetail>();

        //UI Components, search for the text ones
        UnityEngine.UI.Text[] TextComponents = transform.GetComponentsInChildren<UnityEngine.UI.Text>(true);
        foreach(UnityEngine.UI.Text T in TextComponents)
        {
            if (T.gameObject.CompareTag("ResourceTitle"))
            {
                TitleText = T;
            }
            else if (T.gameObject.CompareTag("ResourceDetail"))
            {
                DetailText = T;
            }
        }

        //Image objects and wrappers
        UnityEngine.UI.Image[] ImageComponents = transform.GetComponentsInChildren<UnityEngine.UI.Image>(true);
        foreach (UnityEngine.UI.Image T in ImageComponents)
        {
            if (T.gameObject.CompareTag("ResourceImageWrapper"))
            {
                ContentImage = T;
                ImageWrapper = ContentImage.transform.parent.gameObject;
                break;
            }
        }

        //Look for the optionlist
        ListController = transform.GetComponentInChildren<J.JListViewController>(true);
        OptionListWrapper = ListController.transform.parent.gameObject; //Looks awful, but the scroll has always this structure


        if (!TitleText || !DetailText || !ImageWrapper || !OptionListWrapper || !ListController)
        {
            Debug.LogError("UI Components not found, maybe you changed their tag?");
        }

        /*if (!DetailZoom)
        {
            Debug.LogError("ZoomInDetail component not found, cannot show");
        }*/

        if (!UI)
        {
            Debug.LogError("No UI Component bound, cannot continue");
        }

        //Generate the dictionary using the serializable keyvalue pair
        _blueprintPrefabs = new Dictionary<ContentType, GameObject>();
        foreach(TypeGameObjectValuePair KV in BlueprintPrefabs)
        {
            _blueprintPrefabs.Add(KV.Key, KV.Value);
        }

        //Manually hide, we don't want to start showing resources without them being selected
        UI.SetActive(false);
    }

    private void Start()
    {
        //Check for auto open config, only when on normal application mode
        bDeferedOpen = bOpenOnStart && JResourceManager.Instance.AppMode == ApplicationMode.Normal;
    }

    private void Update()
    {
        if(bDeferedOpen)
        {
            Show();
            bDeferedOpen = false;
        }
    }

    /// <summary>
    /// Shows the resource, displaying the first page if not specified and moving the player to the "Interest" point
    /// </summary>
    public void Show(int ShowPage = 0)
    {
        //Has to be first... if not, the internal IEnumerator from the yield will not run (as its deactivated)
        UI.SetActive(true);

        //Show the page if valid, if not, fallback to first page
        if (!GoToPage(ShowPage))
        {
            GoToPage(0);
        }

        //Show the detail zoom, which will trigger the movement and all
        //DetailZoom.ShowZoomDetail();
        //@TODO: detail zoom changed

        //Invoke the event
        OnShownEvent.Invoke();
        bShown = true;
    }

    /// <summary>
    /// Hides this control, returning control and position to the controller
    /// </summary>
    public void Hide()
    {
        //Unzoom the view
        //DetailZoom.ExitZoomDetail();
        //@TODO: detail zoom removed

        UI.SetActive(false);

        //Invoke the event
        OnHiddenEvent.Invoke();
        bShown = false;
    }

    /// <summary>
    /// Goes to the specified page and configures it accordingly
    /// </summary>
    /// <param name="InPage">The index of the page to go to</param>
    /// <returns>If the page was indeed changed</returns>
    private bool GoToPage(int InPage)
    {
        if (Pages == null || InPage < 0 || InPage >= Pages.Length)
        {
            return false;
        }

        //Update the current index
        CurrentPage = InPage;

        //Change the content here
        ContentPage currentContent = Pages[InPage];
        DetailText.text = currentContent.Data;
        
        //Try to setup the image if there is one
        if(currentContent.ImagenURL.Length > 0)
        {
            //First we should enable the image wrapper
            ImageWrapper.SetActive(true);

            //There's an image url, should try to get it if it exists
            Texture2D image = currentContent.TryGetImage(this);
            if (image)
            {
                ContentImage.overrideSprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0, 0));
            }
            else
            {
                ContentImage.overrideSprite = Sprite.Create(LoadingImage, new Rect(0,0, LoadingImage.width, LoadingImage.height), new Vector2(0, 0));
            }

        }
        else
        {
            //No image wrapper needed
            ImageWrapper.SetActive(false);
        }

        //Check if we should update the list area
        if(UsesOptionListArea(currentContent))
        {
            OptionListWrapper.SetActive(true);
            ListController.Clear();

            FillOptionList(currentContent);
        }
        else
        {
            OptionListWrapper.SetActive(false);
        }

        return true;
    }

    /// <summary>
    /// Goes to the next page if valid, and configures it accordingly
    /// </summary>
    public void NextPage()
    {
        GoToPage(CurrentPage + 1);
    }

    /// <summary>
    /// Goes to the previous page if valid, and configures it accordingly
    /// </summary>
    public void PrevPage()
    {
        GoToPage(CurrentPage - 1);
    }

    private bool UsesOptionListArea(ContentPage content)
    {
        return content.Type == ContentType.Question|| content.Type == ContentType.Assertion || content.Type == ContentType.Pairs;
    }

    private void FillOptionList(ContentPage content)
    {
        IRenderOptionFactory factory = _blueprintPrefabs[content.Type].GetComponent<JRenderOption>().GetFactory();
        JRenderOption[] options = factory.BuildRenderOptions(content.Options);

        foreach(JRenderOption Opt in options)
        {
            ListController.Add(Opt.gameObject);
        }

    }

    /// <summary>
    /// Called when we asked for an image when it was still loading and now has completed
    /// </summary>
    /// <param name="Image">The image loaded</param>
    /// <param name="From">The caller</param>
    private void OnImageLoaded(Texture2D Image, ContentPage From)
    {
        //Check that we still want this image (could have changed page)
        if (Pages[CurrentPage] == From)
        {
            ContentImage.overrideSprite = Sprite.Create(Image, new Rect(0, 0, Image.width, Image.height), new Vector2(0, 0));
        }
    }
}
