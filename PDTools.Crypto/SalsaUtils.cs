using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace PDTools.Crypto;

internal static class Util
{
	/// <summary>
	/// Unchecked integer exclusive or (XOR) operation.
	/// </summary>
	/// <param name="v"></param>
	/// <param name="w"></param>
	/// <returns>The result of (v XOR w)</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint XOr(uint v, uint w)
	{
		return unchecked(v ^ w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint RotateLeft(uint value, int offset)
	{
		return BitOperations.RotateLeft(value, offset);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint RotateRight(uint value, int offset)
	{
		return BitOperations.RotateRight(value, offset);
	}

	/// <summary>
	/// Unchecked integer addition. The ChaCha spec defines certain operations to use 32-bit unsigned integer addition modulo 2^32.
	/// </summary>
	/// <remarks>
	/// See <a href="https://tools.ietf.org/html/rfc7539#page-4">ChaCha20 Spec Section 2.1</a>.
	/// </remarks>
	/// <param name="v"></param>
	/// <param name="w"></param>
	/// <returns>The result of (v + w) modulo 2^32</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Add(uint v, uint w)
	{
		return unchecked(v + w);
	}

	/// <summary>
	/// Add 1 to the input parameter using unchecked integer addition. The ChaCha spec defines certain operations to use 32-bit unsigned integer addition modulo 2^32.
	/// </summary>
	/// <remarks>
	/// See <a href="https://tools.ietf.org/html/rfc7539#page-4">ChaCha20 Spec Section 2.1</a>.
	/// </remarks>
	/// <param name="v"></param>
	/// <returns>The result of (v + 1) modulo 2^32</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint AddOne(uint v)
	{
		return unchecked(v + 1);
	}

	/// <summary>
	/// Convert four bytes of the input buffer into an unsigned 32-bit integer, beginning at the inputOffset.
	/// </summary>
	/// <param name="p"></param>
	/// <param name="inputOffset"></param>
	/// <returns>An unsigned 32-bit integer</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint U8To32Little(byte[] p, int inputOffset)
	{
		unchecked
		{
			return ((uint)p[inputOffset]
				| ((uint)p[inputOffset + 1] << 8)
				| ((uint)p[inputOffset + 2] << 16)
				| ((uint)p[inputOffset + 3] << 24));
		}
	}

	/// <summary>
	/// Serialize the input integer into the output buffer. The input integer will be split into 4 bytes and put into four sequential places in the output buffer, starting at the outputOffset.
	/// </summary>
	/// <param name="output"></param>
	/// <param name="input"></param>
	/// <param name="outputOffset"></param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToBytes(Span<byte> output, uint input, int outputOffset)
	{
		unchecked
		{
			output[outputOffset] = (byte)input;
			output[outputOffset + 1] = (byte)(input >> 8);
			output[outputOffset + 2] = (byte)(input >> 16);
			output[outputOffset + 3] = (byte)(input >> 24);
		}
	}
}
