using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SocialIcons : MonoBehaviour {

    PhotonView view;

    public EMOpenCloseMotion motion;
    public Image img;
    public Sprite[] icons;

    int currentIcon;

    void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    public void SendSocialIcon(int iconID)
    {
        if (PhotonNetwork.inRoom && view != null)
            view.RPC("IconNetwork", PhotonTargets.Others, iconID);
    }

    [AdvancedInspector.Inspect]
    public void TestIcon()
    {
        IconNetwork(0);
    }

    [PunRPC]
    void IconNetwork(int iconID)
    {
        if(iconID >= 0 && iconID < icons.Length)
        {
            CancelInvoke();
            motion.SetStateToClose();
            img.sprite = icons[iconID];
            motion.Open();
            Invoke("CloseMotion", 5f);
        }
    }

    void CloseMotion()
    {
        motion.Close();
    }

}
