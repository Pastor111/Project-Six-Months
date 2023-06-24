using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int CurrentDificulty;
    public string[] Dificulties;
    public TMP_Dropdown Dif_DropDown;

    public GameObject LoadingScreen;


    // Start is called before the first frame update
    void Start()
    {
        Dif_DropDown.ClearOptions();
        for (int i = 0; i < Dificulties.Length; i++)
        {
            Dif_DropDown.options.Add(new TMP_Dropdown.OptionData(Dificulties[i]));
        }

        Dif_DropDown.value = 0;

        Dif_DropDown.onValueChanged.AddListener((i) => { FindObjectOfType<GameSettings>().difficulty = (GameSettings.GameDifficulty)i; });
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LoadScene(int index)
    {
        StartCoroutine(Scene_Load(index));
    }

    public IEnumerator Scene_Load(int sceneIndex)
    {
        var ao = SceneManager.LoadSceneAsync(sceneIndex);
        yield return new WaitForSeconds(1f);

        while (!ao.isDone)
        {
            Debug.Log(ao.progress);
            yield return null;
        }
        
    }
}
