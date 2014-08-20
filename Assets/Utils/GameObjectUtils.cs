using UnityEngine;
using System.Collections.Generic;

public static class GameObjectUtils
{
    public static T GetInterfaceObject<T>(GameObject go) where T : class
    {
        return go.GetComponent(typeof(T)) as T;
    }

    public static T[] GetInterfaceObjects<T>(GameObject go) where T : class
    {
        List<T> res = new List<T>();

        Component[] components = go.GetComponents(typeof(Component));
        foreach (Component component in components)
        {
            if(component is T)
            {
                res.Add(component as T);
            }
        }

        return res.ToArray();
    }

    public static T FindRecursiveComponentByTypeDown<T>(GameObject go) where T : class
    {
        T res = default(T);

        res = GetInterfaceObject<T>(go);
        if (res != null)
        {
            return res;
        }

        foreach (Transform child in go.transform)
        {
            res = (T)GameObjectUtils.FindRecursiveComponentByTypeDown<T>(child.gameObject);
            if (res != null)
            {
                return res;
            }
        }

        return res;
    }

    public static T[] FindRecursiveComponentsByTypeDown<T>(GameObject go) where T : class
    {
        List<T> components = new List<T>();
        
        T[] res = GetInterfaceObjects<T>(go);
        if (res != null)
        {
            foreach (T component in res) 
            {
                components.Add(component);
            }
        }

        foreach (Transform child in go.transform)
        {
            res = (T[])GameObjectUtils.FindRecursiveComponentsByTypeDown<T>(child.gameObject);
            if (res != null)
            {
                foreach (T component in res) 
                {
                    components.Add(component);
                }
            }
        }
        
        return components.ToArray();;
    }

    public static T FindRecursiveComponentByTypeUp<T>(GameObject go) where T : class
    {
        T res = default(T);
        
        res = GetInterfaceObject<T>(go);
        if (res != null)
        {
            return res;
        }

        GameObject parentGO = go.transform.parent.gameObject;
        if (parentGO != null)
        {
            return GameObjectUtils.FindRecursiveComponentByTypeDown<T>(parentGO);
        }

        return res;
    }
}
