using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [Header("Audio References")]
    public AudioSource upgrade_audio;
    public AudioSource button_audio;
    [Header("UI References")]
    public Button buy_button;
    public Button[] upgrade_buttons;
    public CanvasGroup roundStart_panel;
    public CanvasGroup transition_image;
    public Color buttonSelectColor;
    public GameObject shop_panel;
    public GameObject win_panel;
    public Image fill_image;
    public Text bit_text;
    public Text height_text;
    public Text selected_text;
    public Text win_text;
    public static UIManager S { get; private set; }
    [Header("Upgrade Amounts")]
    public float rocketForceUpgrade = 10;
    public float thrustTimeUpgrade = 0.5f;
    public float gaugeSpeedUpgrade = 0.1f;
    private RocketControl rocketControl;
    private readonly float[] fillAmounts = new float[10] { 0, 0.11f, 0.23f, 0.34f, 0.45f, 0.57f, 0.68f, 0.8f, 0.91f, 1.0f };
    private readonly int[] upgrade_costs = new int[9] { 22, 69, 100, 222, 420, 655, 910, 1680, 2222};
    private string upgrade1_des = "Repair your Rocket's engines so it has more launch power! Cost: ";
    private string upgrade2_des = "Repair your Rocket's fuel tank so it can launch longer! Cost: ";
    private string upgrade3_des = "Repair your Rocket's launch gauge so it's bar moves slower! Cost: ";
    private int selectedButton;
    private int upgrade1_amount = 0;
    private int upgrade2_amount = 0;
    private int upgrade3_amount = 0;

    private void Awake()
    {
        if (S != null && S != this)
        {
            // Destroy if another Gamemanager already exists
            Destroy(gameObject);
        }
        else {

            // Here we save our singleton S
            S = this;
            // Furthermore we make sure that we don't destroy between scenes
            DontDestroyOnLoad(gameObject);
        }
        transition_image.alpha = 1;
    }

    private void Start()
    {
        rocketControl = GameObject.Find("Rocket").GetComponent<RocketControl>();
        roundStart_panel.alpha = 1;
        StartCoroutine(RoundStart());
    }

    private void Update()
    {
        if (rocketControl.launchRocket_initial)
        {
            height_text.text = rocketControl.maxHeightAchieved.ToString() + "m";
        }
        bit_text.text = "x " + rocketControl.bitsCollected.ToString();
    }

    public IEnumerator RoundStart()
    {
        while(transition_image.alpha > 0)
        {
            transition_image.alpha -= Time.deltaTime * 2;
            yield return null;
        }
        yield return null;
    }

    public IEnumerator RoundEnd()
    {
        while (transition_image.alpha < 1)
        {
            transition_image.alpha += Time.deltaTime * 2;
            yield return null;
        }
    }

    public IEnumerator RocketLaunch()
    {
        roundStart_panel.interactable = false;
        while (roundStart_panel.alpha > 0)
        {
            roundStart_panel.alpha -= Time.deltaTime * 0.75f;
            yield return null;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void DisplayWinScreen()
    {
        win_panel.SetActive(true);
        win_text.text = "Way to go! You got the Rocket back to the Mothership! Let's hope next time it's exploring that it will be more careful!\n\nOver the course of your adventure, you traveled a total of " + rocketControl.totalHeightAchieved.ToString() + " meters and collected a total of " + rocketControl.totalBitsCollected.ToString() +  " Star Bits!\n\nThanks for playing; I hope you enjoyed Rocket Repair!";
    }

    public void UpgradeButton(int i)
    {
        selectedButton = i;
        foreach (Button button in upgrade_buttons)
        {
            button.image.color = Color.white;
        }
        upgrade_buttons[i].image.color = buttonSelectColor;
        switch (i)
        {
            case 0:
                if (upgrade1_amount <= upgrade_costs.Length - 1) selected_text.text = upgrade1_des + upgrade_costs[upgrade1_amount].ToString() + " Star Bits";
                else selected_text.text = upgrade1_des + "FULLY REPAIRED!";
                fill_image.fillAmount = fillAmounts[upgrade1_amount];
                break;
            case 1:
                if (upgrade2_amount <= upgrade_costs.Length - 1) selected_text.text = upgrade2_des + upgrade_costs[upgrade2_amount].ToString() + " Star Bits";
                else selected_text.text = upgrade2_des + "FULLY REPAIRED!";
                fill_image.fillAmount = fillAmounts[upgrade2_amount];
                break;
            case 2:
                if (upgrade3_amount <= upgrade_costs.Length - 1) selected_text.text = upgrade3_des + upgrade_costs[upgrade3_amount].ToString() + " Star Bits";
                else selected_text.text = upgrade3_des + "FULLY REPAIRED!";
                fill_image.fillAmount = fillAmounts[upgrade3_amount];
                break;
        }
    }

    public void BuyButton()
    {
        switch (selectedButton)
        {
            case 0:
                if(upgrade1_amount > upgrade_costs.Length - 1 || rocketControl.bitsCollected < upgrade_costs[upgrade1_amount])
                {
                    return;
                }
                rocketControl.bitsCollected -= upgrade_costs[upgrade1_amount];
                upgrade1_amount++;
                rocketControl.rocketForce += rocketForceUpgrade;
                break;
            case 1:
                if (upgrade2_amount > upgrade_costs.Length - 1 || rocketControl.bitsCollected < upgrade_costs[upgrade2_amount])
                {
                    return;
                }
                rocketControl.bitsCollected -= upgrade_costs[upgrade2_amount];
                upgrade2_amount++;
                rocketControl.thrusterTime += thrustTimeUpgrade;
                break;
            case 2:
                if (upgrade3_amount > upgrade_costs.Length - 1 || rocketControl.bitsCollected < upgrade_costs[upgrade3_amount])
                {
                    return;
                }
                rocketControl.bitsCollected -= upgrade_costs[upgrade3_amount];
                upgrade3_amount++;
                rocketControl.gaugeSpeed -= gaugeSpeedUpgrade;
                break;
        }
        upgrade_audio.Play();
        UpgradeButton(selectedButton);
    }

    public void ShopButton()
    {
        button_audio.Play();
        height_text.text = "Repair Menu";
        shop_panel.SetActive(true);
        UpgradeButton(0);
    }

    public void CloseButton()
    {
        button_audio.Play();
        height_text.text = "0m";
        shop_panel.SetActive(false);
    }
}
