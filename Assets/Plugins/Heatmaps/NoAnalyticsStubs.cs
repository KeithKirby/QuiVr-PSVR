using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heatmap
{
    public class AnalyticsResult
    {

    }

    public class Analytics
    {
        static public AnalyticsResult CustomEvent(string path, Dictionary<string, object> dict)
        {
            return new AnalyticsResult();
        }
    }
}