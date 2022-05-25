namespace AssemblerInterpreter.Commands
{
  public sealed class Command
  {
    public readonly string Name;
    public readonly string[] Parameters;

    public bool Label
      => Name.LastOrDefault() == ':';

    public Command(string command, string[] parameters)
    {
      Name = command;
      Parameters = parameters;
    }

    public static Command? Create(IEnumerable<string> info)
      => info.Any()
      ? new Command(
        command: info.First(),
        parameters: info.Skip(1).ToArray())
      : null;

    public override string ToString()
      => $"{Name} {string.Join(" ", Parameters)}".TrimEnd();
  }
}
