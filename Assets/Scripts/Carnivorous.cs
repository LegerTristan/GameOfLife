using System.Collections.Generic;

public class Carnivorous : Animal
{
    private const short CARNIVOROUS_MAX_HUNGER = 9, CARNIVOROUS_BREEDING_TURN = 3; //Le nombre de tour qu'un carnivore 
                                                                                    // peut passer sans se nourrir et le nombre
                                                                                    // de tour nécessaire pour se reproduire.
    /// ////////////////////////////////////////
    /// On initialise la valeur de hunger en fonction du nombre de tour qu'un carnivore peut rester sans se nourrir.
    /// ////////////////////////////////////////
    void Awake()
    {
        hunger = CARNIVOROUS_MAX_HUNGER;
    }

    /// ///////////////////////////////////////// 
    /// Méthode réécrite renvoyant la liste des cellules possédant un herbivore ou aucun animal,
    /// c'est-à-dire toutes les cellules sauf celle ayant un carnivore.
    /// ////////////////////////////////////////
    protected override List<Cell> GetFreeCellForMove()
    {
        List<Cell> cells = ownerCell.Neighbours.FindAll(DoesCellIsFreeOrHaveAHerbivorous);
        return cells;
    }

    /// ///////////////////////////////////////// 
    /// Prédicat renvoyant true si la cellule filtré possède un herbivore ou ne possède pas d'animaux.
    /// ////////////////////////////////////////
    private bool DoesCellIsFreeOrHaveAHerbivorous(Cell other)
    {
        Animal otherAnimal = other.Entities.Find(other.EntityWhichIsAnimal) as Animal;
        return null == otherAnimal || otherAnimal is Herbivorous;
    }

    /// ///////////////////////////////////////// 
    /// On détermine les cases où il n'y a pas de carnivore pour qu'il puisse se déplacer,
    /// de plus, si sur cette case il y a un herbivore, alors le carnivore mange l'herbivore et restaure toute
    /// sa faim.
    /// /// ///////////////////////////////////////// 
    protected override void Feed()
    {
        Herbivorous herbivorous = ownerCell.Entities.Find(EntityWhichIsHerbivorous) as Herbivorous;
        if (null != herbivorous)
        {
            ResetHunger();
            ownerCell.RemoveEntity(herbivorous); // On pense à délier l'herbivore à sa cellule hôte pour éviter les erreurs.
            HerbivorousPool.Instance.PutBackToPool(herbivorous);
        }
    }

    /// ///////////////////////////////////////// 
    /// Prédicat renvoyant true si l'entité filtrée est un herbivore
    /// ////////////////////////////////////////
    private bool EntityWhichIsHerbivorous(Entity other)
    {
        return other is Herbivorous;
    }

    /// ////////////////////////////////////////
    /// On réinitialise la faim de l'animal à son maximum
    /// ////////////////////////////////////////
    protected override void ResetHunger()
    {
        hunger = CARNIVOROUS_MAX_HUNGER;
    }

    /// ////////////////////////////////////////
    /// Pour la réécriture de cette méthode, on filtre les cellules en fonction de celle qui possède un carnivore.
    /// ////////////////////////////////////////
    protected override List<Cell> GetCellsWithCompatibleAnimals()
    {
        return ownerCell.Neighbours.FindAll(DoesCellHaveACarnivorous);
    }

    /// ///////////////////////////////////////// 
    /// Prédicat renvoyant true si la cellule filtrée possède un carnivore
    /// ////////////////////////////////////////
    private bool DoesCellHaveACarnivorous(Cell other)
    {
        Carnivorous otherAnimal = other.Entities.Find(other.EntityWhichIsAnimal) as Carnivorous;
        return null != otherAnimal;
    }

    /// ////////////////////////////////////////
    /// On ajoute à la méthode mère, le compte à rebours de reproduction du carnivore. 
    /// ////////////////////////////////////////
    protected override void SetBreeding(Animal partnerAnimal)
    {
        base.SetBreeding(partnerAnimal);
        breedingCountdown = CARNIVOROUS_BREEDING_TURN;
    }
}
