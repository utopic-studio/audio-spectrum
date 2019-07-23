using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[Serializable]
public class ResourceBakedEntry
{
    public ResourceBakedEntry(string InCode, string InPath)
    {
        Code = InCode;
        Path = InPath;
    }

    /// <summary>
    /// Code for the index entry
    /// </summary>
    public string Code;

    /// <summary>
    /// Path for the index entry
    /// </summary>
    public string Path;
}

/// <summary>
/// Holds a resource registry index for knowing in which stage to get an specific resource by its code.
/// Can be baked, which will sweep every BuildStage and queue their existent resources
/// </summary>
[AddComponentMenu("J/Resources/ResourceIndex")]
public class JResourceIndex : MonoBehaviour
{
    /// <summary>
    /// Map holding the index of this object, in the form of {Code, StagePath}
    /// </summary>
    private Dictionary<string, string> IndexMap;

    /// <summary>
    /// List of baked resources for user clarity. They aren't used besides data display.
    /// </summary>
    [ReadOnly]
    public List<ResourceBakedEntry> BakedResources;

#if UNITY_EDITOR
    /// <summary>
    /// Object that is responsible of baking the resources, as it can change scenes and don't get deleted
    /// </summary>
    private static ResourceBaker Baker = new ResourceBaker();

    /// <summary>
    /// Start the baking process
    /// </summary>
    public void Bake()
    {
        Baker.BeginBake(gameObject);
    }
    
    /// <summary>
    /// Called when the baking is completed
    /// </summary>
    /// <param name="IndexMap">New index map to use</param>
    public void OnCommitBake(Dictionary<string, string> NewIndexMap)
    {
        Undo.RecordObject(this, "Bake resource index");

        //Create baked entries
        BakedResources.Clear();
        foreach(KeyValuePair<string,string> KVPair in NewIndexMap)
        {
            BakedResources.Add(new ResourceBakedEntry(KVPair.Key, KVPair.Value));
        }
    }
#endif

    private void Awake()
    {
        IndexMap = new Dictionary<string, string>(BakedResources.Count);
        //We need to serialize index mappings
        foreach(ResourceBakedEntry Entry in BakedResources)
        {
            IndexMap.Add(Entry.Code, Entry.Path);
        }
    }

    /// <summary>
    /// Obtains the scene path for a given baked resource
    /// </summary>
    /// <param name="ResourceCode">Resource code to search for</param>
    /// <param name="ScenePath">Out scene path</param>
    /// <returns>If the resource path was found. </returns>
    public bool FindResourceScene(string ResourceCode, out string ScenePath)
    {
        return IndexMap.TryGetValue(ResourceCode, out ScenePath);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Internal class specialized on building the bake index for the resources.
    /// </summary>
    private class ResourceBaker : UnityEngine.Object
    {
        struct ResourceBakeContext
        {
            public ResourceBakeContext(string Name, string Scene)
            {
                CallerName = Name;
                CallerScene = Scene;
            }

            /// <summary>
            /// Name that we can use to get a reference of the caller, once we finish baking
            /// </summary>
            public string CallerName { get; private set; }

            /// <summary>
            /// Scene from which the baking was inited
            /// </summary>
            public string CallerScene { get; private set; }
        }

        /// <summary>
        /// Current index map being baked
        /// </summary>
        private Dictionary<string, string> IndexMap;

        /// <summary>
        /// Build scenes that we're currently baking
        /// </summary>
        private UnityEditor.EditorBuildSettingsScene[] BuildScenes;

        /// <summary>
        /// Current scene we're baking
        /// @see BuildScenes
        /// </summary>
        private int CurrentBakeScene;

        /// <summary>
        /// Holds the context in which the bake was requested
        /// </summary>
        private ResourceBakeContext BakeContext;
        
        /// <summary>
        /// Starts the baking process
        /// </summary>
        /// <param name="Context">Context object that called the bake</param>
        public void BeginBake(GameObject Context)
        {
            //Make sure we have a clean bake
            IndexMap = new Dictionary<string, string>();
            BuildScenes = UnityEditor.EditorBuildSettings.scenes;
            CurrentBakeScene = 0;

            //Store context
            BakeContext = new ResourceBakeContext(Context.name, UnityEngine.SceneManagement.SceneManager.GetActiveScene().path);

            if (BuildScenes.Length > 0)
            {
                //Will need to manage level loading for resources
                UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnBakeLevelOpened;

                //Begin baking, we will start with the current bake scene
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(BuildScenes[CurrentBakeScene].path);
            }
        }

        private void EndBake()
        {
            //This isn't strictly needed, but it helps a lot with readability
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= OnBakeLevelOpened;
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnFinishBakeLevelOpened;

            //Show loading bar
            EditorUtility.DisplayProgressBar("Baking resources...", "Finishing Bake...", 1.0f);

            //Finally open the context scene
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(BakeContext.CallerScene);
        }

        /// <summary>
        /// Called when loading a scene while on the baking process
        /// </summary>
        /// <param name="scene">Scene that was loaded</param>
        /// <param name="mode"> Mode in which the scene was loaded </param>
        void OnBakeLevelOpened(Scene scene, OpenSceneMode mode)
        {
            //Update progress
            EditorUtility.DisplayProgressBar("Baking resources...", "Searching available resources through BuildScenes...", ((float)CurrentBakeScene) / ((float)BuildScenes.Length));

            //Resources will most likely be set as inactive, so we need to obtain them using the scene manager
            GameObject[] Roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (GameObject go in Roots)
            {
                List<JResource> Resources = new List<JResource>();
                go.GetComponentsInChildren<JResource>(true, Resources);

                //Need to add to the baked list
                foreach (JResource r in Resources)
                {
                    IndexMap.Add(r.Code, scene.path);
                }
            }

            //Finished baking of this scene, request the next one
            if (BuildScenes.Length > ++CurrentBakeScene)
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(BuildScenes[CurrentBakeScene].path);
            }
            else
            {
                EndBake();
            }
        }

        /// <summary>
        /// Called when loading the finishing bake level, separated from previous method for clarity
        /// </summary>
        /// <param name="scene">Scene opened</param>
        /// <param name="mode">Mode in which the scene was opened</param>
        void OnFinishBakeLevelOpened(Scene scene, OpenSceneMode mode)
        {
            //No matter if we succeed or fail now, we don't need to react to scene changes until next bake
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= OnFinishBakeLevelOpened;

            GameObject Caller = GameObject.Find(BakeContext.CallerName);
            JResourceIndex Index = Caller != null ? Caller.GetComponentInChildren<JResourceIndex>() : null;
            if(Index)
            {
                Index.OnCommitBake(IndexMap);
                Debug.Log("Baking complete");
            }
            else
            {
                Debug.LogError("Baking failed, could not locate baking context caller when reloading scene. Most probably caller scene isn't part of the Build Scenes.");
            }

            //Finished, we clear the loading bar
            EditorUtility.ClearProgressBar();
        }
    }
#endif
}
