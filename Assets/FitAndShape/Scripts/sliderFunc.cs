using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class sliderFunc : MonoBehaviour
{
    bool rotating = false;
    float minFov = 60f;
    float maxFov = 85f;
    public GameObject Mesh;
    public GameObject ChestBone, WaistBone;
    public GameObject[] hipsBone = new GameObject[4];
    public GameObject[] armsBone = new GameObject[2];
    public GameObject[] thighsBone = new GameObject[2];
    public GameObject[] calvesBone = new GameObject[2];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePosition.y > Screen.height * 0.32f && Input.GetMouseButtonDown(0))
        {
            rotating = true;
        }
        if(Input.GetMouseButtonUp(0))
            rotating = false;
        if(rotating)
        {
            float amount = Input.GetAxis("Mouse X");
            float amountY = Input.GetAxis("Mouse Y");
            if(Mathf.Abs(amount) > Mathf.Abs(amountY))
                Mesh.transform.Rotate(0f, -Time.deltaTime*600f*amount, 0f);
            //else
            //    Mesh.transform.Rotate(-Time.deltaTime*600f*amountY, 0f, 0f);
        }

        float fov = Camera.main.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * 10f;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;
    }

    public void Chest()
    {
        float value = GameObject.Find("Slider1").GetComponent<Slider>().value;
        value += 0.5f;
        ChestBone.transform.localScale = new Vector3(value, value, value);
    }
    public void Waist()
    {
        float value = GameObject.Find("Slider2").GetComponent<Slider>().value;
        value += 0.5f;
        WaistBone.transform.localScale = new Vector3(value, value, value);
    }

    public void Hips()
    {
        float value = GameObject.Find("Slider3").GetComponent<Slider>().value;
        value += 0.5f;
        if(value > 1f)
            value -= (value - 1f)/2f;
        else
            value += (1f - value)/2f;
        hipsBone[0].transform.localScale = new Vector3(value, value, value);
        hipsBone[1].transform.localScale = new Vector3(value, value, value);
        hipsBone[2].transform.localScale = new Vector3(value, value, value);
        hipsBone[3].transform.localScale = new Vector3(value, value, value);
    }
    public void Arms()
    {
        float value = GameObject.Find("Slider4").GetComponent<Slider>().value;
        value += 0.5f;
        if(value > 1f)
            value -= (value - 1f)/2f;
        else
            value += (1f - value)/2f;
        armsBone[0].transform.localScale = new Vector3(value, 1f, value);
        armsBone[1].transform.localScale = new Vector3(value, 1f, value);
    }
    public void Thighs()
    {
        float value = GameObject.Find("Slider5").GetComponent<Slider>().value;
        value += 0.5f;
        if(value > 1f)
            value -= (value - 1f)/2f;
        else
            value += (1f - value)/2f;
        thighsBone[0].transform.localScale = new Vector3(value, 1f, value);
        thighsBone[1].transform.localScale = new Vector3(value, 1f, value);
        calvesBone[0].transform.localScale = new Vector3(1/value, 1f, 1/value);
        calvesBone[1].transform.localScale = new Vector3(1/value, 1f, 1/value);
    }
    public void Calves()
    {
        float value = GameObject.Find("Slider6").GetComponent<Slider>().value;
        value += 0.5f;
        if(value > 1f)
            value -= (value - 1f)/2f;
        else
            value += (1f - value)/2f;
        calvesBone[0].transform.localScale = new Vector3(value, 1f, value);
        calvesBone[1].transform.localScale = new Vector3(value, 1f, value);
    }

}
