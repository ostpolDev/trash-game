using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Engine.Utility;

/// <summary>
/// A class to collect hardware information to be used
/// in the event of a crash.
/// </summary>
public class HardwareSniffer {

    public static string[] Information { get; private set; }
    public static bool IsInitialized { get; private set; } = false;

    /// <summary>
    /// Call to collect hardware information. Call in the game's Init method.
    /// </summary>
    /// <param name="graphicsDevice"></param>
    public static void Initialize(GraphicsDevice graphicsDevice) {
        List<string> items = [
            $"OS: {RuntimeInformation.OSDescription} / {RuntimeInformation.OSArchitecture}",
            $"Framework: {RuntimeInformation.FrameworkDescription}",
            $"CPU Cores: {Environment.ProcessorCount}"
        ];

        long memUsage = GC.GetTotalMemory(false) / 1024 / 1024;
        items.Add($"App memory usage: {memUsage} MB");

        if (graphicsDevice != null) {
            items.Add($"GPU: {graphicsDevice.Adapter.Description}");
            items.Add($"Display Mode: {graphicsDevice.Adapter.CurrentDisplayMode.Width}x{graphicsDevice.Adapter.CurrentDisplayMode.Height} ({graphicsDevice.Adapter.CurrentDisplayMode.Format})");
            items.Add($"Graphics Profile: {graphicsDevice.GraphicsProfile}");
            items.Add($"Last render metrics:");
            items.Add($"   - {graphicsDevice.Metrics.DrawCount} draws");
            items.Add($"   - {graphicsDevice.Metrics.ClearCount} clears");
            items.Add($"   - {graphicsDevice.Metrics.SpriteCount} sprites");
            items.Add($"   - {graphicsDevice.Metrics.PixelShaderCount} pixel shaders");
            items.Add($"   - {graphicsDevice.Metrics.VertexShaderCount} vertex shaders");
            items.Add($"Viewport: {graphicsDevice.Viewport.Width}x{graphicsDevice.Viewport.Height} ({graphicsDevice.Viewport.AspectRatio})");
        }

        Information = [.. items];
        IsInitialized = true;
    }

}
