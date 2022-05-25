using AssemblerInterpreter.Attributes;
using AssemblerInterpreter.Common;
using AssemblerInterpreter.Processors;
using System.Reflection;
using System.Text;

namespace AssemblerInterpreter.Commands
{
  public class X86Commands : ICommands<IProcessor<int>, int>
  {
    public Dictionary<string, Action<string[], IProcessor<int>>> Items { get; } = new();
    public List<string> RequireAddress { get; } = new();

    public X86Commands()
    {
      FillSupportedOperations();
      FillRequireAddressOperations();
    }

    private void FillSupportedOperations()
    {
      var supportedOperations = typeof(X86Commands)
        .GetMethods()
        .Where(m => m.IsDefined(typeof(ProcessorSupportedOperationAttribute), inherit: true));

      foreach (var m in supportedOperations)
      {
        Items[m.Name.ToLower()] = DelegateBuilder.BuildDelegate<X86Commands, Action<string[], IProcessor<int>>>(this, m);
      }
    }

    private void FillRequireAddressOperations()
    {
      var supportedOperations = typeof(X86Commands)
        .GetMethods()
        .Where(m => m.GetCustomAttribute(typeof(AddressParameterRequiredCompilerHintAttribute)) != null);

      foreach (var m in supportedOperations)
      {
        RequireAddress.Add(m.Name.ToLower());
      }
    }

    private int ReadValue(string unknown, Dictionary<string, int> registers)
    {
      if (int.TryParse(unknown, out int value))
        return value;

      return registers[unknown];
    }

    [ProcessorSupportedOperation]
    public void Mov(string[] @params, IProcessor<int> processor)
    {
      var rx = @params[0];
      int valueY = ReadValue(@params[1], processor.Registers);

      processor.Registers[rx] = valueY;
    }

    [ProcessorSupportedOperation]
    public void Inc(string[] @params, IProcessor<int> processor)
    {
      var rx = @params[0];

      ++processor.Registers[rx];
    }

    [ProcessorSupportedOperation]
    public void Dec(string[] @params, IProcessor<int> processor)
    {
      var rx = @params[0];

      --processor.Registers[rx];
    }

    [ProcessorSupportedOperation]
    public void Jnz(string[] @params, IProcessor<int> processor)
    {
      var valueX = ReadValue(@params[0], processor.Registers);

      if (valueX != 0)
      {
        var offset = int.Parse(@params[1]);

        processor.RelativeJump(offset);
      }
    }

    [ProcessorSupportedOperation]
    public void Add(string[] @params, IProcessor<int> processor)
    {
      var rx = @params[0];

      processor.Registers[rx] += ReadValue(@params[1], processor.Registers);
    }

    [ProcessorSupportedOperation]
    public void Sub(string[] @params, IProcessor<int> processor)
    {
      var rx = @params[0];

      processor.Registers[rx] -= ReadValue(@params[1], processor.Registers);
    }

    [ProcessorSupportedOperation]
    public void Mul(string[] @params, IProcessor<int> processor)
    {
      var rx = @params[0];

      processor.Registers[rx] *= ReadValue(@params[1], processor.Registers);
    }

    [ProcessorSupportedOperation]
    public void Div(string[] @params, IProcessor<int> processor)
    {
      var rx = @params[0];

      processor.Registers[rx] /= ReadValue(@params[1], processor.Registers);
    }

    [ProcessorSupportedOperation]
    [AddressParameterRequiredCompilerHint]
    public void Jmp(string[] @params, IProcessor<int> processor)
    {
      var address = ReadValue(@params[0], processor.Registers);

      processor.GoTo(address);
    }

    [ProcessorSupportedOperation]
    public void Cmp(string[] @params, IProcessor<int> processor)
    {
      var valueX = ReadValue(@params[0], processor.Registers);
      var valueY = ReadValue(@params[1], processor.Registers);

      processor.LastCompareResult = valueX.CompareTo(valueY);
    }

    [ProcessorSupportedOperation]
    [AddressParameterRequiredCompilerHint]
    public void Jne(string[] @params, IProcessor<int> processor)
    {
      if (processor.LastCompareResult != 0)
        Jmp(@params, processor);
    }

    [ProcessorSupportedOperation]
    [AddressParameterRequiredCompilerHint]
    public void Je(string[] @params, IProcessor<int> processor)
    {
      if (processor.LastCompareResult == 0)
        Jmp(@params, processor);
    }

    [ProcessorSupportedOperation]
    [AddressParameterRequiredCompilerHint]
    public void Jge(string[] @params, IProcessor<int> processor)
    {
      if (processor.LastCompareResult >= 0)
        Jmp(@params, processor);
    }

    [ProcessorSupportedOperation]
    [AddressParameterRequiredCompilerHint]
    public void Jg(string[] @params, IProcessor<int> processor)
    {
      if (processor.LastCompareResult > 0)
        Jmp(@params, processor);
    }

    [ProcessorSupportedOperation]
    [AddressParameterRequiredCompilerHint]
    public void Jle(string[] @params, IProcessor<int> processor)
    {
      if (processor.LastCompareResult <= 0)
        Jmp(@params, processor);
    }

    [ProcessorSupportedOperation]
    [AddressParameterRequiredCompilerHint]
    public void Jl(string[] @params, IProcessor<int> processor)
    {
      if (processor.LastCompareResult < 0)
        Jmp(@params, processor);
    }

    [ProcessorSupportedOperation]
    [AddressParameterRequiredCompilerHint]
    public void Call(string[] @params, IProcessor<int> processor)
    {
      processor.ReturnAddresses.Push(processor.CurrentAddress + 1);

      Jmp(@params, processor);
    }

    [ProcessorSupportedOperation]
    public void Ret(string[] _, IProcessor<int> processor)
      => processor.GoTo(processor.ReturnAddresses.Pop());

    [ProcessorSupportedOperation]
    public void Msg(string[] @params, IProcessor<int> processor)
    {
      StringBuilder message = new();

      foreach (string p in @params.Select(s => s.Trim()))
      {
        if (p.StartsWith("'"))
        {
          if (p.EndsWith("'") && p.Length > 2)
          {
            message.Append(p.Substring(1, p.Length - 2));
          }
        }
        else
        {
          var value = processor.Registers[p.ToLower()];

          message.Append(value);
        }
      }

      processor.Data = message.ToString();
    }

    [ProcessorSupportedOperation]
    public void End(string[] _, IProcessor<int> processor)
      => throw new OperationCanceledException();
  }
}
