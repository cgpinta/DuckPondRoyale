using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void ChangeToScene(int nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }
    public void ChangeToScene(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }
    public void ChangeToOnlineLoading()
    {
        Debug.Log("CLICKING METHOD");
        SceneManager.LoadScene("Loading");
    }
}
