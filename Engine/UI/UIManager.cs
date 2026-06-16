using Engine.Debugging;
using Engine.Serialization;
using Engine.UI.Components;
using Engine.UI.Interaction;
using Engine.Utility.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.UI;

public class UIManager : Component, ISerializable {

    private static readonly Logger Logger = Logger.Get("UI");

    public readonly List<AbstractUIComponent> Components = [];
    private readonly List<ITickableUIComponent> TickableComponents = [];
    private readonly List<MouseEventWrapper> MouseEventListeners = [];
    private static readonly Dictionary<string, Type> uiComponentTypeLookup = [];

    private readonly RasterizerState RasterizerState;

    public Rectangle ViewportRectangle { get; private set; }

    public int ChildCount { get { return Components.Count; } }
    public static int RegisteredComponents { get { return uiComponentTypeLookup.Count; } }

    private int ScissorDepth = -1;

    public MouseState CurrentMouseState { get; private set; }
    public MouseState PreviousMouseState { get; private set; }

#if DEBUG
    public bool Debug_DrawScissorTest = false;
    public bool Debug_DrawBounds = false;
    public bool Debug_DrawLocalBounds = false;
    public bool Debug_DisableMouseEventListeners = false;
#endif

    public static UIManager Singleton { get; private set; }

    public UIManager() {
        Singleton = this;
        BaseGame.Instance.OnWindowResize += Instance_OnWindowResize;
        RasterizerState = new() {
            ScissorTestEnable = true,
            DepthBias = 0
        };

        if (BaseGame.Instance.GraphicsDevice != null) {
            ViewportRectangle = new() {
                X = 0,
                Y = 0,
                Width = BaseGame.Instance.GraphicsDevice.Viewport.Width,
                Height = BaseGame.Instance.GraphicsDevice.Viewport.Height
            };
        }
    }

    private void Instance_OnWindowResize(object sender, Events.WindowResizeEventArgs e) {
        TriggerResize(e.Viewport);
    }

    private void TriggerResize(Viewport viewport) {
        ViewportRectangle = new(0, 0, viewport.Width, viewport.Height);
        foreach (AbstractUIComponent component in Components) {
            component.RecalculateScreenPosition(false);
        }
    }

    public void AddComponent(AbstractUIComponent component, bool withChildren = true) {
        Components.Add(component);
        if (component is ITickableUIComponent tickable)
            TickableComponents.Add(tickable);

        if (component is IMouseEventListener mouseEventListener)
            MouseEventListeners.Add(new MouseEventWrapper(component, mouseEventListener));

        if (withChildren)
            foreach (AbstractUIComponent child in component.Children)
                AddComponent(child, true);

        RecalculateAllDepth();
    }

    public void RemoveComponent(AbstractUIComponent component, bool withChildren = true) {
        List<AbstractUIComponent> toDelete = component.Delete(withChildren);
        foreach (AbstractUIComponent item in toDelete) {
            if (item is ITickableUIComponent tickable)
                TickableComponents.Remove(tickable);
            if (item is IMouseEventListener mouseEventListener) {
                MouseEventWrapper wrapper = MouseEventListeners.FirstOrDefault(x => x.Component.UID == component.UID);
                if (wrapper != null) {
                    MouseEventListeners.Remove(wrapper);
                }
            }
            Components.Remove(item);
        }

        RecalculateAllDepth();
    }

    public void RecalculateAllDepth() {
        int i = 0;
        foreach (AbstractUIComponent comp in Components) {
            if (comp.Parent == null) {
                comp.Index = i;
                comp.UpdateZIndex();
                comp.UpdateDepth(0, true);
                i++;
            }
        }

        SortComponentZ();
    }

    public void SortComponentZ() {
        Components.Sort();
    }

    public override void Update(GameTime gameTime, float delta) {
        if (MouseEventListeners.Count <= 0) return;
#if DEBUG
        if (Debug_DisableMouseEventListeners) return;
#endif

        CurrentMouseState = Mouse.GetState();

        if (CurrentMouseState != PreviousMouseState) {
            foreach (var item in MouseEventListeners) {
                item.Update(CurrentMouseState, PreviousMouseState);
            }
        }

        PreviousMouseState = CurrentMouseState;
    }

    public bool IsPointOverUI(Point mousePosition) {
        foreach (AbstractUIComponent component in Components) {
            if (component.Contains(mousePosition)) return true;
        }
        return false;
    }

