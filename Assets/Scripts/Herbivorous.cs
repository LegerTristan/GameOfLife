using System.Collections.Generic;

public class Herbivorous : Animal
{
    private const short HERBIVOROUS_MAX_HUNGER = 6, HERBIVOROUS_BREEDING_TURN = 2;  //Le nombre de tour qu'un herbivore
                                                                                    // peut passer sans se nourrir et le nombre
                                                                                    //de tour nécessaire à la reproduction.
    /// ////////////////////////////////////////
    /// On initialise la valeur de hunger en fonction du nombre de tour qu'un herbivore peut rester sans se nourrir.
    /// ////////////////////////////////////////
    void Awake()
    {
        hunger = HERBIVOROUS_MAX_HUNGER;
    }

    /// ////////////////////////////////////////
    /// Si sur cette case il y a un végétal, alors l'herbivore mange le végétal et restaure toute sa faim.
    /// ////////////////////////////////////////
    protected override void Feed()
    {
        Vegetal vegetal = ownerCell.Entities.Find(ownerCell.EntityWhichIsVegetal) as Vegetal;
        if (null != vegetal)
        {
            ResetHunger();
            ownerCell.RemoveEntity(vegetal);
            VegetalPool.Instance.PutBackToPool(vegetal);
        }
    }

    /// ////////////////////////////////////////
    /// On réinitialise la faim de l'animal à son maximum
    /// ////////////////////////////////////////
    protected override void ResetHunger()
    {
        hunger = HERBIVOROUS_MAX_HUNGER;
    }

    /// ////////////////////////////////////////
    /// Pour la réécriture de cette méthode, on filtre les cellules en fonction de celle qui possède un herbivore.
    /// ////////////////////////////////////////
    protected override List<Cell> GetCellsWithCompatibleAnimals()
    {
        return ownerCell.Neighbours.FindAll(DoesCellHaveAnHerbivorous);
    }

    /// ////////////////////////////////////////
    /// Prédicat qui renvoie true si la cellule possède un Herbivore
    /// ////////////////////////////////////////
    private bool DoesCellHaveAnHerbivorous(Cell other)
    {
        Herbivorous otherAnimal = other.Entities.Find(other.EntityWhichIsAnimal) as Herbivorous;
        return null !=  otherAnimal;
    }

    /// ////////////////////////////////////////
    /// On ajoute à la méthode mère, le compte à rebours de reproduction de l'herbivore. 
    /// ////////////////////////////////////////
    protected override void SetBreeding(Animal partnerAnimal)
    {
        base.SetBreeding(partnerAnimal);
        breedingCountdown = HERBIVOROUS_BREEDING_TURN;
    } 
}
