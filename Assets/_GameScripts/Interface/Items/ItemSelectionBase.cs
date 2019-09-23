using UnityEngine;
using System.Collections;
using VRTK;

public class ItemSelectionBase : MonoBehaviour {

    public ItemChangeUI[] Selectors;
    public AudioClip[] Clips;
    AudioSource src;

    public GameObject left;
    public GameObject right;

    public static ItemSelectionBase instance;

    public float DistanceThreshold = 10f;

    public bool showAll;

    void Awake()
    {
        instance = this;
        src = GetComponent<AudioSource>();
    }

    void Start()
    {
        if(showAll)
        {
            foreach(var v in Selectors)
            {
                v.Show();
            }
            Refresh();
        }
    }

    public static void ForceUpdate()
    {
        if(instance != null)
        {
            instance.Refresh();
        }
    }

    public void Refresh()
    {
        foreach (var v in Selectors)
        {
            v.Refresh();
        }
    }

	void FixedUpdate()
    {
        if ((NVR_Player.instance != null && NVR_Player.instance.NonVR) || showAll)
            return;
        if (left == null || right == null || !right.activeInHierarchy || !left.activeInHierarchy)
        {
            left = VRTK_DeviceFinder.GetControllerLeftHand();
            right = VRTK_DeviceFinder.GetControllerRightHand();
            return;
        }
        Vector3 p1 = left.transform.position;
        Vector3 p2 = right.transform.position;
        ItemChangeUI closest = Selectors[0];
        float dist = float.MaxValue;
        foreach(var v in Selectors)
        {
            float minDist = Mathf.Min(Vector3.Distance(v.transform.position, p1), Vector3.Distance(v.transform.position, p2));
            if (minDist > DistanceThreshold || !v.gameObject.activeSelf)
                return;
            if(minDist < dist)
            {
                dist = minDist;
                closest = v;
            }
        }
        if(dist < .3f)
        {
            foreach(var v in Selectors)
            {
                if(closest == v)
                {
                    v.Show();
                }
                else
                {
                    v.Hide();
                }
            }
            if(NVR_Player.isThirdPerson())
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    closest.Prev();
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    closest.Next();
            }
        }
        else
        {
            foreach(var v in Selectors)
            {
                v.Hide();
            }
        }
    }

    public void PlayChangeItem()
    {
        if(Clips.Length > 0 && src != null)
        {
            src.clip = Clips[Random.Range(0, Clips.Length)];
            src.Play();
        }
    }
}
