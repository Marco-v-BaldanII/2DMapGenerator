using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlgorithmManager : MonoBehaviour
{
   public GameObject[] myAlgorithms;

    public int AlgorithmIndex = -1;

    public void SetWalker()
    {
        AlgorithmIndex = 0;
    }
    public void SetPrim()
    {
        AlgorithmIndex = 1;
    }

    public void SetWave()
    {
        AlgorithmIndex = 2;
    }

    public void SetSudoku()
    {
        AlgorithmIndex = 3;
    }

    public void SetPerlin()
    {
        AlgorithmIndex = 4;
    }

    public void SetDiamond()
    {
        AlgorithmIndex = 5;
    }

    public void ReDo()
    {
        if (AlgorithmIndex != 1)
        {
            myAlgorithms[AlgorithmIndex].gameObject.SetActive(false);
            StartCoroutine("waitABit");
        }
    }


    private IEnumerator waitABit()
    {
        yield return new WaitForSeconds(0.5f);

        myAlgorithms[AlgorithmIndex].gameObject.SetActive(true);
    }

    public void ReLoad()
    {
        SceneManager.LoadScene("Nothing");


    }

}
