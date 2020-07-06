using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WatchScript : MonoBehaviour
{
    public Image watchImg;
    public List<Sprite> watchImgs;
    public List<GameObject> watchObjs;
    public GameObject sidePanel;
    public GameObject changeWatchBtn;
    public GameObject placeholder;
    public GameObject displayProduct;
    public GameObject currentObj;
    public GameObject backButton;
    public GameObject visualizeButton;
    public GameObject camerAR;
    public GameObject camera3D;
    public int posIndex;
    int i;
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        watchImg.sprite = watchImgs[0];
        watchObjs[0].SetActive(true);
    }

    public void nextWatch()
    {
        i++;
        if (i >= watchImgs.Count)
        {
            i = 0;
        }
        watchImg.sprite = watchImgs[i];
        for(int j = 0; j < watchImgs.Count; j++)
        {
            watchObjs[j].SetActive(false);
        }
        watchObjs[i].SetActive(true);
    }
    public void previousWatch()
    {
        i--;
        if (i < 0)
        {
            i = watchImgs.Count-1;
        }
        watchImg.sprite = watchImgs[i];
        for (int j = 0; j < watchImgs.Count; j++)
        {
            watchObjs[j].SetActive(false);
        }
        watchObjs[i].SetActive(true);

    }
    public void EnableDisableSidePanel(bool isActive)
    {
        if (isActive)
        {
            changeWatchBtn.SetActive(!isActive);
            sidePanel.SetActive(isActive);
        }
        else
        {
            changeWatchBtn.SetActive(!isActive);
            sidePanel.SetActive(isActive);
        }
        
    }
    GameObject FindObj()
    {
        GameObject temp = null;
        for(int i = 0; i< placeholder.transform.childCount; i++)
        {
            if (placeholder.transform.GetChild(i).gameObject.activeSelf)
            {
                temp = placeholder.transform.GetChild(i).gameObject;
                posIndex = i;
                break;
            }
        }
        EnableDisableAllRenderer(placeholder,true);
        return temp;
    }
    public void Visualize()
    {
        camera3D.SetActive(true);
        backButton.SetActive(true);
        camerAR.SetActive(false);
        visualizeButton.SetActive(false);
        currentObj = FindObj();
        currentObj.transform.SetParent(displayProduct.transform);
        currentObj.transform.localPosition = Vector3.zero;
        currentObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    public void CloseVisualize()
    {
        camera3D.SetActive(false);
        backButton.SetActive(false);
        camerAR.SetActive(true);
        visualizeButton.SetActive(true);
        EnableDisableAllRenderer(currentObj, false);
        currentObj.transform.SetParent(placeholder.transform);
        currentObj.transform.SetSiblingIndex(posIndex);
        currentObj.transform.localPosition = Vector3.zero;
        currentObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    public void EnableDisableAllRenderer(GameObject obj, bool isActive)
    {
        MeshRenderer[] MRs = obj.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in MRs)
        {
            mr.enabled = isActive;
        }
    }
    public void BackButon()
    {
        ProjectManager.instance.Back();
    }
}
