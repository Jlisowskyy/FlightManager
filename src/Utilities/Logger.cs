namespace proj.Utilities;

public static class Logger
{
// ------------------------------
// Class creation
// ------------------------------

    static Logger()
    {
        Directory.CreateDirectory(DefaultLogDir);
        SetupLogDir(DefaultLogDir);
    }

// ------------------------------
// Class interaction
// ------------------------------

    public static bool IsWorking => _stream != null;

    public static bool SetupLogDir(string dirpath)
    {
        string filename = string.Empty;
        try
        {
            int ind = 0;
            
            do
            {
                filename = $"{dirpath}log_{DateTime.Now:d_M_y}{(ind == 0 ? "" : $"_{ind}")}.txt";
                ++ind;
            } while (File.Exists(filename));

            _stream = new StreamWriter(filename);
        }
        catch (Exception exc)
        {
            _stream = null;
            Console.Error.WriteLine($"[ ERROR ] Log with path: {filename} failed to initialize with error: {exc.Message}...");

            return false;
        }

        Console.WriteLine($"Log with path: {filename} successfully opened...");
        return true;
    }

    public static async Task Log(string msg)
    {
        if (_stream == null) return;
        var refinedMsg = $"{DateTime.Now:hh:mm} | {msg}";
        if (LogToStdOut) Console.WriteLine(refinedMsg);
        await _stream.WriteLineAsync(refinedMsg);
    }

    public static async Task LogError(string msg)
        => Log($"[ ERROR ] {msg}");

    public static void Sync()
        => _stream?.Flush();

// ------------------------------
// Private class methods
// ------------------------------

// ------------------------------
// Class fields
// ------------------------------

    public static bool LogToStdOut { get; set; }

    private const string DefaultLogDir = "logs/";
    private static StreamWriter? _stream;
}