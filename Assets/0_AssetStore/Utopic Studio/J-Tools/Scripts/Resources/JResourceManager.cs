using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mode that this Application is running
/// </summary>
public enum ApplicationMode
{
    Normal,
    Preview
};

/// <summary>
/// Singleton class manager for the resources.
/// </summary>
[AddComponentMenu("J/Resources/ResourceManager")]
public class JResourceManager : MonoBehaviour
{
    /* Singleton getters */
    public static JResourceManager Instance { get; private set; }

    /* Server URL */
    public string FetchURL = "http://localhost:8000/";

    /* The current application mode */
    public ApplicationMode AppMode { get; private set; }

    /* Resource code to search when in preview mode */ 
    public string PreviewResourceCode { get; private set; }

    /* Id for the player to search for */
    public int PlayerId = 1;

    void Awake()
    {
        //As this class is a singleton, we only allow one instance to be alive.
        if (Instance == null)
        {
            //Setup singleton values
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //This will initialize important values, like the application mode, etc
            InitWebArguments();

            //Will need to manage level loading for resources
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelLoaded;
        }
        else if (Instance != this)
        {
            //We already have a manager, eliminate this one and maintain the original.
            Destroy(gameObject);
        }

    }

    void InitWebArguments()
    {
        //We initialize the value first, we could be on a build that doesn't support web argument passing, in this case we fallback to normal mode
        AppMode = ApplicationMode.Normal;

        //Obtain the sections that conform this url (separated by the GET char)
        string[] UrlSections = Application.absoluteURL.Split('?');
        Debug.Log("Obtained application url:" + Application.absoluteURL);
        if (UrlSections.Length > 1)
        {
            //Arguments are on the second section of this URL
            string[] Arguments = UrlSections[1].Split('&');
            foreach (string Arg in Arguments)
            {
                //We need a final pass on the arguments, 
                string[] KeyValue = Arg.Split('=');
                ParseWebArgument(KeyValue[0], KeyValue[1]);
            }
        }
    }

    /// <summary>
    /// Manually selects each significant value and stores it on this object.
    /// </summary>
    /// <param name="Key">Name of the associated argument</param>
    /// <param name="Value">Value of the argument, which needs to be parsed down to the required type</param>
    void ParseWebArgument(string Key, string Value)
    {
        if (Key.ToLower() == "mode")
        {
            switch (Value.ToLower())
            {
                case "normal":
                    AppMode = ApplicationMode.Normal;
                    break;
                case "preview":
                    AppMode = ApplicationMode.Preview;
                    break;
                default:
                    AppMode = ApplicationMode.Normal;
                    break;
            }

            Debug.Log("Entered application mode: " + AppMode);
        }
        else if (Key.ToLower() == "code")
        {
            PreviewResourceCode = Value;
        }
    }

    /// <summary>
    /// Obtains the resource codes currently available on the scene
    /// </summary>
    /// <param name="MappedResources">A search map for each resource, indexed with its code. </param>
    /// <returns>An array of the resource codes currently available. </returns>
    string[] GetResourceCodes(out Dictionary<string, JResource> MappedResources)
    {
        //Initialize the out parameter
        MappedResources = new Dictionary<string, JResource>();

        //Get all the resources
        JResource[] resources = GameObject.FindObjectsOfType<JResource>();

        //Map each code to the resource
        List<string> codes = new List<string>();
        foreach (JResource r in resources)
        {
            if (!MappedResources.ContainsKey(r.Code))
            {
                codes.Add(r.Code);
                MappedResources.Add(r.Code, r);
            }
        }

        return codes.ToArray();
    }

    // Runs each time a scene is loaded, even when script is using DontDestroyOnLoad().
    // Finds a GameObject tagged with 'Player'. Needs to find exactly one.
    void OnLevelLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        //Start loading resources
        StartCoroutine(LoadResources());

        //Optionally force opening of preview resource
        if (JResourceManager.Instance.AppMode == ApplicationMode.Preview)
        {
            string ResourceCode = JResourceManager.Instance.PreviewResourceCode;
            JResource[] resources = GameObject.FindObjectsOfType<JResource>();

            foreach (JResource r in resources)
            {
                if (r.Code == ResourceCode)
                {
                    r.Show();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Obtains the resource codes currently available on the scene
    /// </summary>
    /// <returns>An array of the resource codes currently available. </returns>
    string[] GetResourceCodes()
    {
        Dictionary<string, JResource> MappedResources;
        return GetResourceCodes(out MappedResources);
    }

    /// <summary>
    /// Loads every resource available on the level and updates their runtime info with the remote DB data.
    /// </summary>
    /// <returns>Enumerator for CoRoutine</returns>
    IEnumerator LoadResources()
    {
        Debug.Log("Loading resources...");

        //Obtain all the available codes
        Dictionary<string, JResource> MappedResources;
        string[] codes = GetResourceCodes(out MappedResources);

        //Do a WWW call to the web server
        string url = FetchURL + "jugadores/" + PlayerId + "/recursos/" + string.Join(",", codes);
        Debug.Log("Calling URL:" + url);
        using (WWW www = new WWW(url))
        {
            yield return www;

            //Obtain the json data dictionary
            JSONObject JsonData = new JSONObject(www.text);

            //Data is separated as a dictionary
            if (JsonData.type == JSONObject.Type.OBJECT)
            {
                for (int i = 0; i < JsonData.list.Count; i++)
                {
                    string key = (string)JsonData.keys[i];
                    JSONObject j = (JSONObject)JsonData.list[i];

                    JResource r = null;
                    MappedResources.TryGetValue(key, out r);
                    if (r)
                    {
                        r.SetupFromJsonData(j);
                    }
                }
            }
        }
    }
}
