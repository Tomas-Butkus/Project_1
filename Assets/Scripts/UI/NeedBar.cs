using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetFill(int amount)
    {
        slider.value = amount;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
