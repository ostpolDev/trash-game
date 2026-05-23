using Engine.Debugging;
using Engine.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Engine.UI;

public class UIManager : Component, ISerializable {

    private static readonly Logger Logger = Logger.Get("UI");

    public readonly List<AbstractUIComponent> Components = [];
    private readonly List<ITickableUIComponent> TickableComponents = [];
    private static readonly Dictionary<string, Type> uiComponentTypeLookup = [];

    public int ChildCount { get { return Components.Count; } }
    public static int RegisteredComponents { get { return uiComponentTypeLookup.Count; } }
    

    public static UIManager Singleton { get; private set; }

    public UIManager() {
        Singleton = this;
        BaseGame.Instance.OnWindowResize += Instance_OnWindowResize;
    }

    private void Instance_OnWindowResize(object sender, Events.WindowResizeEventArgs e) {
        foreach (AbstractUIComponent component in Components) {
            component.RecalculateScreenPosition();
        }
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

    public void RemoveComponent(AbstractUIComponent component, bool withChildren = true) {
        List<AbstractUIComponent> toDelete = component.Delete(withChildren);
        foreach (AbstractUIComponent item in toDelete) {
            if (item is ITickableUIComponent tickable)
                TickableComponents.Remove(tickable);
            Components.Remove(item);
        }

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

    public void LoadData(SerializableDictionary dictionary) {
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
                component.LoadData(dict);
            }
        }
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

        return CreateComponentFromType(uiType);
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

    static UIManager() {
        RegisterUIComponent(typeof(SimpleUIComponent));
    }

}
