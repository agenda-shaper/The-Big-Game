using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Block : NetworkBehaviour
{
    public MeshRenderer meshRenderer;

    [SyncVar]
    public GameObject blockInstance;

    [SyncVar]
    public Item item;

    [SyncVar]
    public int rotation;

    [SyncVar]
    public int health;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadImageTexture(Texture2D tex)
    {
        //image.texture = tex;
        meshRenderer.material.mainTexture = tex;
        meshRenderer.material.shader = Shader.Find("Unlit/Transparent");
        meshRenderer.enabled = true;
        //image.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
    }

}
