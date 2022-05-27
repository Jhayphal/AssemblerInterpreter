using AssemblerInterpreter.Processors;
using AssemblerInterpreter.Translators;
using AssemblerInterpreter.Instructions;
using AssemblerInterpreter.Properties;

namespace AssemblerInterpreter
{
  internal class Program
  {
    private static void Main()
    {
      IProcessor<Int32> processor = new X86Processor();
      Assembler<Int32> assembler = new(processor);

      foreach (var sourceCode in GetSamples())
      {
        IEnumerable<Instruction> program = CodeParser.Parse(sourceCode);
        program = assembler.Translate(program);
        processor.Run(program.ToArray());

        Console.WriteLine(processor.Data);
      }
    }

    private static IEnumerable<string> GetSamples()
    {
      string samplesFolderName = Resources.SamplesFolderName;

      if (Directory.Exists(samplesFolderName))
      {
        foreach (var file in Directory.GetFiles(samplesFolderName))
        {
          yield return File.ReadAllText(file);
        }
      }
    }
  }
}