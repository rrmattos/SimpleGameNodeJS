using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IBrain
{
    void GetObjectComponents();
    bool HasComponent(Type type);
}

public class Brain : MonoBehaviour, IBrain
{
    protected Component[] objectComponents;
    protected List<Component> components = new List<Component>();
    protected int currentComponentsCount;

    private Type[] ignoredTypes = { typeof(Transform), typeof(GameObject) };

    protected void OnEnable()
    {
        objectComponents = gameObject.GetComponents<Component>();
        currentComponentsCount = objectComponents.Length;
        GetObjectComponents();
    }

    public virtual void GetObjectComponents()
    {
        components.Clear(); 

        foreach (Component component in objectComponents)
        {
            if (!ignoredTypes.Contains(component.GetType()))
            {
                components.Add(component);
            }
        }
    }

    public virtual bool HasComponent(Type type)
    {
        return components.Exists(component => component.GetType() == type);
    }
}


//public class Brain : MonoBehaviour
//{
//    private Type[] ignoredTypes = new Type[]
//    {
//        typeof(Transform),
//        typeof(GameObject),
//    };
//    protected Component[] objectComponents;


//    [HideInInspector] public virtual List<Component> components { get ; set; } = new List<Component>();
//    protected virtual int currentComponentsCount { get; set; }


//    protected void OnEnable()
//    {
//        objectComponents = gameObject.GetComponents<Component>();
//    }

//    public virtual void GetObjectComponents()
//    {
//        foreach(Component component in objectComponents)
//        {
//            //! --- Se o tipo do component não estiver na lista de tipos ignorados,
//            //!     é aicionado à lista ---
//            if (!ignoredTypes.Contains(component.GetType()) && !components.Contains(component))
//            {
//                components.Add(component);
//            }
//        }
//    }

//    public virtual bool HasComponent(Type _type)
//    {
//        return components.Any(component => component.GetType() == _type);
//    }

//    private void Update()
//    {
//        Debug.Log($"{objectComponents.Length} - {currentComponentsCount}");
//        if (objectComponents.Length > currentComponentsCount)
//        {
//            currentComponentsCount = objectComponents.Length;
//            GetObjectComponents();
//            Debug.Log($"Adicionado {objectComponents.Last()} - count: {currentComponentsCount}");
//        }
//    }
//}