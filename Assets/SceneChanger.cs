using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;
   public void StartGame()
   {
        PlayerPrefs.SetString("GameMode", dropdown.captionText.text);
        SceneManager.LoadScene("Main");
   }
}
