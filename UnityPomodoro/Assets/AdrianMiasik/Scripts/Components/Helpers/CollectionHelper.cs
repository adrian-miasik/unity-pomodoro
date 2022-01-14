namespace AdrianMiasik.Components.Helpers
{
    /// <summary>
    /// Helper methods for lists.
    /// </summary>
    public static class CollectionHelper
    {
        public static int Wrap(int index, int length)
        {
            return (index % length + length) % length;
        }
    }
}