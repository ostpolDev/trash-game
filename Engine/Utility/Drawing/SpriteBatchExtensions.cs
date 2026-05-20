using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Engine.Utility.Drawing;

// Slightly modified from:
// https://github.com/MonoGame-Extended/Monogame-Extended/blob/77135f37427f101de23098240a58f1cb4d61c873/source/MonoGame.Extended/Math/ShapeExtensions.cs

public static class SpriteBatchExtensions {

    private static readonly Vector2 ORIGIN = new(0f, 0.5f);
    private static Texture2D _texture;

    private static Texture2D GetTexture(SpriteBatch spriteBatch) {
        if (_texture == null) {
            _texture = new(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData([Color.White]);
        }
        return _texture;
    }

    public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f, float layerDepth = 0f) {
        float distance = Vector2.Distance(point1, point2);
        float angle = MathF.Atan2(point2.Y - point1.Y, point2.X - point1.X);
        DrawLine(spriteBatch, point1, distance, angle, color, thickness, layerDepth);
    }

    public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f, float layerDepth = 0f) {
        Vector2 scale = new(length, thickness);
        spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, ORIGIN, scale, SpriteEffects.None, layerDepth);
    }

    public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color, float thickness = 1f, float layerDepth = 0f) {
        Texture2D texture = GetTexture(spriteBatch);
        Vector2 topLeft = new(rectangle.X, rectangle.Y);
        Vector2 topRight = new(rectangle.Right - thickness, rectangle.Y);
        Vector2 bottomLeft = new(rectangle.X, rectangle.Bottom - thickness);
        Vector2 horizontalScale = new(rectangle.Width, thickness);
        Vector2 verticalScale = new(thickness, rectangle.Height);

        spriteBatch.Draw(texture, topLeft, null, color, 0f, Vector2.Zero, horizontalScale, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(texture, topLeft, null, color, 0f, Vector2.Zero, verticalScale, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(texture, topRight, null, color, 0f, Vector2.Zero, verticalScale, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(texture, bottomLeft, null, color, 0f, Vector2.Zero, horizontalScale, SpriteEffects.None, layerDepth);
    }

    public static void DrawPolygon(this SpriteBatch spriteBatch, Vector2 position, Polygon polygon, Color color, float thickness = 1f, float layerDepth = 0f) {
        DrawPolygon(spriteBatch, position, polygon.Vertices, color, thickness, layerDepth);
    }

    public static void DrawPolygon(this SpriteBatch spriteBatch, Vector2 offset, IReadOnlyList<Vector2> points, Color color, float thickness = 1f, float layerDepth = 0f) {
        if (points.Count == 0) return;

        if (points.Count == 1) {
            DrawPoint(spriteBatch, points[0], color, (int)thickness);
            return;
        }

        for (int i = 0; i < points.Count - 1; i++)
            DrawLine(spriteBatch, points[i] + offset, points[i + 1] + offset, color, thickness, layerDepth);

        DrawLine(spriteBatch, points[points.Count - 1] + offset, points[0] + offset, color, thickness, layerDepth);
    }

    public static void DrawPoint(this SpriteBatch spriteBatch, float x, float y, Color color, float size = 1f, float layerDepth = 0f) {
        DrawPoint(spriteBatch, new Vector2(x, y), color, size, layerDepth);
    }

    public static void DrawPoint(this SpriteBatch spriteBatch, Vector2 position, Color color, float size = 1f, float layerDepth = 0f) {
        Vector2 scale = Vector2.One * size;
        Vector2 offset = new Vector2(0.5f) - new Vector2(size * 0.5f);
        spriteBatch.Draw(GetTexture(spriteBatch), position + offset, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
    }

    public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color, float thickness = 1f, float layerDepth = 0f) {
        DrawPolygon(spriteBatch, center, CreateCircle(radius, sides), color, thickness, layerDepth);
    }

    public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, float radius, int sides, Color color, float thickness = 1f, float layerDepth = 0f) {
        DrawPolygon(spriteBatch, new Vector2(x, y), CreateCircle(radius, sides), color, thickness, layerDepth);
    }

    public static void DrawEllipse(this SpriteBatch spriteBatch, Vector2 center, Vector2 radius, int sides, Color color, float thickness = 1f, float layerDepth = 0f) {
        DrawPolygon(spriteBatch, center, CreateEllipse(radius.X, radius.Y, sides), color, thickness, layerDepth);
    }

    private static Vector2[] CreateCircle(double radius, int sides) {

        Vector2[] points = new Vector2[sides];
        double step = Math.Tau / sides;
        double theta = 0.0;

        for (int i = 0; i < sides; i++) {
            points[i] = new((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)));
            theta += step;
        }

        return points;

    }

    public static Vector2[] CreateEllipse(float rx, float ry, int sides) {
        Vector2[] vertices = new Vector2[sides];

        double t = 0.0;
        double dt = Math.Tau / sides;
        for (int i = 0; i < sides; i++, t += dt) {
            float x = (float)(rx * Math.Cos(t));
            float y = (float)(ry * Math.Sin(t));
            vertices[i] = new(x, y);
        }

        return vertices;
    }

}

