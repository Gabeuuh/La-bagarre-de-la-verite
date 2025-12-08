using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healtbar : MonoBehaviour
{
    public Slider slider ;

    public void SetMaxHealt(int health){
        slider.maxValue = health;
        slider.value = 0;
    }


    public void SetHealt(int health){
        slider.value = health;
    }

}
