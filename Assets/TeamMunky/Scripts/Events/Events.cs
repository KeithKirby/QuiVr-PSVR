using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Madorium.Events
{
    public class Events
    {
        public delegate void EventHandler();
        public delegate void TypedEventHandler<T>(T val);
        public delegate void TypedEventHandler<T, U>(T param1, U param2);
    }
}