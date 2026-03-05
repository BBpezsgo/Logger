using System.Text;

namespace Logger;

public class ProgressBar : IDisposable, IProgress<float>, IProgress<double>
{
    static readonly TimeSpan AnimationInterval = TimeSpan.FromSeconds(1.0 / 8);

    readonly Timer Timer;
    readonly Lock Lock;

    float Progress;
    string Title = string.Empty;

    bool IsDisposed;
    LogEntry LastLine;

    public int MaxWidth;

    public ProgressBar()
    {
        Timer = new Timer(TimerHandler);
        Lock = new Lock();
        Log.InteractiveLocks.Add(Lock);
        Progress = float.NaN;
        MaxWidth = int.MaxValue;

        if (Console.IsOutputRedirected) return;

        Log.Keep(LastLine);

        ResetTimer();
    }

    public void Report(float value)
    {
        value = Math.Clamp(value, 0f, 1f);

        Interlocked.Exchange(ref Progress, value);
    }

    void IProgress<double>.Report(double value)
    {
        value = Math.Clamp(value, 0f, 1f);

        Interlocked.Exchange(ref Progress, (float)value);
    }

    public void Report(int index, int length) => Report((float)index / (float)length);

    public void Report(string title, float value = float.NaN)
    {
        if (!float.IsNaN(value)) value = Math.Clamp(value, 0f, 1f);

        Interlocked.Exchange(ref Title, title);
        Interlocked.Exchange(ref Progress, value);
    }

    public void Report(string title, int index, int length) => Report(title, (float)index / (float)length);

    void TimerHandler(object? state)
    {
        lock (Lock)
        {
            if (IsDisposed) return;

            string title = Title;

            int width = Math.Min(Console.WindowWidth - 1, MaxWidth);

            if (title.Length >= width / 2)
            {
                title = title[..(width / 2 - 3)] + "...";
            }

            LastLine.Back();
            LastLine = default;

            if (!float.IsNaN(Progress))
            {
                if (string.IsNullOrWhiteSpace(Title))
                {
                    int w = width;
                    if (w >= 2)
                    {
                        int fill = (int)(w * Progress);
                        int empty = w - fill;

                        StringBuilder b = new();
                        b.Append('[');
                        b.Append('#', fill);
                        b.Append(' ', empty);
                        b.Append(']');

                        LastLine += Log.Write(b.ToString());
                    }
                }
                else
                {
                    LastLine += Log.Write(title);
                    LastLine += Log.Write(new string(' ', Math.Max(0, width / 2 - title.Length)));
                    int w = width - Console.CursorLeft - 1;
                    if (w >= 2)
                    {
                        int fill = (int)(w * Progress);
                        int empty = w - fill;

                        StringBuilder b = new();
                        b.Append('[');
                        b.Append('#', fill);
                        b.Append(' ', empty);
                        b.Append(']');

                        LastLine += Log.Write(b.ToString());
                    }
                }
            }
            else
            {
                int w = LastLine.Length;

                LastLine.Back();
                LastLine = Log.Write(title);

                Console.Write(new string(' ', Math.Max(0, w - LastLine.Length)));
            }

            Log.Rekeep(LastLine);

            ResetTimer();
        }
    }

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
}
