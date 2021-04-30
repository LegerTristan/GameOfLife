using UnityEngine;

public class CellGrid : MonoBehaviour
{
    public const short ROW_CELL_NBR = 72, COLUMN_CELL_NBR = 48; //Le nombre de case par ligne et par colonne

    [SerializeField]
    public GameObject cellShape; // Le prefab de la case nécessaire à l'instanciation.

    /// /////////////////////////////////////////
    /// Création de la zone de jeu, on intialise un tableau multidimensionnel et on instancie une cellule
    /// sur chaque case.
    /// De plus, on fait apparaître une ou plusieurs entités (un végétal et un animal) sur la case en fonction d'un aléatoire.
    /// À la fin, on appelle la méthode CountNeighboursCell() pour compter les voisins de chaque cellule.
    /// ////////////////////////////////////////
    public Cell[,] CreateGrid()
    {
        Cell[,] zone = new Cell[ROW_CELL_NBR, COLUMN_CELL_NBR];
        for (int j = 0; j < COLUMN_CELL_NBR; j++)
        {
            for (int i = 0; i < ROW_CELL_NBR; i++)
            {
                Vector2 position = new Vector2(i, j);
                Cell cell = Instantiate(cellShape, position, Quaternion.identity).GetComponent<Cell>();
                cell.gameObject.name = "Cell" + i + " : " + j;  // On ajoute un nom et on parente chaque cellule à la grille
                cell.transform.SetParent(gameObject.transform); // pour une meilleure lisibilité lors des tests.
                zone[i, j] = cell;
                zone[i, j].SpawnEntity(); // Apparition d'une ou plusieurs entités sur la case
            }
        }
        CountNeighboursCell(zone);
        return zone;
    }

    /// /////////////////////////////////////////
    /// On parcourt à nouveau le tableau multidimensionnel pour compter le nombre de voisins de chaque case, qui sera nécéssaire
    /// pour simplifier les actions de chaque tour.
    /// Pour connaître la présence d'une case voisine, on fait une double boucle et on ajoute chaque celle qui est à l'intérieur
    /// des extrêmités à l'exception de la cellule dont on cherche les voisins.
    /// ////////////////////////////////////////
    public void CountNeighboursCell(Cell[,] zone)
    { 
        for (int j = 0; j < COLUMN_CELL_NBR; j++)
        {
            for (int i = 0; i < ROW_CELL_NBR; i++)
            {
                for (int x = i - 1; x < i + 2; x++) // On commence la double boucle en partant de la case en bas à gauche, puis 
                {                                   // on va jusqu'à celle en haut à droite.
                    for (int y = j - 1; y < j + 2; y++)
                    {
                        if (x >= 0 && x < ROW_CELL_NBR && y >= 0 && y < COLUMN_CELL_NBR)
                            if (x != i || y != j)
                            {
                                zone[i, j].AddNeighbour(zone[x, y]);
                            }
                    }
                }
            }
        }
    }
}
