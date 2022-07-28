#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyberultimate.Unity
{
    public class SliderWithCounter : MonoBehaviour
    {
        [Header("Remember to set On Value Changed to OnDrag method.")]

        protected Slider slider = null!;
        [SerializeField] private Text textCounter = null!;

        public Text TextCounter => textCounter;

        protected void Awake()
        {
            slider = GetComponent<Slider>();
        }

        public virtual void OnDrag()
        {
            textCounter.text = slider.value.ToString();
        }

        public virtual void OnDragWithMax()
        {
            textCounter.text = $"{slider.value.ToString()}/{slider.maxValue.ToString()}";
        }
    }
}

