using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Block : NetworkBehaviour
{
    public MeshRenderer meshRenderer;

    public GameObject meshObject;

    public MeshFilter meshFilter;

    [SyncVar]
    public GameObject blockInstance;

    [SyncVar]
    public Item item;

    [SyncVar]
    public int rotation;

    [SyncVar]
    public int health;

    private Coroutine currentMoveCoroutine; // Store the current running coroutine

    public bool animating;


    public void loadImageTexture(Texture2D tex)
    {
        //image.texture = tex;
        meshRenderer.material.mainTexture = tex;
        meshRenderer.material.shader = Shader.Find("Standard");
        meshRenderer.enabled = true;
        //image.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
    }
    public void AnimateHit(float angle)
    {
        // Stop any existing coroutine to reset the animation
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            animating = false;
            Engine.Instance.blockManager.UpdateBlock(this);
            meshObject.transform.localPosition = new Vector3(0, 0, 0); // Reset mesh position
        }

        Vector3 direction = Quaternion.Euler(0, -(angle-90), 0) * Vector3.forward;
        currentMoveCoroutine = StartCoroutine(MoveBlock(direction, 0.1f, 0.15f));
    }

    private IEnumerator MoveBlock(Vector3 direction, float duration, float distance)
    {
        animating = true;
        Engine.Instance.blockManager.UpdateBlock(this);
        Vector3 startPos = meshObject.transform.localPosition;
        Vector3 endPos = startPos + direction.normalized * distance;

        float time = 0;
        while (time <= duration)
        {
            float t = time / duration;
            meshObject.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        // Move back to original position
        time = 0;
        while (time <= duration)
        {
            float t = time / duration;
            meshObject.transform.localPosition = Vector3.Lerp(endPos, startPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        meshObject.transform.localPosition = startPos; // Make sure it's back to the original position
        animating = false;
        Engine.Instance.blockManager.UpdateBlock(this);
    }


}
