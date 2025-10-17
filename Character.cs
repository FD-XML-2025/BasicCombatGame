using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;

namespace BasicCombatGame;

/*
 * A character is an entity with an animated sprite.
 */
public class Character : Entity
{
    private SpriteSheet _spriteSheet;
    private AnimationController _animationController;
    private string _spriteSheetAsset;
    private string _animState;

    public Character()
    {
        
    }
    
    public Character(string spriteSheetAsset, string spriteName)
    {
        SpriteName        = spriteName;
        _spriteSheetAsset = spriteSheetAsset;
    }
    
    public void LoadContent(ContentManager contentManager)
    {
        base.LoadContent(contentManager);
        
        Texture2D adventurerTexture = contentManager.Load<Texture2D>(_spriteSheetAsset);
        Texture2DAtlas atlas = Texture2DAtlas.Create("Atlas/"+SpriteName, adventurerTexture, 128, 128);
        _spriteSheet = new SpriteSheet("SpriteSheet/"+SpriteName, atlas);

        _spriteSheet.DefineAnimation("idle", builder =>
        {
            builder.IsLooping(true)
                .AddFrame(0, TimeSpan.FromSeconds(0.1))
                .AddFrame(1, TimeSpan.FromSeconds(0.1))
                .AddFrame(2, TimeSpan.FromSeconds(0.1))
                .AddFrame(3, TimeSpan.FromSeconds(0.1))
                .AddFrame(4, TimeSpan.FromSeconds(0.1))
                .AddFrame(5, TimeSpan.FromSeconds(0.1))
                .AddFrame(6, TimeSpan.FromSeconds(0.1))
                .AddFrame(7, TimeSpan.FromSeconds(0.1));
        });

        SpriteSheetAnimation idleAnimation = _spriteSheet.GetAnimation("idle");
        _animationController = new AnimationController(idleAnimation);
    }

    public void Update(GameTime gameTime)
    {
        _animationController.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Texture2DRegion currentFrameTexture = _spriteSheet.TextureAtlas[_animationController.CurrentFrame];
        spriteBatch.Draw(currentFrameTexture, Vector2.Zero, Color.White, 0.0f, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0.0f);
    }
}