using AssemblerInterpreter.Instructions;

namespace AssemblerInterpreter.Processors
{
  public interface IProcessor<TRegister> where TRegister : struct
  {
    Dictionary<string, TRegister> Registers { get; }
    Stack<TRegister> ReturnAddresses { get; }
    IInstructions<IProcessor<TRegister>, TRegister> Supported { get; }

    TRegister CurrentAddress { get; }
    string? Data { get; set; }
    TRegister LastCompareResult { get; set; }

    void GoTo(TRegister address);
    void RelativeJump(TRegister offset);
    IProcessor<TRegister> Run(Instruction[] program);
  }
}