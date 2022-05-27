using AssemblerInterpreter.Attributes;
using AssemblerInterpreter.Common;
using AssemblerInterpreter.Processors;
using System.Reflection;
using System.Text;

namespace AssemblerInterpreter.Instructions
{
  public class X86Instructions : IInstructions<IProcessor<Int32>, int>
  {
    public Dictionary<string, Action<string[], IProcessor<Int32>>> Instructions { get; } = new();
    public List<string> RequireAddress { get; } = new();

    public X86Instructions()
    {
      FillSupportedOperations();
      FillRequireAddressOperations();
    }

    private static void AddToMessage(StringBuilder message, string parameter, Dictionary<string, int> registers)
    {
      if (parameter.StartsWith("'"))
      {
        if (parameter.EndsWith("'") && parameter.Length > 2)
        {
          message.Append(parameter[1..^1]);
        }
      }
      else
      {
        var value = registers[parameter.ToLower()];

        message.Append(value);
      }
    }

    private static int ReadValue(string unknown, Dictionary<string, int> registers)
    {
      if (int.TryParse(unknown, out int value))
        return value;

      return registers[unknown];
    }

    private void FillSupportedOperations()
    {
      var supportedOperations = typeof(X86Instructions)
        .GetMethods()
        .Where(m => m.IsDefined(typeof(ProcessorSupportedInstructionAttribute), inherit: true));

      foreach (var m in supportedOperations)
      {
        Instructions[m.Name.ToLower()] = DelegateBuilder.BuildDelegate<X86Instructions, Action<string[], IProcessor<Int32>>>(this, m);
      }
    }

    private void FillRequireAddressOperations()
    {
      var supportedOperations = typeof(X86Instructions)
        .GetMethods()
        .Where(m => m.GetCustomAttribute(typeof(RequiredAddressParameterTranslatorHintAttribute)) != null);

      foreach (var m in supportedOperations)
      {
        RequireAddress.Add(m.Name.ToLower());
      }
    }

    [ProcessorSupportedInstruction]
    public void Mov(string[] @params, IProcessor<Int32> processor)
    {
      var rx = @params[0];
      int valueY = ReadValue(@params[1], processor.Registers);

      processor.Registers[rx] = valueY;
    }

    [ProcessorSupportedInstruction]
    public void Inc(string[] @params, IProcessor<Int32> processor)
    {
      var rx = @params[0];

      ++processor.Registers[rx];
    }

    [ProcessorSupportedInstruction]
    public void Dec(string[] @params, IProcessor<Int32> processor)
    {
      var rx = @params[0];

      --processor.Registers[rx];
    }

    [ProcessorSupportedInstruction]
    public void Jnz(string[] @params, IProcessor<Int32> processor)
    {
      var valueX = ReadValue(@params[0], processor.Registers);

      if (valueX != 0)
      {
        var offset = int.Parse(@params[1]);

        processor.RelativeJump(offset);
      }
    }

    [ProcessorSupportedInstruction]
    public void Add(string[] @params, IProcessor<Int32> processor)
    {
      var rx = @params[0];

      processor.Registers[rx] += ReadValue(@params[1], processor.Registers);
    }

    [ProcessorSupportedInstruction]
    public void Sub(string[] @params, IProcessor<Int32> processor)
    {
      var rx = @params[0];

      processor.Registers[rx] -= ReadValue(@params[1], processor.Registers);
    }

    [ProcessorSupportedInstruction]
    public void Mul(string[] @params, IProcessor<Int32> processor)
    {
      var rx = @params[0];

      processor.Registers[rx] *= ReadValue(@params[1], processor.Registers);
    }

    [ProcessorSupportedInstruction]
    public void Div(string[] @params, IProcessor<Int32> processor)
    {
      var rx = @params[0];

      processor.Registers[rx] /= ReadValue(@params[1], processor.Registers);
    }

    [ProcessorSupportedInstruction]
    [RequiredAddressParameterTranslatorHint]
    public void Jmp(string[] @params, IProcessor<Int32> processor)
    {
      var address = ReadValue(@params[0], processor.Registers);

      processor.GoTo(address);
    }

    [ProcessorSupportedInstruction]
    public void Cmp(string[] @params, IProcessor<Int32> processor)
    {
      var valueX = ReadValue(@params[0], processor.Registers);
      var valueY = ReadValue(@params[1], processor.Registers);

      processor.LastCompareResult = valueX.CompareTo(valueY);
    }

    [ProcessorSupportedInstruction]
    [RequiredAddressParameterTranslatorHint]
    public void Jne(string[] @params, IProcessor<Int32> processor)
    {
      if (processor.LastCompareResult != 0)
        Jmp(@params, processor);
    }

    [ProcessorSupportedInstruction]
    [RequiredAddressParameterTranslatorHint]
    public void Je(string[] @params, IProcessor<Int32> processor)
    {
      if (processor.LastCompareResult == 0)
        Jmp(@params, processor);
    }

    [ProcessorSupportedInstruction]
    [RequiredAddressParameterTranslatorHint]
    public void Jge(string[] @params, IProcessor<Int32> processor)
    {
      if (processor.LastCompareResult >= 0)
        Jmp(@params, processor);
    }

    [ProcessorSupportedInstruction]
    [RequiredAddressParameterTranslatorHint]
    public void Jg(string[] @params, IProcessor<Int32> processor)
    {
      if (processor.LastCompareResult > 0)
        Jmp(@params, processor);
    }

    [ProcessorSupportedInstruction]
    [RequiredAddressParameterTranslatorHint]
    public void Jle(string[] @params, IProcessor<Int32> processor)
    {
      if (processor.LastCompareResult <= 0)
        Jmp(@params, processor);
    }

    [ProcessorSupportedInstruction]
    [RequiredAddressParameterTranslatorHint]
    public void Jl(string[] @params, IProcessor<Int32> processor)
    {
      if (processor.LastCompareResult < 0)
        Jmp(@params, processor);
    }

    [ProcessorSupportedInstruction]
    [RequiredAddressParameterTranslatorHint]
    public void Call(string[] @params, IProcessor<Int32> processor)
    {
      processor.ReturnAddresses.Push(processor.CurrentAddress + 1);

      Jmp(@params, processor);
    }

    [ProcessorSupportedInstruction]
    public void Ret(string[] _, IProcessor<Int32> processor)
      => processor.GoTo(processor.ReturnAddresses.Pop());

    [ProcessorSupportedInstruction]
    public void Msg(string[] @params, IProcessor<Int32> processor)
    {
      StringBuilder message = new();

      foreach (string parameter in @params)
      {
        AddToMessage(message, parameter, processor.Registers);
      }

      processor.Data = message.ToString();
    }
    
    [ProcessorSupportedInstruction]
    public void End(string[] _, IProcessor<Int32> processor)
      => throw new OperationCanceledException();
  }
}
