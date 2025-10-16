using System.Collections.Generic;

namespace BasicCombatGame;

public class Etage
{
    private double hauteur { get; set; }
    private double largeur { get; set; }
    private List<Ennemie> enemies { get; set; }

    public Etage(double hauteur, double largeur, List<Ennemie> enemies)
    {
        this.hauteur = hauteur;
        this.largeur = largeur;
        this.enemies = enemies;
    }
}