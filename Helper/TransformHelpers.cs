using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformHelpers
{
    public static Transform DeepFind(this Transform parent, string targetName)
    {
        Transform temp = null;
        foreach (Transform child in parent)
        {
            if (child.name == targetName)
            {
                return child;
            }
            else
            {
                temp = DeepFind(child, targetName);
                if (temp != null)
                {
                    return temp;
                }
            }

        }
        return null;
    }
}
