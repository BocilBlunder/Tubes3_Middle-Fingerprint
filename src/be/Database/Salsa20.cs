using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tubes3_Middle_Fingerprint.Database
{
    internal class Salsa20
    {
        public static uint rotl(uint value, int shift)
        {
            return (value << shift) | (value >> (32 - shift));
        }

        static void s20_quarterround(ref uint y0, ref uint y1, ref uint y2, ref uint y3)
        {
            y1 = y1 ^ rotl(y0 + y3, 7);
            y2 = y2 ^ rotl(y1 + y0, 9);
            y3 = y3 ^ rotl(y2 + y1, 13);
            y0 = y0 ^ rotl(y3 + y2, 18);
        }

        static void s20_rowround(uint[] y)
        {
            s20_quarterround(ref y[0], ref y[1], ref y[2], ref y[3]);
            s20_quarterround(ref y[5], ref y[6], ref y[7], ref y[4]);
            s20_quarterround(ref y[10], ref y[11], ref y[8], ref y[9]);
            s20_quarterround(ref y[15], ref y[12], ref y[13], ref y[14]);
        }

        static void s20_columnround(uint[] x)
        {
            s20_quarterround(ref x[0], ref x[4], ref x[8], ref x[12]);
            s20_quarterround(ref x[5], ref x[9], ref x[13], ref x[1]);
            s20_quarterround(ref x[10], ref x[14], ref x[2], ref x[6]);
            s20_quarterround(ref x[15], ref x[3], ref x[7], ref x[11]);
        }

        static void s20_doubleround(uint[] x)
        {
            s20_columnround(x);
            s20_rowround(x);
        }

        static uint s20_littleendian(byte[] b, int start)
        {
            return b[0 + start] + ((uint)b[1 + start] << 8) + ((uint)b[2 + start] << 16) + ((uint)b[3 + start] << 24);
        }

        static void s20_rev_littleendian(byte[] b, uint w, int start)
        {
            b[0 + start] = (byte)w;
            b[1 + start] = (byte)(w >> 8);
            b[2 + start] = (byte)(w >> 16);
            b[3 + start] = (byte)(w >> 24);
        }

        static void s20_hash(byte[] seq)
        {
            int i;
            uint[] x = new uint[16];
            uint[] z = new uint[16];

            for (i = 0; i < 16; ++i)
            {
                x[i] = z[i] = s20_littleendian(seq, 4 * i);
            }

            for (i = 0; i < 10; ++i)
            {
                s20_doubleround(z);
            }

            for (i = 0; i < 16; ++i)
            {
                z[i] += x[i];
                s20_rev_littleendian(seq, z[i], 4 * i);
            }
        }


        static void s20_expand32(byte[] k, byte[] n, byte[] key_stream)
        {
            int i, j;

            byte[][] o = {
                new byte[] {(byte)'e', (byte)'x', (byte)'p', (byte)'a' },
                new byte[] {(byte)'n', (byte)'d', (byte)' ', (byte)'3' },
                new byte[] {(byte)'2', (byte)'-', (byte)'b', (byte)'y' },
                new byte[] {(byte)'t', (byte)'e', (byte)' ', (byte)'k' }
            };

            for (i = 0; i < 64; i += 20)
            {
                for (j = 0; j < 4; ++j)
                {
                    key_stream[i + j] = o[i / 20][j];
                }
            }

            for (i = 0; i < 16; ++i)
            {
                key_stream[4 + i] = k[i];
                key_stream[44 + i] = k[i + 16];
                key_stream[24 + i] = n[i];
            }

            s20_hash(key_stream);
        }

        public static void s20_crypt(byte[] key, byte[] nonce, uint s_i, byte[] buffer)
        {
            byte[] keystream = new byte[64];

            byte[] n = new byte[16];
            for (int idx = 0; idx < 16; idx++)
            {
                n[idx] = 0;
            }

            uint i;

            for (i = 0; i < 8; ++i)
            {
                n[i] = nonce[i];
            }


            if (s_i % 64 != 0)
            {
                s20_rev_littleendian(n, s_i / 64, 8);
                s20_expand32(key, n, keystream);
            }

            for (i = 0; i < buffer.Length; ++i)
            {
                if ((s_i + i) % 64 == 0)
                {
                    s20_rev_littleendian(n, ((s_i + i) / 64), 8);
                    s20_expand32(key, n, keystream);
                }
                buffer[i] ^= keystream[(s_i + i) % 64];
            }
        }
    }
}
