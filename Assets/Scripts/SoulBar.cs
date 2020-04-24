using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulBar : MonoBehaviour
{
    [SerializeField]
    private Slider sliderCurrent;

    [SerializeField]
    private Slider sliderMaximum;

    [SerializeField]
    private int targetMax;  

    private void Start()
    {
        sliderCurrent.maxValue = sliderMaximum.maxValue;
    }

    public void UpdateCurrent(int value)
    {        
        if (value > sliderMaximum.value)
        {
            sliderCurrent.value = sliderMaximum.value;            
        } else
        {
            sliderCurrent.value = value;
        }        
    }    

    public void UpdateMaximumPossible(int maximum)
    {
        sliderMaximum.maxValue = maximum;
        sliderCurrent.maxValue = maximum;
    }

    public void UpdateCurrentMaximum(int maximum, bool animate = true)
    {        
        targetMax = maximum;

        if (sliderMaximum.value == targetMax)
        {
            return;
        }

        if (animate)
        {
            StartCoroutine(AnimateMax());
        }
        else
        {
            sliderMaximum.value = maximum;            
        }

    }

    IEnumerator AnimateMax()
    {
        while (sliderMaximum.value != targetMax)
        {
            int increment = sliderMaximum.value < targetMax ? 1 : -1;
            sliderMaximum.value += increment;            
            yield return new WaitForSeconds(0.3f);
        }        
    }

}
