using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHolder : MonoBehaviour {

    public static PlayerHolder instance;
    public ChestFollow chest;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (chest != null)
            chest.down = Down();
    }

    public static Vector3 Forward()
    {
        if (instance != null)
            return instance.transform.forward;
        return Vector3.forward;
    }

    public static Vector3 Up()
    {
        if (instance != null)
            return instance.transform.up;
        return Vector3.up;
    }

    public static Vector3 Right()
    {
        if (instance != null)
            return instance.transform.right;
        return Vector3.right;
    }

    public static Vector3 Left()
    {
        if (instance != null)
            return -1*instance.transform.right;
        return Vector3.left;
    }

    public static Vector3 Down()
    {
        if (instance != null)
            return -1 * instance.transform.up;
        return Vector3.down;
    }

    public static Vector3 TransformDirection(Vector3 inpt)
    {
        if (instance != null)
            return instance.transform.TransformDirection(inpt);
        return inpt;
    }

    public static Vector3 InverseTransformDirection(Vector3 inpt)
    {
        if (instance != null)
            return instance.transform.InverseTransformDirection(inpt);
        return inpt;
    }
}
