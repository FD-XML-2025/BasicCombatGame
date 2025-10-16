namespace BasicCombatGame;

public class Joueur
{
    private double hp { get; set; }
    private double dmg { get; set; }
    private Arme arme { get; set; }

    public Joueur(double hp, double dmg, Arme arme)
    {
        this.hp = hp;
        this.dmg = dmg;
        this.arme = arme;
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