using System.Text;

namespace Logger;

public abstract class ProgressBarBase : IDisposable
{
    static readonly TimeSpan AnimationInterval = TimeSpan.FromSeconds(1.0 / 8);

    readonly Timer Timer;
    readonly Lock Lock;

    bool IsDisposed;
    LogEntry LastLine;

    public int MaxWidth;

    public ProgressBarBase()
    {
        Timer = new Timer(TimerHandler);
        Lock = new Lock();
        Log.InteractiveLocks.Add(Lock);
        MaxWidth = int.MaxValue;

        if (Console.IsOutputRedirected) return;

        Log.Keep(LastLine);

        ResetTimer();
    }

    void TimerHandler(object? state)
    {
        lock (Lock)
        {
            if (IsDisposed) return;

            LastLine.Back();
            LastLine = default;

            Render(ref LastLine);

            Log.Rekeep(LastLine);

            ResetTimer();
        }
    }

    protected abstract void Render(ref LogEntry line);

    void ResetTimer() => Timer.Change(AnimationInterval, TimeSpan.FromMilliseconds(-1));

    public void Dispose()
    {
        if (IsDisposed) return;

        lock (Lock)
        {
            IsDisposed = true;
            LastLine.Clear();
            Log.Unkeep();
        }

        Log.InteractiveLocks.Remove(Lock);

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }

    public static string Render(double value, int width, string characters)
    {
        StringBuilder b = new();
        if (characters.Length == 2)
        {
            int fill = (int)(width * value);
            int empty = width - fill;

            b.Append(characters[0], fill);
            b.Append(characters[1], empty);
        }
        else
        {
            string p = characters;

            int filled = (int)(width * (double)value);
            int empty = (int)(width * (1f - (double)value));
            int segment = (int)((double)value * width % 1 * (p.Length - 1));

            b.Append(p[0], filled);
            if (filled + empty < width) b.Append(p[p.Length - 1 - segment]);
            b.Append(p[^1], empty);
        }
        return b.ToString();
    }

    public const string Barialle = "⣿⣷⣧⣇⡇⡆⡄⡀ ";
    public const string Solid = "█▉▊▋▌▍▎▏ ";
    public const string Hashtags = "# ";
}
