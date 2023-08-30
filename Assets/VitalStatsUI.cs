using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class VitalStatsUI : MonoBehaviour
{

    
    public RectTransform healthBarFillRect;

    public RectTransform hungerBarFillRect;

    public RectTransform coldBarFillRect;

    public RectTransform energyBarFillRect;


    private const float maxHealthScale = 3.75f;
    private const float maxSquareScale = 1.25f;
    private const int maxStat = 255;

    public void UpdateVitalStats(VitalStats vitalStats)
    {
        //Debug.Log("" + vitalStats);
        float newScaleHealth = (float)vitalStats.health / maxStat * maxHealthScale;
        healthBarFillRect.localScale = new Vector3(newScaleHealth, healthBarFillRect.localScale.y, healthBarFillRect.localScale.z);

        float newScaleHunger = (float)vitalStats.hunger / maxStat * maxSquareScale;
        hungerBarFillRect.localScale = new Vector3(hungerBarFillRect.localScale.x,newScaleHunger, hungerBarFillRect.localScale.z);

        float newScaleCold = (float)vitalStats.cold / maxStat * maxSquareScale;
        coldBarFillRect.localScale = new Vector3(coldBarFillRect.localScale.x, newScaleCold,coldBarFillRect.localScale.z);

        float newScaleEnergy = (float)vitalStats.energy / maxStat * maxSquareScale;
        energyBarFillRect.localScale = new Vector3(energyBarFillRect.localScale.x, newScaleEnergy,energyBarFillRect.localScale.z);
    }


}
