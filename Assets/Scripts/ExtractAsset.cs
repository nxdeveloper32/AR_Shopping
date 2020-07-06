#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

[CustomEditor (typeof(LoadAsset))]
public class ExtractAsset : Editor
{
    public static ExtractAsset instance;
    public TextureImporter assetImporter;

    private void Awake()
    {
        instance = this;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LoadAsset loadAsset = (LoadAsset)target;
        if (GUILayout.Button("Download Asset"))
        {
            loadAsset.DownloadFiles();
        }
        //if (GUILayout.Button("Asset"))
        //{
        //    loadAsset.StartTest();
        //}
        /*if (GUILayout.Button("FixAssets"))
        {
            SetupGlassMaterial("Assets/Resources/Watch1/Test.mat");
            
        }
        if (GUILayout.Button("Save"))
        {
            AssetDatabase.SaveAssets();
            //loadAsset.LoadSprite();
        }*/
    }
    public List<string> ExtractMaterials(string assetPath, string destinationPath, string modelTexture, bool isDialTexture, string dialTexture)
    {
        List<string> listOfMaterials = new List<string>();
        HashSet<string> hashSet = new HashSet<string>();
        IEnumerable<Object> enumerable = from x in AssetDatabase.LoadAllAssetsAtPath(assetPath)
                                         where x.GetType() == typeof(Material)
                                         select x;
        int i = 0;
        foreach (Object item in enumerable)
        {
            string path = Path.Combine(destinationPath, item.name) + ".mat";
            listOfMaterials.Insert(i, path);
            //path = AssetDatabase.GenerateUniqueAssetPath(path);

            string value = AssetDatabase.ExtractAsset(item, path);
            if (string.IsNullOrEmpty(value))
            {
                hashSet.Add(assetPath);
            }
            i++;
        }
        foreach (string item2 in hashSet)
        {
            AssetDatabase.WriteImportSettingsIfDirty(item2);
            AssetDatabase.ImportAsset(item2, ImportAssetOptions.ForceUpdate);
        }
        return listOfMaterials;
    }
    public void PostprocessTexture(string path)
    {
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }
    public void PostprocessMaterial(string materialPath, string texturePath)
    {
        Debug.Log(materialPath + " / " + texturePath);
        Material material = AssetDatabase.LoadAssetAtPath(materialPath , typeof(Material)) as Material;
        Texture texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture)) as Texture;
        material.shader = Shader.Find("Unlit/Transparent Cutout");
        material.SetTexture("_MainTex", texture);
        AssetDatabase.SaveAssets();

    }
    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
    public void SetupGlassMaterial(string path)
    {
        Material material = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
        material.shader = Shader.Find("Unlit/Transparent Cutout");
        Texture texture = Resources.Load<Texture>("Watch1/dial");
        material.SetTexture("_MainTex", texture);
        //AssetDatabase.CreateAsset(material, "materialPath" + "/mySkybox.mat");
        // Saves the copy of the material to the disk

        //AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        //AssetDatabase.SaveAssets();
    }
}
#endif