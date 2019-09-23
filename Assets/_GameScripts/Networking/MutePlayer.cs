using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MutePlayer : MonoBehaviour {

    public string PlayerName;

    public Sprite MuteIcon;
    public Sprite BaseIcon;

    public Color MuteColor;
    public Color BaseColor;
    public Color MyColor;

    PlayerSync ps;

    Image Img;

    bool mine;

    public void Init(string name)
    {
        PlayerName = name;
        Img = GetComponent<Image>();
        if (PlayerName != PhotonNetwork.player.name)
            GetComponent<Collider>().enabled = true;
        GetPlayer();
        UpdateGraphic();
    }

    public void GetPlayer()
    {
        foreach (var v in FindObjectsOfType<PlayerSync>())
        {
            PhotonView ph = v.GetComponent<PhotonView>();
            if(ph == null || ph.owner == null)
            {
                Debug.Log("Bad/No PhotonView");
            }
            else if (ph.owner.name == PlayerName)
            {
                ps = v;
                if (v.GetComponent<PhotonView>().isMine)
                    mine = true;
                return;
            }
        }
    }

	public void ToggleMute()
    {
        if (ps == null)
            GetPlayer();
        if(ps != null)
        {
            ps.ToggleMute();
            UpdateGraphic();
        }
    }

    public void UpdateGraphic()
    {
        if (ps == null)
            return;
        if (ps.muted || ps.selfMute)
            Img.sprite = MuteIcon;
        else
            Img.sprite = BaseIcon;

        if (ps.muted)
            Img.color = MuteColor;
        else
            Img.color = BaseColor;

        if (mine)
            Img.color = MyColor;
    }
}
