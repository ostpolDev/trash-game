using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine;

/// <summary>
/// Base class for anything that receives updates and can draw
/// </summary>
public abstract class Component {

    /// <summary>
    /// Called once every frame
    /// </summary>
    /// <param name="delta">Time in seconds since last frame</param>
    public abstract void Update(GameTime gameTime, float delta);

    /// <summary>
    /// Called once every frame
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    /// <param name="alpha">Time in seconds since last FixedUpdate call</param>
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha);

    /// <summary>
    /// Called 30 times every second
    /// </summary>
    public abstract void FixedUpdate();

}

