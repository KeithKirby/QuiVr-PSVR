using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.Events;

public class LineButton : MonoBehaviour {

    public bool used;
    VRTK_InteractableObject io;

    public UnityEvent OnUse;

    public bool move;
    public Vector3 MoveValue;

    // Use this for initialization
    void Start () {
        io = GetComponentInChildren<VRTK_InteractableObject>();
        io.InteractableObjectUsed += Use;
    }
	
    public void Reset()
    {
        used = false;
    }

	void Use(object o, InteractableObjectEventArgs e)
    {
        if (used)
            return;
        used = true;
        OnUse.Invoke();
    }

    void Update()
    {
        if(used && move)
        {
            transform.position += MoveValue * Time.deltaTime;
            if (transform.position.y < -30)
                Destroy(gameObject);
        }
    }
}
