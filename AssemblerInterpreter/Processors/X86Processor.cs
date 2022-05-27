using AssemblerInterpreter.Instructions;

namespace AssemblerInterpreter.Processors
{
  public sealed class X86Processor : IProcessor<Int32>
  {
    public Dictionary<string, int> Registers { get; } = new();
    public Stack<Int32> ReturnAddresses { get; } = new();
    public IInstructions<IProcessor<Int32>, int> Supported { get; } = new X86Instructions();
    public string? Data { get; set; }
    public int LastCompareResult { get; set; }
    public int CurrentAddress { get; private set; }

    public void RelativeJump(int offset)
    {
      CurrentAddress += offset;

      --CurrentAddress; // for next loop
    }

    public void GoTo(int address)
    {
      CurrentAddress = address;

      --CurrentAddress; // for next loop
    }

    public IProcessor<Int32> Run(Instruction[] program)
    {
      try
      {
        for (; CurrentAddress < program.Length; ++CurrentAddress)
          Run(program[CurrentAddress]);

        Data = null;
      }
      catch (OperationCanceledException)
      {
        // end
      }
      catch
      {
        throw;
      }
      finally
      {
        CurrentAddress = 0;
        LastCompareResult = 0;
        Registers.Clear();
        ReturnAddresses.Clear();
      }

      return this;
    }

    private void Run(Instruction instruction)
    {
      var action = Supported.Instructions[instruction.Name];

      action(instruction.Parameters, this);
    }
  }
}
