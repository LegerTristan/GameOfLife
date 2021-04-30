using UnityEngine;
using UnityEngine.UI;

public class GameOfLife : MonoBehaviour
{
    public float speed = 1f; // Vitesse d'écoulement du temps dans le jeu

    public static int TURN = 0; // Nombre de tours écoulés depuis le début du jeu

    [SerializeField]
    private GameObject menuPanel; // Le panel du menu.

    [SerializeField]
    private Text turnText; // Le composant Text qui gère la mise à jour du nombre de tour écoulé sur l'interface utilisateur.

    private float timer = 0f; // Chronomètre qui déclare le changement de tour lorsqu'il atteint la valeur de 0.1

    [SerializeField]
    private CellGrid grid; // Composant Grille qui est chargé de la création et de l'attribution des voisins de la zone.

    private Cell[,] zone; // Le tableau 2D contenant toutes les cases de la grille.


    /// /////////////////////////////////////////
    /// Lors du lancement du jeu, on demande au champ grid de créer la zone de jeu.
    /// On initialise également notre champ statique Instance, pour l'utiliser plus tard.
    /// 
    /// On utilise un tableau multidimensionnel pour accéder aux données plus rapidement,
    /// et aussi pour avoir une taille prédéfinie.
    /// ////////////////////////////////////////
    private void Start()
    {
        zone = grid.CreateGrid();
    }

    /// /////////////////////////////////////////
    /// Lors de l'Update, on augmente la valeur du timer en fonction de la vitesse d'écoulement du temps multipliée par une vitesse
    /// passée en variables précédemment.
    /// Lorsque le chronomètre (timer) est supérieur à 0.1, on incrémente le nombre de tour et le jeu joue ses actions du tour.
    /// 
    /// Dans le Jeu de la vie, un tour est similaire à une journée,
    /// durant laquelle chaque entité va effectuer une action (Se déplacer, se reproduire, se nourrir etc).
    /// Chaque entité peut être un animal, herbivore ou carnivore, ou un végétal.
    /// Chaque journée (et donc chaque tour) va se dérouler en 3 parties :
    /// 
    /// - Le rafraîchissement des actions : On indique aux animaux qu'ils peuvent de nouveau se déplacer, et on baisse
    ///   leur nourriture.
    ///   On diminue également le nombre de tour avant la naissance d'un nouvel animal pour les animaux qui sont en train de
    ///   se reproduire.
    ///   S'il s'agit d'un tour où les plantes peuvent agir, on met à jour leur possibilité de se propager via un booléen.
    ///   
    /// - Les actions du tour : On parcourt à nouveau chaque case de la zone et sur chacune d'elle, les animaux vont se déplacer
    ///   et se nourrir. S'il s'agit d'un herbivore, alors on détermine les cases où il n'y a pas d'autres animaux pour qu'il
    ///   puisse se déplacer et se nourrir s'il y a un végétal.
    ///   S'il s'agit d'un carnivore, alors on détermine les cases où il n'y a pas de carnivore pour qu'il puisse se déplacer,
    ///   de plus, si sur cette case il y a un herbivore, alors le carnivore mange l'herbivore et restaure toute
    ///   sa faim.
    ///   
    /// - La recherche pour reproduction : On parcourt une dernière fois chaque case de la zone et sur chacune d'elle, s'il y a un animal,
    ///   on vérifie s'il peut se reproduire.
    ///   Le cas échéant, on regarde sur chaque case voisine s'il y a un animal de la même espèce, de genre opposé et si celui-ci
    ///   peut aussi se reproduire.
    ///   Si toutes ces conditions sont remplis alors ces deux animaux sont dans une phase de reproduction où il ne se déplace
    ///   plus pendant un certain nombre de tours. 
    /// ////////////////////////////////////////
    void Update()
    {
        if(timer >= .1f)
        {
            timer = 0f;
            UpdateTurn();
            RefreshLifeAction(); // Le rafraîchissement des actions
            PlayTurn(); // Chaque entité joue son action
            SearchForBreeding(); // La recherche pour reproduction
        }
        else
        {
            timer += Time.deltaTime * speed;
        }

        // On peut mettre le jeu en pause via la touche P.
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Cursor.visible)
            {
                menuPanel.SetActive(false);
                Cursor.visible = false;
            }
            else
            {
                menuPanel.SetActive(true);
                Cursor.visible = true;
            }
        }
    }

    /// /////////////////////////////////////////
    ///  Pour chaque case, on met à jour certaines variables de chaque entité via la méthode RefreshLifeAction().
    /// ////////////////////////////////////////
    private void RefreshLifeAction()
    {
        for (int j = 0; j < CellGrid.COLUMN_CELL_NBR; j++)
        {
            for (int i = 0; i < CellGrid.ROW_CELL_NBR; i++)
            {
                for (int k = 0; k < zone[i, j].Entities.Count; k++)
                {
                    zone[i, j].Entities[k].RefreshActions();
                }
            }
        }
    }

    /// ///////////////////////////////////////// 
    /// On parcourt chaque case de la zone et sur chacune d'elle, on fait jouer les actions de chaque entité via la méthode
    /// DoSomeActions();
    /// ////////////////////////////////////////
    public void PlayTurn()
    {

        for (int j = 0; j < CellGrid.COLUMN_CELL_NBR; j++)
        {
            for (int i = 0; i < CellGrid.ROW_CELL_NBR; i++)
            {
                for (int k = 0; k < zone[i, j].Entities.Count; k++)
                {
                    zone[i, j].Entities[k].DoSomeActions();
                }
            }
        }
    }

    /// ///////////////////////////////////////// 
    /// On parcourt chaque case de la zone et sur chacune d'elle, s'il y a un animal,
    /// on appelle la méthode SearchForBreeding() qui va chercher si l'animal peut se reproduire
    /// ////////////////////////////////////////
    private void SearchForBreeding()
    {
        Animal animalPlaying;
        for (int j = 0; j < CellGrid.COLUMN_CELL_NBR; j++)
        {
            for (int i = 0; i < CellGrid.ROW_CELL_NBR; i++)
            {
                for (int k = 0; k < zone[i, j].Entities.Count; k++)
                {
                    animalPlaying = zone[i, j].Entities.Find(zone[i, j].EntityWhichIsAnimal) as Animal;
                    if(null != animalPlaying)
                    {
                        animalPlaying.SearchForBreeding();
                    }
                }
            }
        }
    }

    /// ///////////////////////////////////////// 
    /// On met à jour le nombre de tours écoulés depuis le début du jeu via le composant texte affiché en haut de l'écran.
    /// ////////////////////////////////////////
    private void UpdateTurn()
    {
        TURN++;
        turnText.text = "Tour : " + TURN;
    }

    /// ///////////////////////////////////////// 
    /// Méthode pour quitter le jeu ou l'éditeur.
    /// ////////////////////////////////////////
    public void LeaveGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}


