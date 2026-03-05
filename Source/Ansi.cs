namespace Logger;

public static class Ansi
{
    public static string Bold(this string v) => $"\e[1m{v}\e[22m";
    public static string Dim(this string v) => $"\e[2m{v}\e[22m";
    public static string Italic(this string v) => $"\e[3m{v}\e[23m";
    public static string Underline(this string v) => $"\e[4m{v}\e[24m";
    public static string Blinking(this string v) => $"\e[5m{v}\e[25m";
    public static string Inverse(this string v) => $"\e[7m{v}\e[27m";
    public static string Hidden(this string v) => $"\e[8m{v}\e[28m";
    public static string Strikethrough(this string v) => $"\e[9m{v}\e[29m";
}
