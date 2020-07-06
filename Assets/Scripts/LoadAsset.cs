#if (UNITY_EDITOR) 
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

public class LoadAsset : MonoBehaviour
{
    public WatchScript watchScriptInstance;
    public GameObject placeHolder;
    public GameObject imageTarget;
    public const string serverUrl = "http://arshopping.nxstudiosgames.com/uploads/";
    public const string unityRequestUrl = "http://arshopping.nxstudiosgames.com/unityrequest.php";
    public string puvlicapi;
    public string TexturePath;
    public const string resourcePath = "Assets/Resources/";
    // Start is called before the first frame update
    string[] Data;
    string modelFbxName;
    string modelTexture;
    string dialTexture;
    string modelImage;
    bool isDialTexture;
    bool loadNext;
    Task downloadingTask;
    List<string> matPath;
    string modelPath = "";
    string texturePath = "";
    string imagePath = "";
    string dialTexturePath = "";
    
    public void DownloadFiles()
    {
        
        DestroyImmediate(placeHolder);
        GameObject obj = new GameObject();
        obj.name = "PlaceHolder New";
        placeHolder = obj;
        watchScriptInstance.placeholder = obj;
        placeHolder.transform.parent = imageTarget.transform;
        placeHolder.transform.localPosition = Vector3.zero;
        placeHolder.transform.localScale = Vector3.one * .5f;
        StartCoroutine(GetDataFromServer());
    }
    IEnumerator GetDataFromServer()
    {
        Debug.Log("Connecting To Server...");
        using (UnityWebRequest www = UnityWebRequest.Get(unityRequestUrl))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Downloading...");
                StartCoroutine(SeperateData(www.downloadHandler.text));
            }
        }
    }
    IEnumerator SeperateData(string request)
    {
        string[] splitArray = request.Split(char.Parse(","));
        watchScriptInstance.watchImgs = new List<Sprite>();
        watchScriptInstance.watchObjs = new List<GameObject>();
        for (int i = 0; i < splitArray.Length - 1; i++)
        {
            matPath = new List<string>();
            PrepareData(splitArray[i]);
            yield return new WaitUntil(() => loadNext == true);
        }
        Debug.Log("Completed....");
    }
    void PrepareData(string data)
    {
        loadNext = false;
        string[] splitArray = data.Split(char.Parse("="));
        Data = splitArray[1].Split(char.Parse(":"));
        string sUrl = serverUrl + Data[0] + "/";
        modelFbxName = Data[5];
        modelTexture = Data[6];
        dialTexture = Data[7];
        modelImage = Data[8];
        StartCoroutine(DownloadAssetFromServer(sUrl, modelFbxName, Data[0]));
        StartCoroutine(DownloadAssetFromServer(sUrl, modelTexture, Data[0]));
        if (dialTexture != "-")
        {
            isDialTexture = true;
            StartCoroutine(DownloadAssetFromServer(sUrl, dialTexture, Data[0]));
        }
        else
        {
            isDialTexture = false;
        }
        downloadingTask = new Task(DownloadAssetFromServer(sUrl, modelImage, Data[0]));
        if (downloadingTask.Running)
        {
            StartCoroutine(StartFixingTask());
        }
        
    }
    IEnumerator StartFixingTask()
    {
        bool isComplete = false;
        while (!isComplete)
        {
            Debug.Log("Downloading...");
            if (!downloadingTask.Running)
            {
                UnityEditor.AssetDatabase.Refresh();
                Debug.Log("Asset Downloaded");
                Debug.Log("...............Fixing Asset...............");
                UnityEditor.AssetDatabase.Refresh();
                modelPath = resourcePath + Data[0] + "/" + modelFbxName;
                texturePath = resourcePath + Data[0] + "/" + modelTexture;
                ExtractAsset.instance.PostprocessTexture(texturePath);
                imagePath = resourcePath + Data[0] + "/" + modelImage;
                ExtractAsset.instance.PostprocessTexture(imagePath);
                
                dialTexturePath = "";
                if (isDialTexture)
                {
                    dialTexturePath = resourcePath + Data[0] + "/" + dialTexture;
                    ExtractAsset.instance.PostprocessTexture(dialTexturePath);
                }
                Debug.Log("...............Texture Fixed...............");
                
                Debug.Log("Extracting Materials...");
                matPath = ExtractAsset.instance.ExtractMaterials(modelPath, resourcePath + Data[0], texturePath, isDialTexture, dialTexturePath);
                int i = 0;
                foreach (string a in matPath)
                {
                    Debug.Log(a);
                }
                foreach (string a in matPath)
                {
                    string[] data = a.Split(char.Parse("\\"));
                    string materialPath = data[0] + "/" + data[1];
                    if (isDialTexture)
                    {
                        if (i == 1)
                        {
                            Debug.Log(materialPath + " Model Path " + Data[0] + "/" + modelTexture);
                            ExtractAsset.instance.PostprocessMaterial(materialPath, resourcePath + Data[0] + "/" + modelTexture);
                        }
                        else if (i == 0)
                        {
                            Debug.Log(materialPath + " Dial Path " + Data[0] + "/" + dialTexture);
                            ExtractAsset.instance.PostprocessMaterial(materialPath, resourcePath + Data[0] + "/" + dialTexture);
                        }
                        i++;
                    }
                    else
                    {
                        ExtractAsset.instance.PostprocessMaterial(materialPath, resourcePath + Data[0] + "/" + modelTexture);
                    }
                }
                UnityEditor.AssetDatabase.Refresh();
                isComplete = true;
                
                watchScriptInstance.watchImgs.Add(Resources.Load<Sprite>(RemoveExtension(Data[0] + "/" + modelImage)));
                UnityEditor.AssetDatabase.Refresh();
                UnityEditor.AssetDatabase.Refresh();
                Object gameObject = Resources.Load<GameObject>(RemoveExtension(Data[0] + "/" + modelFbxName));
                GameObject obj = (GameObject) Instantiate(gameObject, placeHolder.transform);
                obj.SetActive(false);
                
                watchScriptInstance.watchObjs.Add(obj);//watchScriptInstance.watchObjs.Count
                loadNext = true;
            }
            yield return null;
        }
    }
    string RemoveExtension(string path)
    {
        string fileName = path.Split(char.Parse("."))[0];
        return fileName;
    }
    IEnumerator DownloadAssetFromServer(string sUrl, string assetName, string targetFolder)
    {
        string[] FileData = assetName.Split(char.Parse("."));
        string fileName = FileData[0];
        string fileExtension = FileData[1];
        string url = sUrl + fileName+ "."+ fileExtension;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var folder = Directory.CreateDirectory(Application.dataPath + "/Resources/" + targetFolder);
                string savePath = string.Format("{0}/{1}."+ fileExtension, Application.dataPath + "/Resources/"+targetFolder, fileName);
                byte[] test = www.downloadHandler.data;
                File.WriteAllBytes(savePath, test);
                UnityEditor.AssetDatabase.Refresh();
            }
        }
    }
    public void FixAssets()
    {
        //ExtractAsset.instance.PostprocessTexture(texturePath);
        //ExtractAsset.instance.PostprocessTexture(imagePath);
        //ExtractAsset.instance.ExtractMaterials(modelPath, resourcePath + Data[0]);
        //string colors = Data[7].Split(char.Parse("-"));
        //dialTexturePath = resourcePath + Data[0] + "/" + dialTexture;
        //ExtractAsset.instance.PostprocessTexture(dialTexturePath);
        /*for (int j = 0; j < 2; j++)
        {
            Color color;
            ColorUtility.TryParseHtmlString(colors[j], out color);
            ExtractAsset.instance.PostprocessMaterial(resourcePath + Data[0] + "/" + (j + 1) + ".mat", color);
        }
        ExtractAsset.instance.SetupGlassMaterial(resourcePath + Data[0] + "/" + (3) + ".mat", Color.white);*/
        //StartCoroutine(StartProccess());
    }
    IEnumerator StartProccess()
    {
        
        yield return new WaitForSeconds(1f);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnityEditor.AssetDatabase.Refresh();
            // Object bullet_o = Resources.Load(filename);
            //GameObject Bullet = (GameObject)Instantiate(bullet_o);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            //StartCoroutine(DownloadAssetFromServer(filename));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
           // ExtractAsset.instance.ExtractMaterials("Assets/Models/watch1.fbx","Assets/Models/Materials");
        }
    }
    public void stst()
    {
        //Debug.Log("ttete");
        //List<string> listOfMaterials = new List<string>();
        //HashSet<string> hashSet = new HashSet<string>();
        //IEnumerable<Object> enumerable = from x in AssetDatabase.LoadAllAssetsAtPath(puvlicapi)
        //                                 where x.GetType() == typeof(Material)
        //                                 select x;
        //int i = 0;
        //foreach (Object item in enumerable)
        //{
        //    Debug.Log(item.name);
        //    //string path = Path.Combine(destinationPath, item.name) + ".mat";
        //    //listOfMaterials.Insert(i, path);
        //    ////path = AssetDatabase.GenerateUniqueAssetPath(path);

        //    //string value = AssetDatabase.ExtractAsset(item, path);
        //    //if (string.IsNullOrEmpty(value))
        //    //{
        //    //    hashSet.Add(assetPath);
        //    //}
        //    i++;
        //}
        //ExtractAsset.instance.SetupGlassMaterial(puvlicapi,TexturePath);
        /*DestroyImmediate(placeHolder);
        GameObject obj = new GameObject();
        obj.name = "PlaceHolder";
        placeHolder = obj;
        placeHolder.transform.parent = imageTarget.transform;*/
        //
    }
}
#endif