using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace U3D.Extensions
{
    public static class TransformExtensions
    {
        public static bool BelongsToHierarchy(this Transform subject, Transform hierarchy)
        {
            if (subject == null)
                return false;
            else if (subject == hierarchy)
                return true;
            else
                return BelongsToHierarchy(subject.transform.parent, hierarchy);
        }
    }
}
