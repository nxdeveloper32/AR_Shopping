using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FridgeManager : MonoBehaviour
{
    public Text DisCount;
    public GameObject UserDefineObj;
    public List<GameObject> FridgeObjs;
    public bool isCalculatingDistance;
    public Camera ARCamera;
    public float minimumDistance;
    Animator myAnim;
    int i;
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
    }
    private void Update()
    {
        if (isCalculatingDistance)
        {
            var dis = Vector3.Distance(ARCamera.transform.position, UserDefineObj.transform.position);
            DisCount.text = dis.ToString();
            if (dis <= minimumDistance)
            {
                EnableDisableAnim(true);
            }
            else
            {
                EnableDisableAnim(false);
            }
        }
    }
    public void AssignFridgeObj()
    {
        for(int i =0; i< UserDefineObj.transform.childCount; i++)
        {
            FridgeObjs.Insert(i, UserDefineObj.transform.GetChild(i).gameObject);
        }
    }
    void EnableDisableAnim(bool isEnable)
    {
        myAnim = UserDefineObj.transform.GetChild(i).GetComponent<Animator>();
        myAnim.SetBool("IsFrigdgeOpen", isEnable);
    }
    public void nextFridge()
    {
        i++;
        if (i >= FridgeObjs.Count)
        {
            i = 0;
        }
        for (int j = 0; j < FridgeObjs.Count; j++)
        {
            FridgeObjs[j].SetActive(false);
        }
        FridgeObjs[i].SetActive(true);
    }
    public void previousFridge()
    {
        i--;
        if (i < 0)
        {
            i = FridgeObjs.Count-1;
        }
        for (int j = 0; j < FridgeObjs.Count; j++)
        {
            FridgeObjs[j].SetActive(false);
        }
        FridgeObjs[i].SetActive(true);

    }
    public void BackButon()
    {
        ProjectManager.instance.Back();
    }
}
