using System.Collections.Generic;
using UnityEngine;

public class GenericPool<T> : MonoBehaviour where T : Component
{
    /// ///////////////////////////////////////// 
    /// Utilisation du pattern ObjectPool pour réduire la consommation CPU du garbage collector.
    /// On instancie un nouvel objet de type générique lorsque nécessaire, puis on l'active et le sort de la file
    /// lorsqu'un objet de ce type est demandé.
    /// Lorsqu'on en a plus besoin, on le désactive et le remet dans la file pour plus tard.
    /// Cela évite d'allouer et désallouer de la mémoire en boucle ce qui aurait un coût non négligeable pour le CPU.
    /// ////////////////////////////////////////
 
    public static GenericPool<T> Instance; // Utilisation du pattern Singleton : on crée une instance statique de la GenericPool

    [SerializeField]
    private T objectShape; // Préfab de l'objet générique

    private Queue<T> objects; // File d'objets génériques

    /// ///////////////////////////////////////// 
    /// On initialise l'instance, et la file d'objets
    /// ////////////////////////////////////////
    private void Awake()
    {
        Instance = this;
        objects = new Queue<T>();
    }

    /// ///////////////////////////////////////// 
    /// On récupère un objet de la file s'il y en a,
    /// sinon on instancie un nouvel objet que l'on envoie à la place.
    /// ////////////////////////////////////////
    public T GetFromPool()
    {
        if(0 == objects.Count)
        {
            AddObjectToPool();
        }
        T objectFromPool = objects.Dequeue();
        objectFromPool.gameObject.SetActive(true); // On pense à activer l'objet pour l'utiliser.
        return objectFromPool;
    }

    /// ///////////////////////////////////////// 
    /// On remet l'objet dans la file et on le désactive
    /// ////////////////////////////////////////
    public void PutBackToPool(T objectToPutBack)
    {
        objectToPutBack.gameObject.SetActive(false);
        objects.Enqueue(objectToPutBack);
    }

    /// ///////////////////////////////////////// 
    /// On instancie un nouvel objet à partir du préfab, puis on l'ajoute à la file.
    /// ////////////////////////////////////////
    private void AddObjectToPool()
    {
        T newObject = Instantiate(objectShape);
        objects.Enqueue(newObject);
    }
}
