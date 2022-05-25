using AssemblerInterpreter.Commands;
using AssemblerInterpreter.Processors;

namespace AssemblerInterpreter.Compilers
{
  internal sealed class DefaultCompiler<TRegister>
    where TRegister : struct
  {
    private readonly IProcessor<TRegister> processor;

    private DefaultCompiler(IProcessor<TRegister> processor)
    {
      this.processor = processor;
    }

    public static DefaultCompiler<TRegister> Create(IProcessor<TRegister> processor)
      => new(processor);

    public IEnumerable<Command> Compile(IEnumerable<Command> operations)
    {
      if (!operations.Any(x => x.Label))
        return operations;

      return RemoveLabels(operations);
    }

    private IEnumerable<Command> RemoveLabels(IEnumerable<Command> commands)
    {
      var operations = commands.ToList();
      var operationsRequireAddress = processor.Supported.RequireAddress;

      var requireAddressIndexes = operations
        .Select((o, i) =>
          new
          {
            Require = operationsRequireAddress.Contains(o.Name),
            Index = i
          })
        .Where(x => x.Require)
        .Select(x => x.Index)
        .ToList();

      int labelIndex = 0;

      while ((labelIndex = operations.FindIndex(labelIndex, x => x.Label)) >= 0)
      {
        var labelName = operations[labelIndex].Name[..^1];

        for (int i = requireAddressIndexes.Count - 1; i >= 0; --i)
        {
          var current = operations[requireAddressIndexes[i]];

          if (string.Equals(current.Parameters[0], labelName, StringComparison.CurrentCultureIgnoreCase))
          {
            current.Parameters[0] = labelIndex.ToString();

            requireAddressIndexes.RemoveAt(i);
          }
        }

        operations.RemoveAt(labelIndex);

        for (int i = 0; i < requireAddressIndexes.Count; ++i)
        {
          if (requireAddressIndexes[i] >= labelIndex)
            --requireAddressIndexes[i];
        }
      }

      return operations;
    }
  }
}
