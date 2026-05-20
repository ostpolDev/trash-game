using Microsoft.Xna.Framework;

namespace Engine.Utility.Drawing;

public class IsometricHelper(int tileWidth, int tileHeight, int squashX = 0, int squashY = 0) {

    private readonly int TileWidth = tileWidth;
    private readonly int TileHeight = tileHeight;

    private readonly int HalfTileWidth = tileWidth / 2;
    private readonly int HalfTileHeight = tileHeight / 2;

    public readonly int SquashX = squashX;
    public readonly int SquashY = squashY;

    public Point MapToScreenPosition(int x, int y) {
        return new(
                (x - y) * (HalfTileWidth - SquashX),
                (x + y) * (HalfTileHeight - SquashY)
            );
    }

    public Point ScreenToMapPosition(int x, int y) {
        return new(
                (x / HalfTileWidth + y / HalfTileHeight) / 2,
                (y / HalfTileHeight - (x / HalfTileWidth)) / 2
            );
    }

    public Rectangle GetDestination(int x, int y, int width, int height) {
        return new(MapToScreenPosition(x, y), new Point(width, height));
    }

    public Rectangle GetDestination(int x, int y) {
        return GetDestination(x, y, TileWidth, TileHeight);
    }

}
