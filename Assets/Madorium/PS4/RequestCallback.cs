using System;
using System.Collections.Generic;

public class RequestCallbacks<T>
{
    class Entry
    {
        public Action<T> Complete;
        public T Response;
    }

    public void Add(Action<T> complete, T response)
    {
        _requests.Add(new Entry() { Complete = complete, Response = response });
    }

    public void Complete(T response)
    {
        var req = _requests.Find((entry) => { return entry.Response.Equals(response); });
        _requests.Remove(req);
        if (null != req.Complete)
            req.Complete(req.Response);
    }

    List<Entry> _requests = new List<Entry>();
}