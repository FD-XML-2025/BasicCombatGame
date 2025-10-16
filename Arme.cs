using System;

namespace BasicCombatGame;

public class Arme
{
    private String nom { get; set; }
    private double dmg { get; set; }
    private double range { get; set; }

    public Arme(String nom, double dmg, double range)
    {
        this.nom = nom;
        this.dmg = dmg;
        this.range = range;
    }

    public void Equip()
    {
        
    }
}