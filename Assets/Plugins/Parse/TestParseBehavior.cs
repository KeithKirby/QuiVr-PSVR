using UnityEngine;
using System.Collections;
using Parse;
public class TestParseBehavior : MonoBehaviour {

	IEnumerator Start()
	{
        yield return true;
		yield return new WaitForSeconds(0.5f);
		ParseQuery<ParseObject> query = ParseObject.GetQuery("Custom");
		var Task = query.GetAsync("YcSOAcoX0L");
		while(!Task.IsCompleted)
		{
			yield return true;
		}
		if(Task.IsFaulted || Task.IsCanceled)
		{
			Debug.LogError("Task Failed: " + Task.Exception.Message);
		}
		if(Task.Exception == null)
		{
            string v = Task.Result.Get<string>("StringData1");
            Debug.Log(v);
            Task.Result["StringData1"] = "" + (float.Parse(v) * 2);
            Task.Result.SaveAsync();
		}
        Debug.Log("Ended");
	}
}
