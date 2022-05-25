using AssemblerInterpreter.Commands;

namespace AssemblerInterpreter.Processors
{
  public sealed class X86Processor : IProcessor<int>
  {
    public Dictionary<string, int> Registers { get; } = new();
    public Stack<int> ReturnAddresses { get; } = new();
    public ICommands<IProcessor<int>, int> Supported { get; } = new X86Commands();
    public string? Data { get; set; }
    public int LastCompareResult { get; set; }
    public int CurrentAddress => address;

    private int address = 0;

    public void RelativeJump(int offset)
    {
      address += offset;

      --address; // for next loop
    }

    public void GoTo(int address)
    {
      this.address = address;

      --this.address; // for next loop
    }

    public IProcessor<int> Run(Command[] program)
    {
      try
      {
        for (; address < program.Length; ++address)
          Run(program[address]);

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
        address = 0;
        LastCompareResult = 0;
        Registers.Clear();
        ReturnAddresses.Clear();
      }

      return this;
    }

    private void Run(Command operation)
    {
      var action = Supported.Items[operation.Name];

      action(operation.Parameters, this);
    }
  }
}
