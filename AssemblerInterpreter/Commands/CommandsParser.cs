using System.Text;

namespace AssemblerInterpreter.Commands
{
  internal static class CommandsParser
  {
    public static List<Command> Parse(string code)
    {
      var lines = ToLines(code);

      return ParseLines(lines);
    }

    private static List<Command> ParseLines(IEnumerable<string> codeLines)
    {
      List<Command> operations = new();

      foreach (var line in codeLines)
      {
        var operation = ParseOperation(line);

        if (operation != null)
          operations.Add(operation);
      }

      return operations;
    }

    private static Command? ParseOperation(string line)
    {
      var info = new List<string>();
      var buffer = new StringBuilder();
      var inQuotes = false;

      foreach (var c in line)
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
              return Command.Create(info);
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

              return Command.Create(info);
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

      return Command.Create(info);
    }

    private static IEnumerable<string> ToLines(string code)
      => code
        .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim());
  }
}
