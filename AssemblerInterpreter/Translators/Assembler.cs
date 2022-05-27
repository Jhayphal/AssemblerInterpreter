using AssemblerInterpreter.Instructions;
using AssemblerInterpreter.Processors;

namespace AssemblerInterpreter.Translators
{
  internal sealed class Assembler<TRegister>
    where TRegister : struct
  {
    private readonly IProcessor<TRegister> processor;

    public Assembler(IProcessor<TRegister> processor)
    {
      this.processor = processor;
    }

    public IEnumerable<Instruction> Translate(IEnumerable<Instruction> commands)
    {
      var program = new List<Instruction>(commands);
      
      return program.FindIndex(x => x.Label) < 0
        ? program
        : ReplaceLabelsWithAddresses(program);
    }

    private IEnumerable<Instruction> ReplaceLabelsWithAddresses(List<Instruction> instructions)
    {
      var operationsRequireAddress = processor.Supported.RequireAddress;

      var requireAddressIndexes = instructions
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

      while ((labelIndex = instructions.FindIndex(labelIndex, x => x.Label)) >= 0)
      {
        var labelName = instructions[labelIndex].Name[..^1];

        for (int i = requireAddressIndexes.Count - 1; i >= 0; --i)
        {
          var current = instructions[requireAddressIndexes[i]];

          if (string.Equals(current.Parameters[0], labelName, StringComparison.CurrentCultureIgnoreCase))
          {
            current.Parameters[0] = labelIndex.ToString();

            requireAddressIndexes.RemoveAt(i);
          }
        }

        instructions.RemoveAt(labelIndex);

        for (int i = 0; i < requireAddressIndexes.Count; ++i)
        {
          if (requireAddressIndexes[i] >= labelIndex)
          {
            --requireAddressIndexes[i];
          }
        }
      }

      return instructions;
    }
  }
}
