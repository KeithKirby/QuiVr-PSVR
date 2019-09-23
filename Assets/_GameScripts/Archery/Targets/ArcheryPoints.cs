using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArcheryPoints : MonoBehaviour {

    public EMOpenCloseMotion Motion;
    public Text PointText;


    public void ShowPoints(int val, Vector3 pos)
    {
        transform.position = pos;
        transform.localScale = Vector3.one * (Vector3.Distance(pos, PlayerHead.instance.transform.position) / 30);
        PointText.text = "" + val;
        StopCoroutine("Close");
        StartCoroutine("Close");
    }

    IEnumerator Close()
    {
        yield return true;
        Motion.SetStateToClose();
        Motion.Open();
        while (Motion.motionState != EMBaseMotion.MotionState.Open)
        {
            yield return true;
        }
        Motion.Close();
        StopCoroutine("Close");
    }

    [BitStrap.Button]
    public void TestPoints()
    {
        ShowPoints(100, transform.position);
    }
}
