using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int Life = 100;
    public float InteractRadius;
    public LayerMask InteractLayer;
    [Space]
    [Space]
    [Space]
    [Header("UI")]
    public Slider HealthBar;
    public TMPro.TextMeshProUGUI HealthText;
    public GameObject HitScreen;
    [Space]
    [Space]
    public GameObject PressToInteract;
    [Space]
    [Space]
    public GameObject DialogueUI;
    public TMPro.TextMeshProUGUI D_CharacterName;
    public TMPro.TextMeshProUGUI D_Dialogue;
    [Space]
    public GameObject GamePlayUI;
    public GameObject GameOverUI;
    [Space]
    [Space]
    [Space]
    [Header("REFENCES")]
    public PlayerMovement movement;
    public CameraController cam;
    public Gun gun;
    public static Player instance;

    Interactable intRequest;
    DialogueManager dialogue;


    // Start is called before the first frame update
    void Start()
    {
        HealthBar.maxValue = Life;
        HealthBar.minValue = 0;
        instance = this;
        GamePlayUI.SetActive(true);
        GameOverUI.SetActive(false);
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.value = Life;
        HealthText.text = $"{Life} | {100}";

        if(Life <= 0)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                LevelGenerator.generator.ReloadLevel();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        if(intRequest != null)
        {
            PressToInteract.SetActive(true);

            if (Input.GetKeyDown(KeyCode.I))
            {
                intRequest.Interact();
            }
        }
        else
        {
            PressToInteract.SetActive(false);
        }

        if(dialogue != null)
        {
            DialogueUI.SetActive(true);
            GamePlayUI.SetActive(false);
            D_CharacterName.text = dialogue.GetCurrentDialogue().Character;
            D_Dialogue.text = dialogue.GetCurrentDialogue().Text;
        }
        else
        {
            DialogueUI.SetActive(false);
            GamePlayUI.SetActive(true);
        }

        var col = Physics.OverlapSphere(transform.position, InteractRadius, InteractLayer);

        if(col.Length > 0)
        {
            SetInteractRequest(col[0].GetComponent<Interactable>());
        }
        else
        {
            SetInteractRequest(null);
        }

    }

    public void GetDamage(int damage = 10)
    {
        Life -= damage;

        if (Life <= 0)
        {
            Life = 0;
            Die();
        }
        else
        {
            //Time.timeScale = 0;
            HitScreen.SetActive(true);
            StartCoroutine(ShowHit(0.1f));
        }
    }

    public void SetDialogueInput(DialogueManager manager)
    {
        dialogue = manager;
    }

    IEnumerator ShowHit(float t)
    {
        yield return new WaitForSeconds(t);
        Time.timeScale = 1;
        HitScreen.SetActive(false);

    }

    public void SetInteractRequest(Interactable interactable)
    {
        intRequest = interactable;
    }

    public void Die()
    {
        GamePlayUI.SetActive(false);
        GameOverUI.SetActive(true);
        Time.timeScale = 0.0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, InteractRadius);
    }
}
