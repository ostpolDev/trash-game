using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Utility.Drawing;

public static class TextureExtensions {

    public static void Blit(this Texture2D target, Texture2D source, Rectangle sourceArea, Rectangle destinationArea) {
        Color[] colors = new Color[sourceArea.Width * sourceArea.Height];
        source.GetData(0, sourceArea, colors, 0, colors.Length);
        target.SetData(0, destinationArea, colors, 0, colors.Length);
    }

    public static Texture2D Slice(this Texture2D source, GraphicsDevice device, Rectangle sourceArea, bool mipmap = false) {
        Color[] colors = new Color[sourceArea.Width * sourceArea.Height];
        source.GetData(0, sourceArea, colors, 0, colors.Length);

        Texture2D texture2D = new(device, sourceArea.Width, sourceArea.Height, mipmap, SurfaceFormat.Color);
        texture2D.SetData(colors);

        return texture2D;
    }

}
