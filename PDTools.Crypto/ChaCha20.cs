/*
 * Copyright (c) 2015, 2018 Scott Bennett
 *           (c) 2018-2021 Kaarlo Räihä
 *
 * Permission to use, copy, modify, and distribute this software for any
 * purpose with or without fee is hereby granted, provided that the above
 * copyright notice and this permission notice appear in all copies.
 *
 * THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
 * WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
 * ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
 * WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
 * ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
 * OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
 */

// Re-adapted to use Span - Nenkai

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices; // For MethodImplOptions.AggressiveInlining

namespace PDTools.Crypto
{
	/// <summary>
	/// Class that can be used for ChaCha20 encryption / decryption
	/// </summary>
	public sealed class ChaCha20 : IDisposable
	{
		/// <summary>
		/// Only allowed key lenght in bytes
		/// </summary>
		public const int allowedKeyLength = 32;

		/// <summary>
		/// Only allowed nonce lenght in bytes
		/// </summary>
		public const int allowedNonceLength = 12;

		/// <summary>
		/// How many bytes are processed per loop
		/// </summary>
		public const int processBytesAtTime = 64;

		private const int stateLength = 16;

		/// <summary>
		/// The ChaCha20 state (aka "context")
		/// </summary>
		private uint[] state;

		/// <summary>
		/// Determines if the objects in this class have been disposed of. Set to true by the Dispose() method.
		/// </summary>
		private bool isDisposed;

		/// <summary>
		/// Set up a new ChaCha20 state. The lengths of the given parameters are checked before encryption happens.
		/// </summary>
		/// <remarks>
		/// See <a href="https://tools.ietf.org/html/rfc7539#page-10">ChaCha20 Spec Section 2.4</a> for a detailed description of the inputs.
		/// </remarks>
		/// <param name="key">
		/// A 32-byte (256-bit) key, treated as a concatenation of eight 32-bit little-endian integers
		/// </param>
		/// <param name="nonce">
		/// A 12-byte (96-bit) nonce, treated as a concatenation of three 32-bit little-endian integers
		/// </param>
		/// <param name="counter">
		/// A 4-byte (32-bit) block counter, treated as a 32-bit little-endian integer
		/// </param>
		public ChaCha20(byte[] key, byte[] nonce, uint counter)
		{
			this.state = new uint[stateLength];
			this.isDisposed = false;

			this.KeySetup(key);
			this.IvSetup(nonce, counter);
		}

		/// <summary>
		/// The ChaCha20 state (aka "context"). Read-Only.
		/// </summary>
		public uint[] State
		{
			get
			{
				return this.state;
			}
		}


		// These are the same constants defined in the reference implementation.
		// http://cr.yp.to/streamciphers/timings/estreambench/submissions/salsa20/chacha8/ref/chacha.c
		private static readonly byte[] sigma = Encoding.ASCII.GetBytes("expand 32-byte k");
		private static readonly byte[] tau = Encoding.ASCII.GetBytes("expand 16-byte k");

		/// <summary>
		/// Set up the ChaCha state with the given key. A 32-byte key is required and enforced.
		/// </summary>
		/// <param name="key">
		/// A 32-byte (256-bit) key, treated as a concatenation of eight 32-bit little-endian integers
		/// </param>
		private void KeySetup(byte[] key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("Key is null");
			}

			if (key.Length != allowedKeyLength)
			{
				throw new ArgumentException($"Key length must be {allowedKeyLength}. Actual: {key.Length}");
			}

			state[4] = Util.U8To32Little(key, 0);
			state[5] = Util.U8To32Little(key, 4);
			state[6] = Util.U8To32Little(key, 8);
			state[7] = Util.U8To32Little(key, 12);

			byte[] constants = (key.Length == allowedKeyLength) ? sigma : tau;
			int keyIndex = key.Length - 16;

			state[8] = Util.U8To32Little(key, keyIndex + 0);
			state[9] = Util.U8To32Little(key, keyIndex + 4);
			state[10] = Util.U8To32Little(key, keyIndex + 8);
			state[11] = Util.U8To32Little(key, keyIndex + 12);

			state[0] = Util.U8To32Little(constants, 0);
			state[1] = Util.U8To32Little(constants, 4);
			state[2] = Util.U8To32Little(constants, 8);
			state[3] = Util.U8To32Little(constants, 12);
		}

		/// <summary>
		/// Set up the ChaCha state with the given nonce (aka Initialization Vector or IV) and block counter. A 12-byte nonce and a 4-byte counter are required.
		/// </summary>
		/// <param name="nonce">
		/// A 12-byte (96-bit) nonce, treated as a concatenation of three 32-bit little-endian integers
		/// </param>
		/// <param name="counter">
		/// A 4-byte (32-bit) block counter, treated as a 32-bit little-endian integer
		/// </param>
		private void IvSetup(byte[] nonce, uint counter)
		{
			if (nonce == null)
			{
				// There has already been some state set up. Clear it before exiting.
				Dispose();
				throw new ArgumentNullException("Nonce is null");
			}

			if (nonce.Length != allowedNonceLength)
			{
				// There has already been some state set up. Clear it before exiting.
				Dispose();
				throw new ArgumentException($"Nonce length must be {allowedNonceLength}. Actual: {nonce.Length}");
			}

			state[12] = counter;
			state[13] = Util.U8To32Little(nonce, 0);
			state[14] = Util.U8To32Little(nonce, 4);
			state[15] = Util.U8To32Little(nonce, 8);
		}


		#region Encryption methods



		#endregion // Encryption methods


		#region // Decryption methods

