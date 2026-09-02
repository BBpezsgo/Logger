using System.Text;

namespace Logger;

public class SteppingProgressBar : ProgressBarBase
{
    readonly int Total;
    readonly string Characters;
    int Current;
    string Title = string.Empty;

    public SteppingProgressBar(int total, string characters = Hashtags)
    {
        Total = total;
        Characters = characters;
    }

    public void Step(int count = 1)
    {
        Interlocked.Exchange(ref Current, Current + count);
    }

    public void Step(string title, int count = 0)
    {
        Interlocked.Exchange(ref Title, title);
        if (count != 0) Interlocked.Exchange(ref Current, Current + count);
    }

    protected override void Render(ref LogEntry line)
    {

        string title = Title;

        int width = Math.Min(Console.WindowWidth - 2, MaxWidth);

        if (title.Length >= width / 2)
        {
            title = title[..(width / 2 - 3)] + "...";
        }

        if (Total <= 0)
        {
            line = Log.Write(title + new string(' ', Math.Max(0, width - title.Length - 1)));
            return;
        }

        float progress = (float)Current / (float)Total;

        if (string.IsNullOrWhiteSpace(Title))
        {
            int w = width;
            if (w >= 2)
            {
                StringBuilder b = new();
                b.Append('[');
                b.Append(Render(progress, w, Characters));
                b.Append(']');

                line += Log.Write(b.ToString());
            }
        }
        else
        {
            line += Log.Write(title);
            line += Log.Write(new string(' ', Math.Max(0, width / 2 - title.Length)));
            int w = width - Console.CursorLeft - 1;
            if (w >= 2)
            {
                StringBuilder b = new();
                b.Append('[');
                b.Append(Render(progress, w, Characters));
                b.Append(']');

                line += Log.Write(b.ToString());
            }
        }
    }
}
