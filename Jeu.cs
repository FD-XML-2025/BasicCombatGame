using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BasicCombatGame;

public class Jeu
{
    private int etagesTotal { get; set; }
    private Etage etageCourant { get; set; }
    public Joueur joueur { get; set; }
    private List<Ennemie> enemies { get; set; }
    private int score { get; set; }
    
    public void Initialize()
    {
        // 1. Définir le namespace XML qui est dans ton fichier
        XNamespace jeux = "http://www.univ-grenoble-alpes.fr/l3miage/jeux";
        XDocument doc = XDocument.Load("Content/jeu_init_2.xml");

        // 2. Utiliser ce namespace pour trouver les éléments
        // var joueurElement = doc.Root.Element(jeux + "joueur");
        double health = double.Parse(doc.Element("jeu").Element("joueur").Element("health").Value);
        double dmg = double.Parse(doc.Element("jeu").Element("joueur").Element("dmg").Value);

        // 3. Créer l'objet Joueur avec ces valeurs
        joueur = new Joueur(health, dmg);
    }

    public void LoadContent(ContentManager content)
    {
        joueur.LoadContent(content);
    }
    
    public void Update()
    {
        joueur.Update();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        joueur.Draw(spriteBatch);
    }
}