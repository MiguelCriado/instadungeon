public static class byteExtensions
{
	public static byte RotateLeft(this byte value, int count)
	{
		return (byte) ((value << count) | (value >> (8 - count)));
	}

	public static byte RotateRight(this byte value, int count)
	{
		return (byte)((value >> count) | (value << (8 - count)));
	}
}
