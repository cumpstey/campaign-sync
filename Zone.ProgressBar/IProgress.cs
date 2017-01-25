namespace Zone.Progress
{
    /// <summary>
    /// Defines a provider for progress updates.
    /// </summary>
    /// <typeparam name="T">The type of progress update value.</typeparam>
    /// <remarks>
    /// System.IProgress<T> was introduced in .NET 4.5. If we upgrade, we can remove this custom interface.
    /// </remarks>
    public interface IProgress<in T>
    {
        void Report(T value);
    }
}
