using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonType
{

    Hidden = -2,
    Misc = -1,
    Skill, // Default value 0
    Survival,   // Value 1
    Armor,   // Value 2
    Buildings,
    Tools,
    Guns,
    Nature,
    Medicine,
    Materials,
    Cables,
}

public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    public Button button;

    public RawImage rawImage;

    public string image_source;
    public ButtonType buttonType;
    public void OnPointerEnter(PointerEventData eventData)
    {
       button.buttonManager.HandleButtonEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        button.buttonManager.HandleButtonExit(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        button.buttonManager.HandleButtonDown(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       button.buttonManager.HandleButtonUp(this);
    }
    public void UpdateButtonImage(int state) {
        string image;
        if (state == 0){
            image = image_source + "-out";
        } else if (state == 1){
            image = image_source + "-in";
        } else if (state == 2){
            image = image_source + "-click";
        } else {
            Debug.LogError($"Failed to load button image with state: {state}");
            return;
        }

        Texture2D loadedTexture = button.buttonManager.localPlayer.player.inventory.LoadTextureFromPath("img/"+image);
        if (loadedTexture != null)
        {
            rawImage.texture = loadedTexture;
            rawImage.enabled = true;
        }
            else
        {
            Debug.LogError($"Failed to load texture from path: {image}");
        }
    }
}
