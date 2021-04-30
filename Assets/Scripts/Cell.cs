using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField]
    public List<Cell> Neighbours { get; private set; } // Liste des cellules voisines, lisible en publique mais l'édition est privée
    public List<Entity> Entities; // { get; private set; } // Liste des entités présentes sur la case, lisible en publique mais l'édition est privée

    /// /////////////////////////////////////////
    /// On initialise les deux listes et on charge les ressources de chaque prefab
    /// nécessaires pour instancier les différentes entités.
    /// ////////////////////////////////////////
    private void Awake()
    {
        Neighbours = new List<Cell>();
        Entities = new List<Entity>();
    }

    /// /////////////////////////////////////////
    /// On ajoute une nouvelle cellule à la liste des cellules voisines.
    /// ////////////////////////////////////////
    public void AddNeighbour(Cell cell)
    {
        Neighbours.Add(cell);
    }

    /// /////////////////////////////////////////
    /// On utilise un random pour déterminer si l'on ajoute un végétal et/ou un animal à cette case en début de jeu.
    /// 
    /// Axe d'amélioration : Écrire un script d'ObjectPool pour charger au préalable plusieurs instance d'Animal
    /// afin de pallier à l'instanciation et à la destruction répétitive de GameObjects.
    /// Une fois les GameObjects chargés, il suffirait de les activer ou désactiver via SetActive() et de les associer à la case
    /// qui leur correspond.
    /// ////////////////////////////////////////
    public void SpawnEntity()
    {
        if (Random.value > .75f)
        {
            SpawnVegetal();
        }

        if (Random.value > .85f)
        {
            SpawnAnimal();
        }
    }

    /// /////////////////////////////////////////
    /// On instancie un nouveau végétal et on appelle la méthode SetEntity() qui se charge
    /// de mettre à jour les valeurs nécessaires pour lier le végétal à la cellule.
    /// ////////////////////////////////////////
    public void SpawnVegetal()
    {
        Vegetal newVegetal = VegetalPool.Instance.GetFromPool();
        SetEntity(newVegetal);
    }

    /// /////////////////////////////////////////
    /// On instancie un nouvel animal et on appelle la méthode GiveRandomGender() pour lui donne un genre et
    /// SetEntity() qui se charge de mettre à jour les valeurs nécessaires pour lier l'animal à la cellule.
    /// ////////////////////////////////////////
    private void SpawnAnimal()
    {
        Animal newAnimal = LoadAnimal();
        newAnimal.GiveRandomGender();
        SetEntity(newAnimal);
    }

    /// /////////////////////////////////////////
    /// On positionne l'entité sur la cellule, puis on déclare sa cellule hôte comme étant celle-ci.
    /// Enfin, l'entité est ajouté à la liste des entités de la cellule.
    /// ////////////////////////////////////////
    public void SetEntity(Entity entity)
    {
        entity.transform.position = transform.position;
        entity.SetOwnerCell(this);
        Entities.Add(entity);
    }

    /// /////////////////////////////////////////
    /// On retire l'entité de la liste des entités de la cellule
    /// ////////////////////////////////////////
    public void RemoveEntity(Entity entity)
    {
        Entities.Remove(entity);
    }

    /// /////////////////////////////////////////
    /// Prédicat retournant true si l'entité filtré est un animal.
    /// ////////////////////////////////////////
    public bool EntityWhichIsAnimal(Entity other)
    {
        return other is Animal;
    }

    /// /////////////////////////////////////////
    /// Prédicat retournant true si l'entité filtré est un végétal.
    /// ////////////////////////////////////////
    public bool EntityWhichIsVegetal(Entity other)
    {
        return other is Vegetal;
    }

    /// /////////////////////////////////////////
    /// Instanciation d'un nouvel animal dont l'espèce est déterminée aléatoirement
    /// ////////////////////////////////////////
    public Animal LoadAnimal()
    {
        if (Random.value > .4f)
        {
            
            return HerbivorousPool.Instance.GetFromPool();
        }
        else
        {
            return CarnivorousPool.Instance.GetFromPool();
        }
    }

    /// /////////////////////////////////////////
    /// Surcharge de la méthode LoadAnimal, utilisé pour la reproduction.
    /// Cette fois-ci l'espèce est déterminé par l'animal passé en paramètre.
    /// ////////////////////////////////////////
    public Animal LoadAnimal(Animal animal)
    {
        if (animal is Herbivorous)
        {
            return HerbivorousPool.Instance.GetFromPool();
        }
        else
        {
            return CarnivorousPool.Instance.GetFromPool();
        }
    }
}
