namespace AdrianMiasik.Components.Core.Helpers
{
    /// <summary>
    /// Helper methods for collections.
    /// </summary>
    public static class CollectionHelper
    {
        /// <summary>
        /// Wraps an index within the bounds of a collection.
        /// </summary>
        /// <param name="index">The index you are trying to get (can surpass the bounds of your collection, it will
        /// divide down to a value within the bounds of your provided collection length as the second param)</param>
        /// <param name="length">The length of the collection</param>
        /// <example>(E.g: You provide index -1 with length 3, you get index (2). This works both negative and positive)
        /// </example>
        /// <returns></returns>
        public static int Wrap(int index, int length)
        {
            return (index % length + length) % length;
        }
    }
}