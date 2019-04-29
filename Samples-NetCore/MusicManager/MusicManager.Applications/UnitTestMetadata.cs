using System.ComponentModel.Composition.Primitives;

namespace Waf.MusicManager.Applications
{
    public static class UnitTestMetadata
    {
        public const string Name = "UnitTest";

        public const string Data = "Data";

        public static bool IsContained(ComposablePartDefinition definition, object value = null)
        {
            if (value == null)
            {
                return definition.Metadata.ContainsKey(Name);
            }
            else
            {
                return definition.Metadata.ContainsKey(Name) && definition.Metadata[Name].Equals(value);
            }
        }
    }
}
