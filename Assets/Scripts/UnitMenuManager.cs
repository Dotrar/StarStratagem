using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class UnitMenuManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject buttonPrefab;

    [Range(0.01f, 1f)]
    public float buttonSpacing = 0.02f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateMenuItems( StarUnit unit)
    {
        Debug.Log("Creating Menu:");
        int indx = 0;
        foreach (var action in unit.actions)
        {
            GameObject obj = Instantiate(buttonPrefab,transform);
            Text textObject = obj.transform.Find("Text").GetComponent<Text>();
            Button buttonObj = obj.transform.GetComponent<Button>();
            //configure this button as required
            textObject.text = action.name;
            buttonObj.onClick.AddListener(CancelDeSelect);
            buttonObj.onClick.AddListener(action.callback);
            
            //TODO i don't think this works properly. semi linear layout.
            obj.transform.Translate(0, buttonSpacing * indx, 0);
            Debug.Log("Creating " + action.name);
            indx += 1;

        }

    }

    void CancelDeSelect()
    {
        Debug.Log("added to every callback");
    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
