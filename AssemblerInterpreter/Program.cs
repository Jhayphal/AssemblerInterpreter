using AssemblerInterpreter.Processors;
using AssemblerInterpreter.Compilers;
using AssemblerInterpreter.Commands;

namespace AssemblerInterpreter
{
  internal class Program
  {
    static void Main()
    {
      IProcessor<int> processor = new X86Processor();
      DefaultCompiler<int> compiler = DefaultCompiler<int>.Create(processor);

      foreach (var code in GetSamples())
      {
        List<Command>? program = CommandsParser.Parse(code);
        Command[] commands = compiler.Compile(program).ToArray();
        processor.Run(commands);

        Console.WriteLine(processor.Data);
      }
    }

    static IEnumerable<string> GetSamples()
    {
      const string SamplesFolderName = @"Samples";

      if (Directory.Exists(SamplesFolderName))
      {
        foreach (var file in Directory.GetFiles(SamplesFolderName))
        {
          yield return File.ReadAllText(file);
        }
      }
    }
  }
}