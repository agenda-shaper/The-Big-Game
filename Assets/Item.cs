using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class Item
{

    public int id;
    public ImgData img;
    public DetailData detail;
    public int stack;
    public int[] width;
    public int[] height;

    public int[] inmapx;
    public int[] inmapy;
    public PutableImg putableimg;
    public NonPutableImg notputableimg;
    public bool isDoor;
    public Broken[] broken; 
    public Instation[] instation; 
    public string ground_img;
    public BlockMeshes blockMeshes;
    public int health;
    public int selection_type;

    public string texture;

    public string connection_type;

    public bool hittable_by_projectiles;

    public bool collidesWithEntities;

    public string inmap_image;

    // Start is called before the first frame update
    void Start()
    {
        
    }

}



[System.Serializable]
public class BlockMeshes
{
    public string inward;
    public string outward;
    public string side;
    public string enclosed;
    public string building;
}

[System.Serializable]
public class PutableImg
{
    public string source;
}

[System.Serializable]
public class NonPutableImg
{
    public string source;
}

[System.Serializable]
public class ImgData
{
    public string[] source;
}

[System.Serializable]
public class DetailData
{
    public string name;
    public string description;
    public int level;
    public int skillCost;
    public int type;
    public RequiredResources[] requiredResources;
}
[System.Serializable]
public class RequiredResources
{
    public int itemId;
    public int count;
}

[System.Serializable]
public class Broken
{
    public string source; // Changed to a string
}

[System.Serializable]
public class Instation
{
    public string source; // Changed to a string
}