using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureSpawnButton : MonoBehaviour {

    public int EnemyID;
    public int Elite;
    public Text CostText;
    public Image CooldownDisplay;
    public int Cost;
    public float Cooldown = 1f;
    float cd = 0;
    public bool CanClick;

    AnimateScale scaleAnim;

    void Awake()
    {
        scaleAnim = GetComponent<AnimateScale>();
    }

    void Start()
    {
        if(CostText != null)
            CostText.text = Cost + "";
        Button b = GetComponent<Button>();
        if(b != null)
            b.onClick.AddListener(Click);
    }

    void Update()
    {
        if (cd > 0)
            cd -= Time.deltaTime;
        if(CooldownDisplay != null)
            CooldownDisplay.fillAmount = cd / Cooldown;
    }

    public void Click()
    {
        if (pvpmanager.instance.PlayingPVP && PvPPanel.instance.isOpen())
        {
            if (Cost <= pvpmanager.instance.myResource && pvpmanager.instance.AllowedEnm() > 0 && cd <= 0)
            {
                Debug.Log("Enemy Button Clicked");
                cd = Cooldown;
                pvpmanager.instance.myResource -= Cost;
                pvpmanager.instance.SpawnEnemy(EnemyID, Elite);
                if (scaleAnim != null)
                    scaleAnim.Play();
                PvPPanel.instance.PlaySpawn();
            }
        }
    }

    public void UpdateButtonVals()
    {
        string enemName = EnemyDB.v.Enemies[EnemyID].Name.ToUpper().Replace(" ", "").Trim();
        if (Elite > 0)
            enemName += "_ELITE_";
#if !NOANALYTICS
        Cost = RemoteSettings.GetInt(enemName + "_COST", Cost);
        Cooldown = RemoteSettings.GetFloat(enemName + "_CD", Cooldown);
#endif
        if (CostText != null)
            CostText.text = Cost + "";
    }
}
