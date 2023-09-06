using System.Collections;
using System.Collections.Generic;
using FitAndShape;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingError : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToLoginScene()
    {
        SceneManager.LoadScene(LoginPresenter.SceneName);
    }
}
