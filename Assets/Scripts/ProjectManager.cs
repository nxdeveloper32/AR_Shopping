using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ProjectManager : MonoBehaviour
{
    public static ProjectManager instance;
    public void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Back()
    {
        LoadScene(SceneType.Home);
    }
    public void LoadScene(SceneType scene)
    {
        SceneManager.LoadSceneAsync(scene.ToString());
    }
}
public enum SceneType
{
    Watch,
    Fridge,
    Card,
    Home,
}