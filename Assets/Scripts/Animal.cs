using System;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Entity
{
    [SerializeField]
    protected short breedingCountdown, hunger;  // Le compte à rebours pour la reproduction et le nombre de tour
                                                // avant de mourir de faim représenté par des short int

    private Animal partnerAnimal; // L'animal partenaire  de celui-ci durant la reproduction.
    private bool canMove = true, canBreed = true, MaleGender; // Booléen pour indiquer la possibilité de se déplacer, de se
                                                              // reproduire, ainsi que le genre de l'animal


    /// ///////////////////////////////////////// 
    /// On définit le genre de l'animal aléatoirement via un random.
    /// ////////////////////////////////////////
    public void GiveRandomGender()
    {
        MaleGender = (UnityEngine.Random.value < 0.5) ? true : false;
    }

    /// ////////////////////////////////////////
    ///  On indique à chaque animal qu'il peut de nouveau se déplacer si celui-ci n'est pas en train de se reproduire.
    ///  Dans le cas où il se reproduit, on diminue le nombre de tour avant la naissance d'un nouvel animal et on contrôle
    ///  que l'animal partenaire est toujours vivant.
    ///  Si le compte à rebours pour la naissance est à 0, alors on appelle la méthode GiveBirth().
    ///  Enfin on baisse la nourriture de l'animal via la méthode UpdateHunger().
    ///  /////////////////////////////////////////
    public override void RefreshActions()
    {
        if (canBreed)
        {
            canMove = true;
        }
        else
        {
            if (null != partnerAnimal)
            {
                breedingCountdown--;
                if (breedingCountdown == 0)
                {
                    canBreed = true;  // On indique que l'animal peut de nouveau se déplacer
                    canMove = true;   // et se reproduire.

                    /* On vérifie également le countdown du partenaire afin d'éviter d'instancier 2 fois un animal.
                     * Par conséquent, le bébé né lorsque le second animal du couple a atteint le coutdown de 0 également.*/
                    if (partnerAnimal.breedingCountdown == 0)
                    {
                        GiveBirth();
                    }
                }
            }
            else
            {
                canBreed = true;      // Si l'animal partenaire est mort, on réinitialise tout.
                canMove = true;
                breedingCountdown = 0;
            }
        }
        UpdateHunger();
    }

    /// ///////////////////////////////////////// 
    /// On décrémente la satiété de l'animal.
    /// Si hunger est inférieur ou égale à 0, on détruit l'animal (mort de faim).
    /// ////////////////////////////////////////
    public void UpdateHunger()
    {
        hunger--;
        if (hunger <= 0)
        {
            ownerCell.RemoveEntity(this);
            if(this is Herbivorous)
            {
                HerbivorousPool.Instance.PutBackToPool((Herbivorous)this);
            }
            else
            {
                CarnivorousPool.Instance.PutBackToPool((Carnivorous)this);
            } 
        }
    }

    /// ////////////////////////////////////////
    /// En fonction de l'alimentation de l'animal, on donne une liste de cellule voisine où il peut se déplacer,
    /// une fois qu'il s'est déplacé, l'animal se nourrit.
    /// ////////////////////////////////////////
    public override void DoSomeActions()
    {
        if (canMove)
        {
            List<Cell> moveCell = GetFreeCellForMove();
            if (moveCell.Count > 0)
            {
                Move(moveCell);
            }
        }
        Feed();
    }

    /// ////////////////////////////////////////
    /// Méthode que les classes filles vont pouvoir réecrire en fonction de leur spécificité, dans le cas général,
    /// on prend seulement les cellulesz où il n'y a aucun animal.
    /// ////////////////////////////////////////
    protected virtual List<Cell> GetFreeCellForMove()
    {
        return ownerCell.Neighbours.FindAll(DoesCellIsFreeForAnimal);
    }

    /// ////////////////////////////////////////
    /// À partir d'un aléatoire, l'animal se déplace sur une nouvelle cellule
    /// en appelant RemoveEntity() pour se délier de sa celulle hôte et
    /// SetEntity() pour se lier à la nouvelle cellule.
    /// ////////////////////////////////////////
    private void Move(List<Cell> moveFreeCell)
    {
        int random = UnityEngine.Random.Range(0, moveFreeCell.Count);
        ownerCell.RemoveEntity(this);
        moveFreeCell[random].SetEntity(this);
        canMove = false; // On pense à désactiver la possibilité de se déplacer pour éviter qu'un animal se déplace de plusieurs cases.
    }

    /// ////////////////////////////////////////
    /// Prédicat qui renvoie true si la cellule filtré ne possède pas d'animal.
    /// ////////////////////////////////////////
    private bool DoesCellIsFreeForAnimal(Cell other)
    {
        return null == other.Entities.Find(other.EntityWhichIsAnimal);
    }

    /// ////////////////////////////////////////
    /// L'animal se nourrit s'il en a la possibilité.
    /// Les classes filles peuvent éditer cette méthode, actuellement elle renvoie qu'un log.
    /// ////////////////////////////////////////
    protected virtual void Feed()
    {
        Debug.Log("I'm feed");
    }

    /// ///////////////////////////////////////// 
    /// Les ckasses filles vont remettre à jour la satiété de l'animal en fonction de son alimentation.
    /// ////////////////////////////////////////
    protected virtual void ResetHunger()
    {
        Debug.Log("Hunger reset !");
    }

    /// ////////////////////////////////////////
    /// On vérifie si l'animal peut se reproduire.
    /// Le cas échéant, on boucle et on vérifie chaque case voisine s'il y a un animal de la même espèce, de genre opposé et si celui-ci
    /// peut aussi se reproduire.
    /// Si toutes ces conditions sont remplis alors on indique que ces deux animaux sont en reproduction et on définit leur
    /// animal partenaire comme étant l'un l'autre via la méthode SetBreeding()
    /// /// ////////////////////////////////////////
    public void SearchForBreeding()
    {
        if (canBreed)
        {
            List<Cell> cellsWithCompatibleAnimals = GetCellsWithCompatibleAnimals(); // On filtre les cellules pour avoir la
            Animal compatibleAnimal;                                                 // liste des partenaires de même espèce
            foreach (Cell cell in cellsWithCompatibleAnimals)
            {
                compatibleAnimal = cell.Entities.Find(ownerCell.EntityWhichIsAnimal) as Animal;
                if (compatibleAnimal.canBreed && MaleGender != compatibleAnimal.MaleGender)
                {
                    SetBreeding(compatibleAnimal);
                    compatibleAnimal.SetBreeding(this);
                }
            }
        }
    }

    /// ////////////////////////////////////////
    /// Méthode que les classes filles vont réécrire, elle renvoie la liste des partenaires de même espèce pour l'animal
    /// /// ////////////////////////////////////////
    protected virtual List<Cell> GetCellsWithCompatibleAnimals()
    {
        return null;
    }

    /// ////////////////////////////////////////
    /// On met à jour le booléen pour la reproduction et ont définit l'animal partenaire des deux animaux
    /// comme étant l'un l'autre.
    /// /////////////////////////////////////////
    protected virtual void SetBreeding(Animal partnerAnimal)
    {
        canBreed = false;
        this.partnerAnimal = partnerAnimal;
    }

    /// ////////////////////////////////////////
    /// On prend une cellule libre (sans un animal) pour y faire apparaître
    /// un animal de même espèce que les parents.
    /// De plus, on lui donne un genre aléatoire et on le lie à la cellule.
    /// /////////////////////////////////////////
    private void GiveBirth()
    {
        Cell cellForBreeding = ownerCell.Neighbours.Find(DoesCellIsFreeForAnimal);                                                          // y instancier un animal.
        if (null != cellForBreeding)
        {
            Animal bornAnimal = ownerCell.LoadAnimal(this);
            bornAnimal.ResetValue();
            bornAnimal.GiveRandomGender();
            cellForBreeding.SetEntity(bornAnimal);
        }
        // On réinitialise le partenaire animal pour éviter les erreurs lorsqu'un des deux animaux
        // voudra de nouveau se reproduire.
        partnerAnimal.partnerAnimal = null;
        partnerAnimal = null;
    }

    /// ///////////////////////////////////////// 
    /// On réinitialise les variables de l'Animal qui vient du pool, et on redonne un genre aléatoirement.
    /// ////////////////////////////////////////
    private void ResetValue()
    {
        GiveRandomGender();
        ResetHunger();
        canMove = true;
        canBreed = true;
        partnerAnimal = null;
    }
}
