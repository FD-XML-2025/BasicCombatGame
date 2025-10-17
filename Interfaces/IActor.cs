using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BasicCombatGame;

public interface IActor
{
    Vector2 Position { get; set; }

    void Update(GameTime gameTime);

    void Draw(SpriteBatch spriteBatch);

    void LoadContent(ContentManager content);
}