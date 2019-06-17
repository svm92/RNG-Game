using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHighlighter : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, IDeselectHandler, ISelectHandler {

	public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Player.keyboardModeOn = false;
        if (!EventSystem.current.alreadySelecting)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnDeselect(BaseEventData baseEventData)
    {
        GetComponent<Selectable>().OnPointerExit(null);
    }

    public void OnSelect(BaseEventData baseEventData)
    {
        if (Player.keyboardModeOn)
        {
            GameObject scrollbar = GameObject.Find("Scrollbar Vertical");
            if (scrollbar != null)
            {
                Transform parent = baseEventData.selectedObject.transform.parent;
                if (parent.name == "CollectionPanel")
                {
                    int index = 0;
                    for (int i = 0; i < parent.childCount; i++)
                    {
                        if (parent.GetChild(i).gameObject == baseEventData.selectedObject)
                        {
                            index = i;
                            break;
                        }
                    }
                    float row = Mathf.Ceil((index + 1) / 4f) - 1f;
                    float nRows = Mathf.Ceil(Player.collection.Count / 4f) - 1f;
                    if (nRows == 0) nRows = 1; // Avoid division by 0
                    scrollbar.GetComponent<Scrollbar>().value = 1 - (row / nRows); // 1 for upper row, 0 for lower
                }
                else if (parent.name == "FortuneContainer")
                {
                    int index = 0;
                    for (int i = 0; i < parent.childCount; i++)
                    {
                        if (parent.GetChild(i).gameObject == baseEventData.selectedObject)
                        {
                            index = i;
                            break;
                        }
                    }
                    // 1 for upper row, 0 for lower
                    scrollbar.GetComponent<Scrollbar>().value = 1 - (index / (float)(parent.childCount - 1));
                }
            }
        }
    }

}
