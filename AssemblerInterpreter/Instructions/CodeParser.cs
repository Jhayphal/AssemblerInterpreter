using System.Text;

namespace AssemblerInterpreter.Instructions
{
  internal static class CodeParser
  {
    public static List<Instruction> Parse(string code)
    {
      var lines = ToLines(code);

      return ParseLines(lines);
    }

    private static IEnumerable<string> ToLines(string code)
      => code.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

    private static List<Instruction> ParseLines(IEnumerable<string> codeLines)
    {
      List<Instruction> operations = new();

      foreach (var line in codeLines)
      {
        var operation = ParseInstruction(line);

        if (operation != null)
          operations.Add(operation);
      }

      return operations;
    }

    private static Instruction? ParseInstruction(string line)
    {
      var info = new List<string>();
      var buffer = new StringBuilder();
      var inQuotes = false;

      foreach (var c in line.Trim())
      {
        switch (c)
        {
          case ' ':
            if (info.Count == 0)
            {
              info.Add(buffer.ToString());
              buffer.Clear();
            }
            else if (inQuotes)
            {
              buffer.Append(c);
            }
            break;

          case ':':
            if (info.Count == 0)
            {
              buffer.Append(c);
              info.Add(buffer.ToString());
              return Create(info);
            }
            else if (inQuotes)
            {
              buffer.Append(c);
            }
            break;

          case ',':
            if (inQuotes)
            {
              buffer.Append(c);
            }
            else
            {
              info.Add(buffer.ToString());
              buffer.Clear();
            }
            break;

          case '\'':
            inQuotes = !inQuotes;
            buffer.Append(c);
            break;

          case ';':
            if (!inQuotes)
            {
              if (buffer.Length > 0)
              {
                info.Add(buffer.ToString());
              }

              return Create(info);
            }
            break;

          default:
            buffer.Append(c);
            break;
        }
      }

      if (buffer.Length > 0)
      {
        info.Add(buffer.ToString());
      }

      return Create(info);
    }

    private static Instruction? Create(IEnumerable<string> info)
      => info.Any()
      ? new Instruction(
        name: info.First().Trim().ToLower(),
        parameters: info.Skip(1).Select(s => s.Trim()).ToArray())
      : null;
  }
}
