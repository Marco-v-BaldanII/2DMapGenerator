using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadAlgorithms : MonoBehaviour
{
    // Start is called before the first frame update



    private void Awake()
    {


        StartCoroutine("LoadCode");




    }


    private IEnumerator LoadCode()
    {
        yield return new WaitForSeconds(0.05f);

        SceneManager.LoadScene("SampleScene");


    }


}
