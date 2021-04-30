using System.Collections.Generic;

public class Vegetal : Entity
{
    private const short propagationCountdown = 4; // Le compte à rebours avant la propagation des végétaux
    private bool canPropagate = false; // Booléen qui indique la possibilité au végétal de se propager

    /// ////////////////////////////////////////
    ///  S'il s'agit d'un tour où les plantes peuvent agir, alors on met le booléen pour la possibilité
    ///  de se propager à true.
    ///  /////////////////////////////////////////
    public override void RefreshActions()
    {
        canPropagate = false;
        if(GameOfLife.TURN % propagationCountdown == 0)
        {
            canPropagate = true;
        }
    }

    /// ////////////////////////////////////////
    ///  S'il s'agit d'un tour où les plantes peuvent agir, ils se propagent sur toutes les cases voisines
    ///  qui n'ont pas de végétal.
    /// ////////////////////////////////////////
    public override void DoSomeActions()
    {
        if (canPropagate && GameOfLife.TURN % propagationCountdown == 0)
        {
            List<Cell> NeighboursWthtVegetal = ownerCell.Neighbours.FindAll(DoesCellIsFreeForVegetal); // Utilisation d'un prédicat
            for (int m = 0; m < NeighboursWthtVegetal.Count; m++)                                      // pour obtenir les cellules
            {                                                                                          // sans végétal
                NeighboursWthtVegetal[m].SpawnVegetal();
            }
        }
    }

    /// ///////////////////////////////////////// 
    /// Prédicat renvoyant true si la cellule filtré ne possède pas de végétal.
    /// ////////////////////////////////////////
    private bool DoesCellIsFreeForVegetal(Cell other)
    {
        return !other.Entities.Find(ownerCell.EntityWhichIsVegetal);
    }
}
