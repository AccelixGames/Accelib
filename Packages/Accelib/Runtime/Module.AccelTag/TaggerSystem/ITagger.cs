using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Accelib.Module.AccelTag.TaggerSystem
{
    public interface ITagger
    {
        public IEnumerable<SO_AccelTag> TagList { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasTag(SO_AccelTag tag) => TagList.Contains(tag);

        public bool AnyTagsMatch(in IEnumerable<SO_AccelTag> tags)
        {
            if (!tags.Any()) return true;
            return tags.Any(HasTag);
        }

        public bool AllTagsMatch(in IEnumerable<SO_AccelTag> tags)
        {
            if (!tags.Any()) return true;
            return tags.All(HasTag);
        }

        public bool NoTagsMatch(in IEnumerable<SO_AccelTag> tags)
        {
            if (!tags.Any()) return true;
            return !tags.Any(HasTag);
        }
    }
}