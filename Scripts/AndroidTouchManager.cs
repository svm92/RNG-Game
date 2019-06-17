using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidTouchManager : MonoBehaviour {

    GameObject selectedCard = null;
    float stationaryTouchTime = 0;
    bool doubleTap = false;
    Vector2 slideDirection = Vector2.zero;
    Camera cam;

    private void Awake()
    {
        if (!Player.isAndroid()) // If not on Android, destroy the script
        {
            Destroy(this);
        }
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // If a finger touches the screen, detect if there's a card there
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 worldPoint = cam.ScreenToWorldPoint(touch.position);
                RaycastHit2D raycastHit2D = Physics2D.Raycast(worldPoint, Vector3.forward);
                if (gameObject.name == "BattleManager")
                {
                    // If it finds a gameObject with a CardObject component that is interactable
                    if (raycastHit2D.collider != null && raycastHit2D.collider.GetComponent<CardObject>() != null
                        && raycastHit2D.collider.gameObject.GetComponent<Button>().interactable)
                    {
                        selectedCard = raycastHit2D.collider.gameObject;
                    }
                } else if (gameObject.name == "DeckEditor")
                {
                    // If it finds a gameObject with a DeckEditorCardObject component
                    if (raycastHit2D.collider != null && raycastHit2D.collider.GetComponent<DeckEditorCardObject>() != null)
                    {
                        selectedCard = raycastHit2D.collider.gameObject;
                    }
                }
            }
            
            // Restart or advance timer
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                stationaryTouchTime = 0;
            } else if (touch.phase == TouchPhase.Stationary)
            {
                stationaryTouchTime += Time.deltaTime;
            }

            // Check for doubletaps
            doubleTap = (touch.tapCount > 1);

            // On finger lift, get slide direction
            if (touch.phase == TouchPhase.Ended)
            {
                slideDirection = getSlideDirection();
            }

            // Stop timer
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                stationaryTouchTime = 0;
            }
        }
        else // If not touching screen
        {
            setAllTouchVariablesToDefault();
        }

        if (selectedCard != null)
        {
            // If double tapping or sliding up
            if (gameObject.name == "BattleManager" && (doubleTap || slideDirection == Vector2.up))
            {
                CardObject cardObj = selectedCard.GetComponent<CardObject>();
                StartCoroutine(cardObj.playCardAsync());
                setAllTouchVariablesToDefault();
            } // If double tapping, or sliding right if in collection, or sliding left if in deck
            else if (gameObject.name == "DeckEditor" && (doubleTap || 
                (slideDirection == Vector2.right && selectedCard.transform.parent.name == "CollectionPanel") || 
                (slideDirection == Vector2.left && selectedCard.transform.parent.name == "DeckPanel")))
            {
                DeckEditorCardObject deco = selectedCard.GetComponent<DeckEditorCardObject>();
                deco.useCard();
                setAllTouchVariablesToDefault();
            }
        }
    }

    void setAllTouchVariablesToDefault()
    {
        doubleTap = false;
        slideDirection = Vector2.zero;
        selectedCard = null;
    }

    Vector2 getSlideDirection()
    {
        Touch touch = Input.GetTouch(0);
        Vector2 pixelDelta = touch.position - touch.rawPosition;
        // Final position - Original position
        // Check that final position is positive in x in respect to the original position. Don't count as slide if:
        // - Turned left in last moment
        // - Went too far up or down (25% of screen))
        // - Too much time passed without moving the finger (2.5s)
        if (pixelDelta.x > 0 && touch.deltaPosition.x > 0
            && Mathf.Abs(pixelDelta.y) < 0.25f * Screen.height && stationaryTouchTime < 2.5f)
        {
            return Vector2.right;
        } else
            if (pixelDelta.x < 0 && touch.deltaPosition.x < 0
            && Mathf.Abs(pixelDelta.y) < 0.25f * Screen.height && stationaryTouchTime < 2.5f)
        {
            return Vector2.left;
        } else
            if (pixelDelta.y > 0 && touch.deltaPosition.y > 0
            && Mathf.Abs(pixelDelta.x) < 0.25f * Screen.width && stationaryTouchTime < 2.5f)
        {
            return Vector2.up;
        } else
            if (pixelDelta.y < 0 && touch.deltaPosition.y < 0
            && Mathf.Abs(pixelDelta.x) < 0.25f * Screen.width && stationaryTouchTime < 2.5f)
        {
            return Vector2.down;
        }
        return Vector2.zero;
    }

}
