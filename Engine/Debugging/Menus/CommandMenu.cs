using Engine.Utility;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Debugging.Menus;

public class CommandMenu : DebugMenu {

    private static readonly Logger Logger = Logger.Get("Commands");

    private const int MAX_BUFFER_SIZE = 32;

    private static readonly Dictionary<string, CommandRegistryItem> commandLookup = [];
    private static readonly List<string> resultBuffer = [];

    private string cheatInput = "";

    public CommandMenu() : base("command") {
        WindowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDecoration;
    }

    protected override void OnDrawMenu() {
        if (ImGui.InputText("Command", ref cheatInput, 256, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.EscapeClearsAll)) {
            ProcessCheat(cheatInput);
            cheatInput = "";
        }

        ImGui.SameLine();
        if (ImGui.Button("Submit")) {
            ProcessCheat(cheatInput);
            cheatInput = "";
        }

        foreach(string str in resultBuffer) {
            ImGui.Text(str);
        }
    }

    private void ProcessCheat(string cheat) {
        if (string.IsNullOrEmpty(cheat)) return;

        string[] parts = cheat.Split(' ');
        string command = parts[0];
        string[] args = parts[1..];

        if (!commandLookup.TryGetValue(command, out CommandRegistryItem func)) {
            WriteToOutput($"Command \"{command}\" not found");
            return;
        }

        try {
            CommandActionResult result = func.Action.Invoke(args);
            if (!result.WasSuccessful) {
                WriteToOutput(result.ErrorMessage ?? "Something went wrong");
            }
            if (result.CloseOnComplete) {
                IsOpened = false;
            }
        } catch (Exception e) {
            Logger.Exception(e);
            WriteToOutput("An internal error occured");
            WriteToOutput(e.Message ?? "Unknown");
        }

        if (command != "clear") {
            WriteToOutput($"> {cheat}");
        }
    }

    public static void WriteToOutput(string msg) {
        Logger.Info(msg);
        resultBuffer.Insert(0, msg);
        if (resultBuffer.Count > MAX_BUFFER_SIZE) {
            resultBuffer.RemoveRange(MAX_BUFFER_SIZE - 1, resultBuffer.Count - MAX_BUFFER_SIZE);
        }
    }

    public static void WriteToOutput(IEnumerable<string> msgs) {
        foreach (string str in msgs)
            WriteToOutput(str);
    }

    public static void RegisterCommand(string command, CommandRegistryItem commandItem) {
        commandLookup[command] = commandItem;
    }

    protected override void PrepareWindow() {
        ImGui.SetNextWindowPos(new(0, 0), ImGuiCond.Appearing);
        ImGui.SetNextWindowSize(new(DebugMenuManager.WindowViewport.Width, 0), ImGuiCond.Always);
        ImGui.SetNextWindowSizeConstraints(new(DebugMenuManager.WindowViewport.Width, 0), new(DebugMenuManager.WindowViewport.Width, 200));
    }

