using System;
using System.Runtime.CompilerServices;

namespace BasicCombatGame;

public class Ennemie
{
    private double hp { get; set; }
    private String type { get; set; }

    public Ennemie(double hp, String type)
    {
        this.hp = hp;
        this.type = type;
    }
    public void Move()
    {
        
    }

    public void Attack()
    {
        
    }

    public void TakeDamage()
    {
        
    }
}