		/// <summary>
		/// Decrypt arbitrary-length byte array (input), writing the resulting byte array to the output buffer.
		/// </summary>
		/// <remarks>Since this is symmetric operation, it doesn't really matter if you use Encrypt or Decrypt method</remarks>
		/// <param name="output">Output byte array</param>
		/// <param name="input">Input byte array</param>
		/// <param name="numBytes">Number of bytes to decrypt</param>
		public void DecryptBytes(Span<byte> bytes, int numBytes, ulong globalOffset = 0)
		{
			this.WorkBytes(bytes, numBytes, globalOffset);
		}



		#endregion // Decryption methods

		/// <summary>
		/// Encrypt or decrypt an arbitrary-length byte array (input), writing the resulting byte array to the output buffer. The number of bytes to read from the input buffer is determined by numBytes.
		/// </summary>
		/// <param name="output">Output byte array</param>
		/// <param name="input">Input byte array</param>
		/// <param name="numBytes">How many bytes to process</param>
		private void WorkBytes(Span<byte> bytes, int numBytes, ulong globalOffset = 0, int offset = 0)
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException("state", "The ChaCha state has been disposed");
			}


			uint[] x = new uint[stateLength];    // Working buffer
			byte[] tmp = new byte[processBytesAtTime];  // Temporary buffer

			ulong current = globalOffset & 0x3f;

			this.state[12] = (uint)globalOffset / 0x40;
			Hash(x, tmp);
			
			int pos = 0;
			while (pos < offset + numBytes)
			{
				bytes[pos++] ^= tmp[(int)current++];

				if (current >= 0x40)
				{
					state[12]++;
					if (state[12] == 0)
						state[13]++;

					Hash(x, tmp);

					current = 0;
				}
			}
        }


        private void Hash(uint[] x, byte[] tmp)
        {
			Buffer.BlockCopy(this.state, 0, x, 0, stateLength * sizeof(uint));

			for (int i = 0; i < 10; i++)
            {
                QuarterRound(x, 0, 4, 8, 12);
                QuarterRound(x, 1, 5, 9, 13);
                QuarterRound(x, 2, 6, 10, 14);
                QuarterRound(x, 3, 7, 11, 15);

                QuarterRound(x, 0, 5, 10, 15);
                QuarterRound(x, 1, 6, 11, 12);
                QuarterRound(x, 2, 7, 8, 13);
                QuarterRound(x, 3, 4, 9, 14);
            }

            for (int i = 0; i < stateLength; i++)
            {
                Util.ToBytes(tmp, Util.Add(x[i], this.state[i]), 4 * i);
            }
        }

        /// <summary>
        /// The ChaCha Quarter Round operation. It operates on four 32-bit unsigned integers within the given buffer at indices a, b, c, and d.
        /// </summary>
        /// <remarks>
        /// The ChaCha state does not have four integer numbers: it has 16. So the quarter-round operation works on only four of them -- hence the name. Each quarter round operates on four predetermined numbers in the ChaCha state.
        /// See <a href="https://tools.ietf.org/html/rfc7539#page-4">ChaCha20 Spec Sections 2.1 - 2.2</a>.
        /// </remarks>
        /// <param name="x">A ChaCha state (vector). Must contain 16 elements.</param>
        /// <param name="a">Index of the first number</param>
        /// <param name="b">Index of the second number</param>
        /// <param name="c">Index of the third number</param>
        /// <param name="d">Index of the fourth number</param>
        private static void QuarterRound(uint[] x, uint a, uint b, uint c, uint d)
		{
			x[a] = Util.Add(x[a], x[b]);
			x[d] = Util.Rotate(Util.XOr(x[d], x[a]), 16);

			x[c] = Util.Add(x[c], x[d]);
			x[b] = Util.Rotate(Util.XOr(x[b], x[c]), 12);

			x[a] = Util.Add(x[a], x[b]);
			x[d] = Util.Rotate(Util.XOr(x[d], x[a]), 8);

			x[c] = Util.Add(x[c], x[d]);
			x[b] = Util.Rotate(Util.XOr(x[b], x[c]), 7);
		}

		#region Destructor and Disposer

		/// <summary>
		/// Clear and dispose of the internal state. The finalizer is only called if Dispose() was never called on this cipher.
		/// </summary>
		~ChaCha20()
		{
			Dispose(false);
		}

		/// <summary>
		/// Clear and dispose of the internal state. Also request the GC not to call the finalizer, because all cleanup has been taken care of.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			/*
			 * The Garbage Collector does not need to invoke the finalizer because Dispose(bool) has already done all the cleanup needed.
			 */
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// This method should only be invoked from Dispose() or the finalizer. This handles the actual cleanup of the resources.
		/// </summary>
		/// <param name="disposing">
		/// Should be true if called by Dispose(); false if called by the finalizer
		/// </param>
		private void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				if (disposing)
				{
					/* Cleanup managed objects by calling their Dispose() methods */
				}

				/* Cleanup any unmanaged objects here */
				if (state != null)
				{
					Array.Clear(state, 0, state.Length);
				}

				state = null;
			}

			isDisposed = true;
		}

		#endregion // Destructor and Disposer
	}

	/// <summary>
	/// Utilities that are used during compression
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// n-bit left rotation operation (towards the high bits) for 32-bit integers.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="c"></param>
		/// <returns>The result of (v LEFTSHIFT c)</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Rotate(uint v, int c)
		{
			unchecked
			{
				return (v << c) | (v >> (32 - c));
			}
		}

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
		public static void ToBytes(byte[] output, uint input, int outputOffset)
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
}