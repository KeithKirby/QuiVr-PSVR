using UnityEngine;
using System.Collections;

public class OwlHead : MonoBehaviour {

    public bool Looking;
    public Transform Mouth;

    public bool isTalking;
	

    float curAngle;
    bool opening;
    void LateUpdate()
    {
        if(Looking && PlayerHead.instance != null)
        {
            Vector3 pos = (PlayerHead.instance.transform.position - transform.position).normalized;
            pos = transform.position - pos;
            transform.LookAt(pos, Vector3.up);
            transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
        }
        Mouth.localEulerAngles = new Vector3(curAngle, 0, 0);
        curAngle = Mouth.localEulerAngles.x;
        if (isTalking)
        {
            if (opening && curAngle < 60)
            {
                Mouth.localEulerAngles += new Vector3(1, 0, 0) * Time.deltaTime * Random.Range(80f,100f);
            } 
            else if (opening && curAngle >= 60)
            {
                opening = false;
            }
            else if (!opening && curAngle > 35)
            {
                Mouth.localEulerAngles -= new Vector3(1, 0, 0) * Time.deltaTime * Random.Range(80f, 100f);
            }
            else if (!opening && curAngle <= 35)
            {
                opening = true;
            }
            curAngle = Mouth.localEulerAngles.x;
        }
        else
        {
            if(curAngle > 35)
                Mouth.localEulerAngles -= new Vector3(1, 0, 0) * Time.deltaTime * Random.Range(80f, 100f);
            else
                Mouth.localEulerAngles = new Vector3(35, 0, 0);
            curAngle = Mouth.localEulerAngles.x;
            opening = true;
        }
            
    }
}
