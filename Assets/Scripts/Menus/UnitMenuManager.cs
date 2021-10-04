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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateMenuItems( StarUnit unit)
    {
        foreach (var action in unit.actions)
        {
            // Create game objects
            GameObject obj = Instantiate(buttonPrefab, transform);
            Text textObject = obj.transform.Find("Text").GetComponent<Text>();
            Button buttonObj = obj.transform.GetComponent<Button>();
            
            
            // Configure this button
            obj.name = $"{action.Name} Button"; //show in editor
            textObject.text = action.Name;
            // buttonObj.onClick.AddListener(CancelDeSelect);
            buttonObj.onClick.AddListener(action.Callback);
            buttonObj.onClick.AddListener(AfterEveryButton);
        }

    }

    void AfterEveryButton()
    {
        // Hide the menu so that it doesn't get in the way of the selection;
        //SetActive(false);
    }

    void CancelDeSelect()
    {
        /*
         TODO is a CancelDeSelect going to be a better solution than the Async(1) ?
         */
    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
