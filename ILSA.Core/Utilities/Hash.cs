namespace ILSA.Core {
    using System.Runtime.CompilerServices;

    static class Murmur<TSource> where TSource : class {
        readonly static int Seed = Compress(448839895, typeof(TSource).GetHashCode());
        //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Calc(TSource source) {
            int sourceHash = (source != null) ? source.GetHashCode() : 62043647;
            return Finalization(Compress(Seed, sourceHash));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Calc(TSource source, int value) {
            int sourceHash = (source != null) ? source.GetHashCode() : 62043647;
            return Finalization(Compress(Compress(Seed, sourceHash),value));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Compress(int prev, int next) {
            uint num = (uint)prev;
            uint num2 = (uint)next;
            num2 *= 1540483477;
            num2 ^= num2 >> 24;
            num2 *= 1540483477;
            num *= 1540483477;
            return (int)(num ^ num2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Finalization(int hashState) {
            uint num = (uint)hashState;
            num ^= num >> 13;
            num *= 1540483477;
            return (int)(num ^ (num >> 15));
        }
    }
}