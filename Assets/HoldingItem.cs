using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class HoldingItem : NetworkBehaviour
{
    public Renderer quadRenderer; // Reference to the quad's renderer.

    public SlotItem slotItem;

    public void LoadImage(Texture2D tex)
    {
        //image.texture = tex;
        quadRenderer.material.mainTexture = tex;
        quadRenderer.material.shader = Shader.Find("Unlit/Transparent");
        float width = tex.width;
        float height = tex.height;

        // Scale the quad to match the texture's dimensions
        float scalingFactor = 0.007f; // Adjust this to change the overall size of the quad
        transform.localScale = new Vector3(width * scalingFactor, height * scalingFactor,1f);
    }
}
