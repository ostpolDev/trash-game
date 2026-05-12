using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Engine.UI;

public class UIManager : Component {

    public readonly List<AbstractUIComponent> Components = [];
    private readonly List<ITickableUIComponent> TickableComponents = [];

    public int ChildCount { get { return Components.Count; } }
    

    public static UIManager Singleton { get; private set; }

    public UIManager() {
        Singleton = this;
    }

    public void AddComponent(AbstractUIComponent component, bool withChildren = true) {
        Components.Add(component);
        if (component is ITickableUIComponent tickable)
            TickableComponents.Add(tickable);

        if (withChildren)
            foreach (AbstractUIComponent child in component.Children)
                AddComponent(child, true);

        SortComponentDepth();
    }

    public void SortComponentDepth() {
        Components.Sort();
    }

    public override void Update(GameTime gameTime, float delta) {
        
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {
        spriteBatch.Begin();
        foreach (AbstractUIComponent component in Components)
            component.Draw(gameTime, spriteBatch, alpha);
        spriteBatch.End();
    }

    public override void FixedUpdate() {
        foreach (ITickableUIComponent tickable in TickableComponents)
            tickable.Tick();
    }

}
