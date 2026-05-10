using Engine.Debugging;
using Engine.Events;
using Engine.Interaction;
using Engine.SceneManagement;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine;

public abstract class BaseGame : Game {

    protected static readonly Logger Logger = Logger.Get("Main");
    public static BaseGame Instance { get; private set; }

    public GraphicsDeviceManager GraphicsDeviceManager { get; protected set; }
    public SpriteBatch SpriteBatch { get; protected set; }

    public bool IsDevelopmentMode { get; private set; }

    private const float FIXED_UPDATE_DELTA = (int)(1000 / 30f);
    private const float MAX_FRAME_TIME = 250f;

    private float previousT = 0f;
    private float accumulator = 0f;
    private float alpha = 0f;
    private float deltaTime = 0f;

    public event EventHandler<WindowResizeEventArgs> OnWindowResize;
    public readonly SceneManager SceneManager;
    public readonly InputManager InputManager = new();
    public readonly DebugMenuManager DebugMenuManager;

    public BaseGame(bool isDevelopmentMode) {
        Instance = this;
        GraphicsDeviceManager = new GraphicsDeviceManager(this);
        SceneManager = new(this);
        DebugMenuManager = new(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        IsDevelopmentMode = isDevelopmentMode;

        Window.ClientSizeChanged += Window_ClientSizeChanged;

        RegisterEngineWindows();

        if (isDevelopmentMode) {
            RegisterDebugMenus();
        }
    }

    private void RegisterEngineWindows() {

    }

    protected virtual void RegisterDebugMenus() { }

    private void Window_ClientSizeChanged(object sender, EventArgs e) {
        OnWindowResize?.Invoke(this, new(GraphicsDevice.Viewport));
        DebugMenuManager.OnResize(GraphicsDevice.Viewport);
    }

    protected override void Initialize() {
        SpriteBatch = new(GraphicsDevice);
        HardwareSniffer.Initialize(GraphicsDevice);
        base.Initialize();
    }

    protected override void LoadContent() {
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime) {
        if (previousT == 0)
            previousT = (float)gameTime.TotalGameTime.TotalMilliseconds;

        float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
        float frameTime = now - previousT;
        if (frameTime > MAX_FRAME_TIME)
            frameTime = MAX_FRAME_TIME;

        previousT = now;
        accumulator += frameTime;

        InputManager.Update();

        while (accumulator >= FIXED_UPDATE_DELTA) {
            // FixedUpdate
            SceneManager.FixedUpdate();
            OnFixedUpdate(gameTime);

            accumulator -= FIXED_UPDATE_DELTA;
        }

        alpha = accumulator / FIXED_UPDATE_DELTA;
        deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        SceneManager.Update(gameTime, deltaTime);
        OnUpdate(gameTime, deltaTime);

        InputManager.LateUpdate();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);
        SceneManager.Draw(gameTime, SpriteBatch, alpha);
        OnDraw(gameTime, alpha);
        DebugMenuManager.Draw(gameTime);
        base.Draw(gameTime);
    }

    protected override void OnExiting(object sender, EventArgs args) {
        Logger.Info("Exiting!");
        Logger.Close();
        base.OnExiting(sender, args);
    }

    public abstract void OnUpdate(GameTime gameTime, float deltaTime);
    public abstract void OnDraw(GameTime gameTime, float alpha);
    public abstract void OnFixedUpdate(GameTime gameTime);
    

}
