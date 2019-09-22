using UnityEngine;
using System.Collections;
using Parse;
public class ParseInitBehavior : ParseInitializeBehaviour {

    public static bool initialized;

	public override void Awake()
	{
        if (initialized)
            Destroy(this);
        initialized = true;
        try
        {
            base.applicationID = "QuiVr";
            base.dotnetKey = "H22S9I95anJC6ipm0UgZeabFp819Dr4q";
            base.server = "http://34.198.149.161:4040/parse/";
            base.Awake();
        }
        catch(System.Exception e) {
            Debug.LogError("Failed to initialize Parse: " + e.ToString());
        }
    }
}
