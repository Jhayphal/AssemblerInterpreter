namespace AssemblerInterpreter.Instructions
{
  public sealed class Instruction
  {
    public readonly string Name;
    public readonly string[] Parameters;

    public bool Label
      => Name.LastOrDefault() == ':';

    public Instruction(string name, IEnumerable<string> parameters)
    {
      Name = name?.Trim().ToLower()
        ?? throw new ArgumentNullException(nameof(name));

      Parameters = parameters?.Select(s => s.Trim()).ToArray()
        ?? throw new ArgumentNullException(nameof(parameters));
    }

    public override string ToString()
      => $"{Name} {string.Join(" ", Parameters)}".TrimEnd();
  }
}
