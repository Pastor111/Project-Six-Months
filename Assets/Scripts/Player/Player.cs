using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int Life = 100;
    public int MaxLife = 100;
    public float InteractRadius;
    public LayerMask InteractLayer;
    [Space]
    [Space]
    [Space]
    [Header("UI")]
    public float HealthDelayUpdate = 1;
    public Slider HealthBar;
    public float HealthBar1Speed;
    public Slider HealthBar2;
    public float HealthBar2Speed;
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
    [Space]
    public GameObject ShopUI;
    public GameObject StorePanel;
    //public TMPro.TextMeshProUGUI D_CharacterName;
    //public TMPro.TextMeshProUGUI D_Dialogue;
    [Space]
    public GameObject GamePlayUI;
    public GameObject GameOverUI;
    [Space]
    public GameObject Notification;
    [Space]
    [Space]
    public TMPro.TextMeshProUGUI GoldText;
    public TMPro.TextMeshProUGUI KeyText;
    [Space]
    [Space]
    [Space]
    [Header("REFENCES")]
    public PlayerMovement movement;
    public CameraController cam;
    public Gun gun;
    public static Player instance;
    [Space]
    [Space]
    [Space]
    [Header("Inventory")]
    public int Gold;
    public float GoldSpeed;
    public int Keys;
    private int m_gold;
    [Space]
    [Space]
    [Space]
    [Header("Game Feel")]
    public AudioClip GainGold;
    public AudioClip BuyItem;

    Interactable intRequest;
    DialogueManager dialogue;
    ShopNPC shop;


    // Start is called before the first frame update
    void Start()
    {
        HealthBar.maxValue = MaxLife;
        HealthBar.minValue = 0;
        HealthBar2.maxValue = MaxLife;
        HealthBar2.minValue = 0;
        SetLifeDirect(Life);
        instance = this;
        GamePlayUI.SetActive(true);
        GameOverUI.SetActive(false);
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //HealthBar.value = Life;
        HealthText.text = $"{Life} | {100}";

        Gold = (int)Mathf.MoveTowards(Gold, m_gold, GoldSpeed * Time.deltaTime);

        GoldText.text = Gold.ToString();
        KeyText.text = Keys.ToString();

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


        if (shop != null)
        {
            ShopUI.SetActive(true);
            GamePlayUI.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            //D_CharacterName.text = dialogue.GetCurrentDialogue().Character;
            //D_Dialogue.text = dialogue.GetCurrentDialogue().Text;
        }
        else
        {
            ShopUI.SetActive(false);
            GamePlayUI.SetActive(true);
            //movement.Lock = false;
            //cam.Lock = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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

        HealthBar2.targetGraphic.color = Color.white;
        StartCoroutine(WaitHealthBar2(HealthDelayUpdate));


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

    public void AddLife(int l = 10)
    {
        Life += l;

        HealthBar2.targetGraphic.color = Color.white;
        StartCoroutine(WaitHealthBar1(HealthDelayUpdate));

        if (Life >= MaxLife)
        {
            Life = MaxLife;
        }
    }

    public void SetLifeDirect(int life)
    {
        Life = life;
        HealthBar.value = life;
        HealthBar2.value = life;
    }

    IEnumerator WaitHealthBar2(float t)
    {
        HealthBar.value = Life;
        yield return new WaitForSeconds(t);

        while(HealthBar2.value != Life)
        {
            HealthBar2.value = Mathf.MoveTowards(HealthBar2.value, Life, HealthBar2Speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator WaitHealthBar1(float t)
    {
        HealthBar2.value = Life;
        yield return new WaitForSeconds(t);

        while (HealthBar.value != Life)
        {
            HealthBar.value = Mathf.MoveTowards(HealthBar.value, Life, HealthBar1Speed * Time.deltaTime);
            yield return null;
        }
    }

    public void SetDialogueInput(DialogueManager manager)
    {
        dialogue = manager;
    }

    public void SetShop(ShopNPC s)
    {
        shop = s;



        if (shop != null)
        {

            movement.Lock = true;
            cam.Lock = true;

            foreach (Transform child in StorePanel.transform.parent)
            {
                if (child.gameObject != StorePanel)
                    Destroy(child.gameObject);
            }

            foreach (SaleItem item in s.SoldItems)
            {
                var itemPanel = Instantiate(StorePanel.gameObject, StorePanel.transform.parent);
                itemPanel.SetActive(true);
                itemPanel.GetComponent<Button>().onClick.AddListener(() => { shop.Buy(item); });
                itemPanel.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = item.Name;
                itemPanel.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = item.Price.ToString();

                if (item.SoldOut)
                    itemPanel.GetComponent<Button>().interactable = false;
            }

            ShopUI.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(shop.CloseStore);
            EventSystem.current.firstSelectedGameObject = StorePanel.transform.parent.GetChild(1).gameObject;

        }
        else
        {
            movement.Lock = false;
            cam.Lock = false;
            ShopUI.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    IEnumerator CloseStore(float t, ShopNPC shop)
    {
        yield return new WaitForSeconds(t);
        shop.CloseStore();
    }

    IEnumerator ShowHit(float t)
    {
        yield return new WaitForSeconds(t);
        Time.timeScale = 1;
        HitScreen.SetActive(false);

    }

    public void ShowNotification(float time, string text)
    {
        Notification.SetActive(true);
        Notification.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
        StartCoroutine(ShowNotif(time));
    }

    IEnumerator ShowNotif(float time)
    {
        yield return new WaitForSeconds(time);
        Notification.SetActive(false);
    }

    public void SetInteractRequest(Interactable interactable)
    {
        intRequest = interactable;
    }

    public void SetGold(int newGold)
    {

        if (newGold >= m_gold)
            AudioManager.PlaySound2D(GainGold);
        else
            AudioManager.PlaySound2D(BuyItem);

        m_gold = newGold;
    }

    public int GetGold() { return m_gold; }

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
