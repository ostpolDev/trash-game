using System;

namespace Engine.Events;

internal class LoggerEventArgs(string content) : EventArgs {

    public string Content = content;

}
