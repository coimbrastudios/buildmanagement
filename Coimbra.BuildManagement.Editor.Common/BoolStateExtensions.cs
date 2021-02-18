namespace Coimbra.BuildManagement.Editor.Common
{
    internal static class BoolStateExtensions
    {
        internal static bool GetOrDefault(this BoolState boolState, bool defaultValue)
        {
            switch (boolState)
            {
                case BoolState.True:
                {
                    return true;
                }

                case BoolState.False:
                {
                    return false;
                }

                default:
                {
                    return defaultValue;
                }
            }
        }
    }
}
