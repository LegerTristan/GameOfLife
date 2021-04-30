using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Cell ownerCell; // La cellule hôte de l'entité

    /// /////////////////////////////////////////
    /// On met à jour la cellule hôte de l'entité.
    /// ////////////////////////////////////////
    public void SetOwnerCell(Cell cell)
    {
        ownerCell = cell;
    }

    /// ////////////////////////////////////////
    ///  La méthode RefreshActions() qui va être réécrite par les classes filles de la classe Entité, à savoir Vegetal et
    ///  Animal.
    ///  On indique à chaque animal qu'il peut de nouveau se déplacer si celui-ci n'est pas en train de se reproduire.
    ///  Dans le cas où il se reproduit, on diminue le nombre de tour avant la naissance d'un nouvel animal et on contrôle
    ///  que l'animal partenaire est toujours vivant.
    ///  Si le compte à rebours pour la naissance est à 0, alors on instancie un nouvel animmal de la même espèce et d'un genre
    ///  aléatoire.
    ///  Enfin on baisse la nourrture de tout les animaux.
    ///  
    ///  S'il s'agit d'un tour où les plantes peuvent agir, alors on vérifie si au moins une case voisine possède un végétal,
    ///  le cas échéant, on indique à cette case via un booléen qu'elle aura un végétal durant les actions de ce tour.
    ///  /////////////////////////////////////////

    public virtual void RefreshActions()
    {
        Debug.Log("Refresh !");
    }

    /// ////////////////////////////////////////
    /// La méthode DoSomeActions() qui va être réécrite par les classes filles de la classe Entité, à savoir Vegetal et
    /// Animal.
    /// son alimentation. S'il s'agit d'un herbivore, alors on détermine les cases où il n'y a pas d'autres animaux pour qu'il
    /// puisse se déplacer, de plus, si sur cette case il y a un végétal, alors l'herbivore mange le végétal et restaure toute
    /// sa faim.
    /// S'il s'agit d'un carnivore, alors on détermine les cases où il n'y a pas de carnivore pour qu'il puisse se déplacer,
    /// de plus, si sur cette case il y a un herbivore, alors le carnivore mange l'herbivore et restaure toute
    /// sa faim.
    /// Quant aux végétaux, ils se propagent s'il s'agit d'un tour divisible par 4.
    /// ////////////////////////////////////////
    public virtual void DoSomeActions()
    {
        Debug.Log("Do Something !");
    }
}
