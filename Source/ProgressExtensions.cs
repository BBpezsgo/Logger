namespace Logger;

public static class ProgressExtensions
{
    public static IEnumerable<T> WithProgress<T>(this IReadOnlyCollection<T> collection, ProgressBar progressBar, string title)
    {
        int current = 0;
        int total = collection.Count;
        foreach (T item in collection)
        {
            progressBar.Report(title, current++, total);
            yield return item;
        }
    }

    public static IEnumerable<T> WithProgress<T>(this IReadOnlyCollection<T> collection, ProgressBar progressBar)
    {
        int current = 0;
        int total = collection.Count;
        foreach (T item in collection)
        {
            progressBar.Report(current++, total);
            yield return item;
        }
    }
}
