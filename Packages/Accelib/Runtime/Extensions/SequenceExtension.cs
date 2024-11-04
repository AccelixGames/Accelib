using DG.Tweening;

namespace Accelib.Extensions
{
    public static class SequenceExtension
    {
        public static Sequence SafeAppend(this Sequence seq, DG.Tweening.Tween t)
        {
            if(t != null) seq.Append(t);
            return seq;
        }

        public static Sequence SafeJoin(this Sequence seq, DG.Tweening.Tween t)
        {
            if(t != null) seq.Join(t);
            return seq;
        }
    }
}