using System;
using System.Collections;
using System.Collections.Generic;
using FitAndShape;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MorphView : MonoBehaviour
{
    [SerializeField] private ModelView _modelView;
    [Header("Sliders")] 
    [SerializeField] private Slider _chestSlider;
    [SerializeField] private Slider _waistSlider;
    [SerializeField] private Slider _hipSlider;
    [SerializeField] private Slider _armSlider;
    [SerializeField] private Slider _legSlider;
    
    
    public IObservable<float> OnMorphChest => _onMorphChest;
    Subject<float> _onMorphChest = new Subject<float>();
    public IObservable<float> OnMorphWaist => _onMorphWaist;
    Subject<float> _onMorphWaist = new Subject<float>();
    public IObservable<float> OnMorphHip => _onMorphHip;
    Subject<float> _onMorphHip = new Subject<float>();
    public IObservable<float> OnMorphArm => _onMorphArm;
    Subject<float> _onMorphArm = new Subject<float>();
    public IObservable<float> OnMorphLeg => _onMorphLeg;
    Subject<float> _onMorphLeg = new Subject<float>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize()
    {
        
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void OnChangeChestSliderValue(float value)
    {
        _onMorphChest.OnNext(value);
    }
    public void OnChangeWaistSliderValue(float value)
    {
        _onMorphWaist.OnNext(value);
    }
    public void OnChangeHipSliderValue(float value)
    {
        _onMorphHip.OnNext(value);
    }
    public void OnChangeArmSliderValue(float value)
    {
        _onMorphArm.OnNext(value);
    }
    public void OnChangeLegSliderValue(float value)
    {
        _onMorphLeg.OnNext(value);
    }
}
