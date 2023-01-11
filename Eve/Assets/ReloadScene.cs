using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }



    public void LoadScene()
    {
     //   SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
        SceneManager.LoadScene("Eve");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
