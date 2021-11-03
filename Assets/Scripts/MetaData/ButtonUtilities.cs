using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class ButtonUtilities : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] Button[] itemsIds;
    [SerializeField] ButtonConfig buttonConfig;
    [System.Serializable]
    public struct ButtonConfig{
        public Color selected, unselected;
        public Sprite check, uncheck;
    }
    public Dictionary<int, Button> buttons { get; set; }
    void Awake()
    {
        buttons=new Dictionary<int, Button>();
        foreach(var e in itemsIds){
            int Id=e.GetComponent<ButtonID>().ID;
            Button button = e.GetComponent<Button>();
            buttons.Add(Id, button);
        }
    }
    public void SetButton(int iD, bool isSelected)
    {
        Button button = buttons[iD];
        button.gameObject.SetActive(true);
        if (isSelected)
        {
            button.image.color = buttonConfig.selected;
            button.gameObject.GetChild(0).GetComponent<Image>().sprite = buttonConfig.check;
            button.GetComponent<ButtonID>().selected = true;
        }
        else
        {
            button.image.color = buttonConfig.unselected;
            button.gameObject.GetChild(0).GetComponent<Image>().sprite = buttonConfig.uncheck;
            button.GetComponent<ButtonID>().selected = false;
        }
    }
    public Button GetButton(int iD)
    {
        if (buttons.ContainsKey(iD))
        {
            return buttons[iD];
        }
        else return null;
    }
}