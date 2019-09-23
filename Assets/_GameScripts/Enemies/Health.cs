using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.Profiling;

public class Health : MonoBehaviour
{
    public float currentHP;
    public float maxHP;
    public bool invincible;
    GameObject HealthCanvas;
    public Image HealthBar;
    public Image HealthBarBack;
    public bool hideOnStay = true;
    public bool dead;
    public bool requireHost;
    public bool NetworkManaged;
    LookAt hlook;
    [System.Serializable]
    public class GOEvent : UnityEvent<GameObject> { }
    public GOEvent OnDeath;
    public GOEvent OnAttacked;
    public GOEvent OnAttacker;
    bool attacked;
    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }
    public FloatEvent TakeDamage;
    Creature c;

    PhotonView v;

    void Awake()
    {
        c = GetComponent<Creature>();
        if (HealthBar != null)
            HealthCanvas = HealthBar.GetComponentInParent<Canvas>().gameObject;
        if(PhotonNetwork.inRoom)
            PhotonNetwork.SendMonoMessageTargets.Add(gameObject);
    }

    void Start()
    {
        if (HealthBar != null)
            hlook = HealthBar.GetComponentInParent<LookAt>();
        if (hideOnStay && HealthBar != null)
        {
            HealthCanvas.SetActive(false);
            HealthBar.color = new Vector4(256, 0, 0, 0);
            HealthBarBack.color = new Vector4(256, 256, 256, 0);
            if (hlook != null)
                hlook.visible = false;
        }
        else if (HealthBar != null)
        {
            HealthBar.color = new Vector4(256, 0, 0, .7f);
            HealthBarBack.color = new Vector4(256, 256, 256, .7f);
        }
        v = GetComponent<PhotonView>();
    }

    public void SetPhotonView(PhotonView view)
    {
        v = view;
    }

    public void NetworkSetHealth()
    {
        if (v == null)
            v = GetComponent<PhotonView>();
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient && v != null)
        {
            v.RPC("SetHP", PhotonTargets.OthersBuffered, currentHP, maxHP);
        }
    }

    [PunRPC]
    void SetHP(float chp, float mhp)
    {
        currentHP = chp;
        maxHP = mhp;
    }

    bool doubleChecked;
    void FixedUpdate()
    {
        if (currentHP <= 0 && !dead)
        {
            if(!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient || (!requireHost || NetworkManaged))
                Die(requireHost || NetworkManaged);
        }
        if(currentHP > 0 && dead && !doubleChecked)
        {
            doubleChecked = true;
            Invoke("DoubleCheckDead", 2.5f);
        }

    }

    void DoubleCheckDead()
    {
        if (currentHP > 0 && dead)
            dead = false;
        doubleChecked = false;
    }

    [AdvancedInspector.Inspect]
    public void ForceDieAll()
    {
        Die(true);
    }

    void Die(bool sendNetwork=true)
    {
        Profiler.BeginSample("Health::Die update network");
        if (PhotonNetwork.inRoom && GetComponent<PhotonView>() != null && sendNetwork && !NetworkManaged)
            GetComponent<PhotonView>().RPC("DieNetwork", PhotonTargets.Others);
        Profiler.EndSample();

        Profiler.BeginSample("Health::Die update ui");
        dead = true;
        if (HealthBar != null && hideOnStay)
        {
            HealthCanvas.SetActive(false);
            if(hlook != null)
                hlook.visible = false;
            HealthBar.color = new Vector4(256, 0, 0, 0);
            HealthBarBack.color = new Vector4(256, 256, 256, 0);
        }
        Profiler.EndSample();

        Profiler.BeginSample("Health::Die OnDeath events invoke");
        
        OnDeath.Invoke(gameObject);
        Profiler.EndSample();
    }

    [PunRPC]
    void DieNetwork()
    {
        if(!dead)
            Die(false);
    }

    public void Revive(bool fullHeal)
    {
        dead = false;
        if (fullHeal)
            currentHP = maxHP;
        else
            currentHP = maxHP / 2;
        if (!hideOnStay && hlook != null)
            hlook.visible = true;
        updateHealthBar();
    }

    public bool isDead()
    {
        return currentHP <= 0;
    }

    public void Kill()
    {
        takeDamageImmediate(currentHP*2);
    }

    public void SetAttacker(GameObject attacker)
    {
        OnAttacker.Invoke(attacker);
    }

    [AdvancedInspector.Inspect]
    public void TakeDamageHundred()
    {
        takeDamageImmediate(100);
    }

    public void takeArrowDamage(float dmg, ArrowCollision e)
    {
        if (c != null)
            c.SetLastHit(e);
        takeDamage(dmg);
    }

    public void takeDamage(float dmg, bool showText=true)
    {
        if (requireHost)
        {
            if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient)
            {
                return;
            }
        }
        if (invincible)
            return;
        if (!dead && dmg != 0)
        {
            if(!attacked && dmg > 0)
            {
                attacked = true;
                OnAttacked.Invoke(gameObject);
            }
            if (showText)
            {
                if(c != null)
                    CombatText.ShowText("" + (int)dmg, c.Healthbar.position, Color.red);
                else
                    CombatText.ShowText("" + (int)dmg, transform.position + Vector3.up, Color.red);
            }
                
            TakeDamage.Invoke(dmg);
            currentHP -= dmg;
            if (currentHP < 0)
                currentHP = 0;
            if (v != null && PhotonNetwork.inRoom)
                v.RPC("NetworkDamage", PhotonTargets.OthersBuffered, dmg, currentHP);
            if (currentHP > maxHP)
                currentHP = maxHP;
            updateHealthBar();
        }
    }

    public void takeDamageImmediate(float dmg)
    {
        currentHP -= dmg;
        if (currentHP < 0)
            currentHP = 0;
        if (currentHP > maxHP)
            currentHP = maxHP;
        if (v != null && PhotonNetwork.inRoom)
            v.RPC("NetworkDamage", PhotonTargets.Others, dmg, -1.0f);
        updateHealthBar();
    }

    public void SetInvincible(float duration=5f)
    {
        invincible = true;
        if (duration > 0)
            Invoke("SetVulnerable", duration);
    }

    public void SetVulnerable()
    {
        invincible = false;
    }

    public void IncreaseHealth(float val)
    {
        if(!isDead())
        {
            maxHP += val;
            currentHP += val;
            if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient && GetComponent<PhotonView>() != null && !NetworkManaged)
                GetComponent<PhotonView>().RPC("SetHealthNetwork", PhotonTargets.Others, currentHP, maxHP);
        }
    }

    [PunRPC]
    void SetHealthNetwork(float cur, float max)
    {
        currentHP = cur;
        maxHP = max;
    }

    [PunRPC]
    void NetworkDamage(float dmg, float chp=-1)
    {
        if (chp < 0)
            chp = currentHP;
        if (!dead)
        {
            currentHP -= dmg;
            if (currentHP != chp && currentHP > 0 && chp >= 0)
                currentHP = chp;
            updateHealthBar();
            if (dmg > 0)
                TakeDamage.Invoke(dmg);
        }
    }

    public void updateHealthBar()
    {
        if (!gameObject.activeInHierarchy || !Settings.GetBool("EnmHealthbars"))
            return;
        if (HealthBar != null)
            HealthBar.fillAmount = currentHP / maxHP;
        else
            return;
        HealthCanvas.SetActive(true);
        if (hideOnStay && currentHP > 0)
        {
            if (hlook != null)
                hlook.visible = true;
            StopCoroutine("HideBar");
            HealthBar.color = new Vector4(256, 0, 0, 1);
            HealthBarBack.color = new Vector4(256, 256, 256, 1);
            if (!dead)
                StartCoroutine("HideBar");
        }
        else
        {
            HealthBar.color = new Vector4(256, 0, 0, .7f);
            HealthBarBack.color = new Vector4(256, 256, 256, 0.7f);
        }
        if (currentHP <= 0)
        {
            HealthCanvas.SetActive(false);
            /*
            if (hlook != null && hideOnStay)
                hlook.visible = false;
            HealthBar.color = new Vector4(256, 0, 0, 0);
            HealthBarBack.color = new Vector4(256, 256, 256, 0);
            */
        }
    }

    void OnJoinedRoom()
    {
        //currentHP = maxHP;
    }

    void OnPhotonPlayerConnected(PhotonPlayer plr)
    {
        if(requireHost && PhotonNetwork.isMasterClient && !NetworkManaged && v != null)
        {
            v.RPC("DieNetwork", plr);
        }
    }

    IEnumerator HideBar()
    {
        float i = 1;
        while (i > 0)
        {
            if (dead)
                i = 0;
            i -= Time.deltaTime * 0.5f;
            HealthBar.color = new Vector4(256, 0, 0, i);
            HealthBarBack.color = new Vector4(256, 256, 256, i);
            yield return true;
        }
        HealthCanvas.SetActive(false);
        HealthBar.color = new Vector4(256, 0, 0, 0);
        HealthBarBack.color = new Vector4(256, 256, 256, 0);
        if (hlook != null)
            hlook.visible = false;
        StopCoroutine("HideBar");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(requireHost && !NetworkManaged)
        {
            if (stream.isWriting)
            {
                stream.SendNext(currentHP);
            }
            else
            {
                float c = (float)stream.ReceiveNext();
                if (c != currentHP)
                {
                    currentHP = c;
                    updateHealthBar();
                }
            }
        }
    }
}
