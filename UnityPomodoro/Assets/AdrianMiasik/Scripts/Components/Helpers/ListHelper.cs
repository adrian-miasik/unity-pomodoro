namespace AdrianMiasik.Components.Helpers
{
    public static class ListHelper
    {
        public static int Wrap(int index, int length)
        {
            return (index % length + length) % length;
        }
    }
}