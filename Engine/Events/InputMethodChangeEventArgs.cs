using Engine.Interaction;
using System;

namespace Engine.Events;

public class InputMethodChangedEventArgs(InputMethod method, ControllerBrand brand = ControllerBrand.NONE) : EventArgs {

    public InputMethod InputMethod = method;
    public ControllerBrand ControllerBrand = brand;

}