    public bool IsMouseOverUI() {
        return IsPointOverUI(CurrentMouseState.Position);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {
        spriteBatch.Begin(rasterizerState: RasterizerState, sortMode: SpriteSortMode.Immediate);
        foreach (AbstractUIComponent component in Components) {
            if (component.ShouldDraw) {
                if (component.Depth <= ScissorDepth) {
                    ScissorDepth = -1;
                    spriteBatch.GraphicsDevice.ScissorRectangle = ViewportRectangle;
                }
                component.Draw(gameTime, spriteBatch, alpha);
                if (component.EnableScissor) {
                    spriteBatch.GraphicsDevice.ScissorRectangle = component.ScissorScreenRectangle;
                    ScissorDepth = component.Depth;
                }
#if DEBUG
                if (Debug_DrawScissorTest && ScissorDepth != -1) {
                    Rectangle rect = spriteBatch.GraphicsDevice.ScissorRectangle;
                    spriteBatch.GraphicsDevice.ScissorRectangle = ViewportRectangle;
                    spriteBatch.DrawRectangle(component.ScissorScreenRectangle, Color.Red);
                    if (Debug_DrawBounds) {
                        spriteBatch.DrawRectangle(component.ScreenArea, Color.Green, 2, 1);
                    }
                    if (Debug_DrawLocalBounds) {
                        spriteBatch.DrawRectangle(component.LocalArea, Color.Blue, 2, 1);
                    }
                    spriteBatch.GraphicsDevice.ScissorRectangle = rect;
                } else if (Debug_DrawBounds || Debug_DrawLocalBounds) {
                    if (Debug_DrawLocalBounds) {
                        spriteBatch.DrawRectangle(component.LocalArea, Color.Blue, 2, 1);
                    }
                    if (Debug_DrawBounds) {
                        spriteBatch.DrawRectangle(component.ScreenArea, Color.Green, 2, 1);
                    }
                }
#endif
            }
        }
        ScissorDepth = -1;
        spriteBatch.GraphicsDevice.ScissorRectangle = ViewportRectangle;
        spriteBatch.End();
    }

    public override void FixedUpdate() {
        foreach (ITickableUIComponent tickable in TickableComponents)
            tickable.Tick();
    }

    public void ReadData(SerializableDictionary dictionary) {
        foreach (string key in dictionary.GetKeys()) {
            if (dictionary.GetEntryTypeFor(key) == DictionaryEntryType.DICTIONARY) {
                SerializableDictionary dict = dictionary.GetSerializableDictionary(key);
                string type = dict.GetString("type");
                if (type == null) {
                    Logger.Error($"Failed to load UI component. Type is null for entry {key}");
                    continue;
                }

                AbstractUIComponent component = CreateComponentFromType(type);
                if (component == null) {
                    Logger.Error($"Failed to load UI component. Could not create");
                    continue;
                }
                component.ReadData(dict);
                component.SetID(key);
                AddComponent(component);
            }
        }
        TriggerResize(BaseGame.Instance.GraphicsDevice.Viewport);
    }

    public void WriteData(SerializableDictionary dictionary) {
        foreach (AbstractUIComponent component in Components) {
            SerializableDictionary dict = new();
            component.WriteData(dict);
            dictionary.Put(component.UID, dict);
        }
    }

    public void ClearAll() {
        TickableComponents.Clear();
        MouseEventListeners.Clear();
        Components.Clear();
    }

    public static AbstractUIComponent CreateComponentFromType(Type type, params object[] args) {
        if (!type.IsAssignableTo(typeof(AbstractUIComponent))) {
            Logger.Error($"Failed to create UI component from type. Type {type.Name} is not assignable to {nameof(AbstractUIComponent)}");
            return null;
        }
        try {

            AbstractUIComponent component = (AbstractUIComponent)Activator.CreateInstance(type, args);
            return component;
        } catch (MissingMethodException e) {
            Logger.Exception(e, $"Empty constructor for type {type.Name} is probably missing!");
            return null;
        } catch (Exception e) {
            Logger.Exception(e);
            return null;
        }
    }

    public static AbstractUIComponent CreateComponentFromType(string typeName, params object[] args) {
        if (!uiComponentTypeLookup.TryGetValue(typeName, out Type uiType)) {
            Logger.Error($"Failed to create UI component from type name. Type not found: {typeName}");
            return null;
        }

        return CreateComponentFromType(uiType, args);
    }

    public static void RegisterUIComponent(Type type) {
        if (type == null) return;
        if (type.IsAbstract) {
            Logger.Error($"Failed to register UI component. Type {type.Name} is abstract and cannot be created.");
        }
        if (!type.IsAssignableTo(typeof(AbstractUIComponent))) {
            Logger.Error($"Failed to register UI component. Type {type.Name} is not assignable to {nameof(AbstractUIComponent)}");
            return;
        }

        uiComponentTypeLookup[type.Name] = type;
    }

    public AbstractUIComponent FindByID(string id) {
        return Components.FirstOrDefault(x => x.UID == id);
    }

    public Rectangle GetUIRectangle() {
        return ViewportRectangle;
    }

    static UIManager() {
        RegisterUIComponent(typeof(SimpleUIComponent));
        RegisterUIComponent(typeof(EmptyUIComponent));
        RegisterUIComponent(typeof(TextUIComponent));
    }

}
