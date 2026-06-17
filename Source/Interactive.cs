namespace Logger;

public static partial class Log
{
    static async Task<string?> ReadLineAsync(CancellationToken cancellationToken)
    {
        using StreamReader reader = new(Console.OpenStandardInput());
        var res = await reader.ReadLineAsync(cancellationToken);
        return res;
    }

    public static bool AskYesNo(string question, bool? defaultValue = null)
    {
        Console.WriteLine();

        do
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(":: ");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write(question);

            Console.Write(" [");
            Console.Write(defaultValue.HasValue ? defaultValue.Value ? "Y/n" : "y/N" : "y/n");
            Console.Write("]");

            Console.Write(" > ");
            string? input = Console.ReadLine()?.ToLowerInvariant().Trim();

            if (input is "y" or "ye" or "yes") return true;
            if (input is "n" or "no" or "nu" or "nuh") return false;

            if (input is not null && input.Length == 0 && defaultValue.HasValue) return defaultValue.Value;
        }
        while (true);
    }

    public static async Task<bool> AskYesNoAsync(string question, bool? defaultValue = null, CancellationToken cancellationToken = default)
    {
        Console.WriteLine();

        do
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(":: ");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write(question);

            Console.Write(" [");
            Console.Write(defaultValue.HasValue ? defaultValue.Value ? "Y/n" : "y/N" : "y/n");
            Console.Write("]");

            Console.Write(" > ");
            string? input = (await ReadLineAsync(cancellationToken))?.ToLowerInvariant().Trim();

            if (input is "y" or "ye" or "yes") return true;
            if (input is "n" or "no" or "nu" or "nuh") return false;

            if (input is not null && input.Length == 0 && defaultValue.HasValue) return defaultValue.Value;
        }
        while (true);
    }

    public static string AskInput(string question, string? defaultValue = null)
    {
        Console.WriteLine();

        do
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(":: ");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write(question);

            if (defaultValue is not null)
            {
                Console.Write(" [");
                Console.Write(defaultValue);
                Console.Write("]");
            }

            Console.Write(" > ");
            string? input = Console.ReadLine()?.ToLowerInvariant();
            if (!string.IsNullOrEmpty(input)) return input;
            if (defaultValue is not null) return defaultValue;
        }
        while (true);
    }

    public static async Task<string> AskInputAsync(string question, string? defaultValue = null, CancellationToken cancellationToken = default)
    {
        Console.WriteLine();

        do
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(":: ");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write(question);

            if (defaultValue is not null)
            {
                Console.Write(" [");
                Console.Write(defaultValue);
                Console.Write("]");
            }

            Console.Write(" > ");
            string? input = (await ReadLineAsync(cancellationToken))?.ToLowerInvariant();
            if (!string.IsNullOrEmpty(input)) return input;
            if (defaultValue is not null) return defaultValue;
        }
        while (true);
    }

    public delegate bool InputValidator(string input);

    public static string AskInput(string question, InputValidator validator, string? defaultValue = null)
    {
        string result;
        do
        {
            result = AskInput(question, defaultValue);
        } while (!validator.Invoke(result));
        return result;
    }

    public static async Task<string> AskInputAsync(string question, InputValidator validator, string? defaultValue = null, CancellationToken cancellationToken = default)
    {
        string result;
        do
        {
            result = await AskInputAsync(question, defaultValue, cancellationToken);
        } while (!validator.Invoke(result));
        return result;
    }

    public delegate bool InputParser<T>(string input, out T result);

    public static bool IntParser(string input, out int result)
    {
        if (int.TryParse(input, out result)) return true;
        Error($"Invalid input");
        return false;
    }

    public static T AskInput<T>(string question, InputParser<T> parser)
    {
        T result;
        string input;
        do
        {
            input = AskInput(question, null);
        } while (!parser.Invoke(input, out result));
        return result;
    }

    public static async Task<T> AskInputAsync<T>(string question, InputParser<T> parser, CancellationToken cancellationToken = default)
    {
        T result;
        string input;
        do
        {
            input = await AskInputAsync(question, null, cancellationToken);
        } while (!parser.Invoke(input, out result));
        return result;
    }
}
