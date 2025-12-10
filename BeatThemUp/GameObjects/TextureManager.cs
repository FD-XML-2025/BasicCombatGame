using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class TextureManager
{
    public static Texture2D Pixel;

    public static void Init(GraphicsDevice device)
    {
        Pixel = new Texture2D(device, 1, 1);
        Pixel.SetData<Color>(new Color[] { Color.White }); // <- important
    }
}