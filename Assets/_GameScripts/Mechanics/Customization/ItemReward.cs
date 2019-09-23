using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemReward : MonoBehaviour {

    public static ItemReward instance;
    Rigidbody ball;
    public bool active;

    public Text EquipButton;
    public Text IgnoreButton;

    public ItemEvent OnSetup;
    ArmorOption curItem;
    FaceHips track;
    bool setup;
    void Awake()
    {
        instance = this;
        ball = GetComponent<Rigidbody>();
        track = GetComponentInChildren<FaceHips>();
    }

	public void Reset()
    {
        active = false;
        curItem = null;
        setup = false;
        track.active = false;
        GetComponent<ItemOrbDisplay>().DestroyParts();
        transform.position = Vector3.zero;
    }

    public void SetupReward(ArmorOption item)
    {
        curItem = item;
        active = true;
        setup = false;
        track.active = true;
    }

    public void Equip()
    {
        if (curItem != null)
        {
            Armory.instance.EquipItem(curItem);
        }
        Reset(); 
    }

    void FixedUpdate()
    {
        if(!setup && curItem != null)
        {
            OnSetup.Invoke(curItem);
            setup = true;
        }
        if(active && Hips.instance != null)
        {
            Vector3 targetPos = Hips.instance.transform.position + Hips.instance.transform.forward*0.6f;
            Vector3 dir = (targetPos - transform.position).normalized;
            float dist = Vector3.Distance(targetPos, transform.position);
            ball.AddForce(dir*dist*100f * Time.fixedDeltaTime);
            if (dist < 5 && ball.velocity.magnitude > 5)
                ball.velocity *= 0.6f;
            if (ball.angularVelocity.magnitude < 0.75f)
                ball.AddTorque(ball.transform.up * Time.fixedDeltaTime * 20f);
            else
                ball.angularVelocity *= 0.9f;
        }
    }

    void Update()
    {
        if(active)
        {
            if (NVR_Player.isThirdPerson())
            {
                EquipButton.text = "Yes (Y)";
                IgnoreButton.text = "No (N)";
                if (Input.GetKeyDown(KeyCode.Y))
                    Equip();
                else if (Input.GetKeyDown(KeyCode.N))
                    Reset();
            }
        }
    }
}
