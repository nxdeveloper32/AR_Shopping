using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScreen : MonoBehaviour
{
    // Start is called before the first frame update

    public void Watch()
    {
        ProjectManager.instance.LoadScene(SceneType.Watch);
    }
    public void Fridge()
    {
        ProjectManager.instance.LoadScene(SceneType.Fridge);
    }
    public void SmartCard()
    {
        ProjectManager.instance.LoadScene(SceneType.Card);
    }
}
