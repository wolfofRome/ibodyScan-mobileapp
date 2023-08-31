using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amatib
{
    public class SkeletonSlider : MonoBehaviour
    {
        public GameObject model, colorModel, skeleton;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void Skeleton()
        {
            float value = GameObject.Find("SkeletonSlider").GetComponent<Slider>().value;
            Color color = model.GetComponent<Renderer>().material.color;
            color.a = 1 - value;
            model.GetComponent<Renderer>().material.color = color;

            colorModel.GetComponent<Renderer>().material.color = color;

            for (int i = 0; i < skeleton.GetComponent<Renderer>().materials.Length; i++)
            {
                Color scolor = skeleton.GetComponent<Renderer>().materials[i].color;
                scolor.a = value;
                skeleton.GetComponent<Renderer>().materials[i].color = scolor;
            }
        }
    }
}
