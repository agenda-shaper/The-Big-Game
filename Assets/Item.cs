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
    public PutableImg putableimg;
    public NonPutableImg notputableimg;
    public bool isDoor;
    public string ground_img;

    public Broken broken;
    public Instiation instiation;


    // Start is called before the first frame update
    void Start()
    {
        
    }

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
}

[System.Serializable]
public class Broken
{
    public string source;
}

[System.Serializable]
public class Instiation
{
    public string source;
}