namespace Engine.UI;

public interface ITickableUIComponent {

    /// <summary>
    /// Called every FixedUpdate (30 / sec)
    /// </summary>
    public void Tick();

}
