namespace LogStudio.Data
{
    public static class ArrayExtensions
    {
        public static int IndexOf(this byte[] source, byte value, int startPos)
        {
            for (int i = startPos; i < source.Length; i++)
            {
                if (source[i] == value)
                    return i;
            }

            return -1;
        }

        public static int IndexOf(this char[] source, char value, int startPos)
        {
            for (int i = startPos; i < source.Length; i++)
            {
                if (source[i] == value)
                    return i;
            }

            return -1;
        }

        public static bool Compare(this byte[] source, int sourceStart, byte[] target, int targetStart, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (source[sourceStart + i] != target[targetStart + i])
                    return false;
            }

            return true;
        }
    }
}