    static CommandMenu() {
        RegisterCommand("help", new() {
            Action = (args) => {
                if (args != null && args.Length >= 1) {
                    string command = args[0];
                    if (!commandLookup.TryGetValue(command, out CommandRegistryItem item)) {
                        return new() { WasSuccessful = false, ErrorMessage = $"Command \"{command}\" was not found" };
                    }
                    if (item.Help == null || item.Help.Length <= 0) {
                        if (!string.IsNullOrEmpty(item.ShortDescription)) {
                            WriteToOutput(item.ShortDescription);
                        } else {
                            WriteToOutput("No help available");
                        }
                    } else {
                        WriteToOutput(item.Help.Reverse());
                    }
                } else {
                    foreach (KeyValuePair<string, CommandRegistryItem> item in commandLookup) {
                        WriteToOutput($"{item.Key} - {item.Value.ShortDescription ?? "?"}");
                    }
                }
                return CommandActionResult.SUCCESS;
            },
            ShortDescription = "Get help with the commands",
            Help = ["Usage: help <command>", "Will display a list of all commands by default", "  command: (optional) Enter a command to get more information about it"]
        });
        RegisterCommand("clear", new() {
            Action = (_) => {
                resultBuffer.Clear();
                return CommandActionResult.SUCCESS;
            },
            ShortDescription = "Clears the command output"
        });
        RegisterCommand("forcequit", new() {
            Action = (_) => {
                BaseGame.Instance.Exit();
                return CommandActionResult.SUCCESS;
            },
            ShortDescription = "Forces the game to exit"
        });
        RegisterCommand("editors", new() {
            Action = (args) => {
                if (args == null || args.Length <= 0) {
                    return CommandActionResult.MISSING_ARGS;
                }

                string action = args[0];
                if (action == "list") {
                    WriteToOutput(DebugMenuManager.ListEditorScenes());
                    WriteToOutput("Registered editors:");
                    return CommandActionResult.SUCCESS;
                } else if (action == "load") {
                    if (args.Length < 2) {
                        return CommandActionResult.MISSING_ARGS;
                    }
                    DebugMenuManager.SetEditorScene(args[1]);
                    return CommandActionResult.SUCCESS_AND_CLOSE;
                }

                return CommandActionResult.INVALID_ARGS;
            },
            ShortDescription = "Editor scene management",
            Help = ["Usage: editors [action] <args>", "  action:", "    - load [editor_name]", "    - list"]
        });
        RegisterCommand("opendir", new() {
            Action = (args) => {
                if (args == null || args.Length <= 0) {
                    return CommandActionResult.MISSING_ARGS;
                }

                string targetDir = args[0];
                bool success = false;

                switch (targetDir) {
                    case "author":
                        success = PathHelper.OpenDir(PathHelper.GetAuthorDirectory());
                        break;
                    case "game":
                        success = PathHelper.OpenDir(PathHelper.GetAppDirectory());
                        break;
                    case "runtime":
                        success = PathHelper.OpenDir(PathHelper.GetRuntimeContentDirectory());
                        break;
                    case "current":
                        success = PathHelper.OpenDir(PathHelper.GetCurrentDirectory());
                        break;
                    case "mods":
                        success = PathHelper.OpenDir(PathHelper.GetModsDirectory());
                        break;
                    default:
                        return CommandActionResult.INVALID_ARGS;
                }

                return success ? CommandActionResult.SUCCESS : CommandActionResult.FAILED;
            },
            ShortDescription = "Open game directories in file explorer",
            Help = ["Usage: opendir [dir]", "  dir:", "    - author, game, runtime, current, mods"]
        });
    }

    public struct CommandActionResult {
        public static readonly CommandActionResult SUCCESS = new() { WasSuccessful = true };
        public static readonly CommandActionResult FAILED = new() { WasSuccessful = false, ErrorMessage = "Something went wrong" };
        public static readonly CommandActionResult MISSING_ARGS = new() { WasSuccessful = false, ErrorMessage = "Missing arguments" };
        public static readonly CommandActionResult INVALID_ARGS = new() { WasSuccessful = false, ErrorMessage = "Invalid arguments" };
        public static readonly CommandActionResult SUCCESS_AND_CLOSE = new() { WasSuccessful = true, CloseOnComplete = true };

        public bool WasSuccessful;
        public string ErrorMessage;
        public bool CloseOnComplete;
    }

    public struct CommandRegistryItem {
        /// <summary>
        /// The function to run when this command is executed. Receives the arguments as the first parameter
        /// </summary>
        public Func<string[], CommandActionResult> Action;

        /// <summary>
        /// A short description displayed in the command list
        /// </summary>
        public string ShortDescription;

        /// <summary>
        /// An array of lines of text to display when help is requested for this command
        /// </summary>
        public string[] Help;
    }

}
