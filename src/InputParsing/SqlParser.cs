using proj.SqlCommands;
using proj.Storage;
using proj.Utilities;

namespace proj.InputParsing;

public class SqlParser
{
// ------------------------------
// Class creation
// ------------------------------

    public SqlParser(ObjectsDB db)
    {
        _db = db;
    }

// ------------------------------
// Class interaction
// ------------------------------

    public async void Parse(string line)
    {
        foreach (var specialCharacter in SpecialCharacters)
            line = line.Replace(specialCharacter, " " + specialCharacter + " ");
        var tokens = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        
        if (tokens.Length == 0)
            return;

        if (Commands.TryGetValue(tokens[0], out var command))
        {
            try
            {
                _executeCommand(tokens, command());
            }
            catch (Exception e)
            {
                await Logger.LogError(e.Message);
            }
        }
        else
            await Logger.LogError($"Unknown command: {tokens[0]}");
    }

    public static bool IsSpecial(string token)
        => token.Length == 1 && SpecialCharacters.Contains(token);

// ------------------------------
// Private class methods
// ------------------------------

    private void _executeCommand(string[] tokens, SqlCommand command)
    {
        var parser = command.GetParser();
        
        parser.Parse(tokens);
        command.Build();

        lock (_db)
            command.Execute(_db);
    }

// ------------------------------
// Class fields
// ------------------------------

    private readonly ObjectsDB _db;
    
    public static Dictionary<string, Func<SqlCommand>> Commands { get; } = new()
    {
        {"display", () => new DisplayCommand()},
        {"delete", () => new DeleteCommand()},
        {"add", () => new AddCommand()},
        {"update", () => new UpdateCommand()}
    };

    public static List<string> SpecialCharacters { get; } =
    [
        ",",
        "=",
        "(",
        ")",
        "<",
        ">",
        "!"
    ];
}