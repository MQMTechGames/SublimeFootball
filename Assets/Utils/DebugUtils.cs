public static class DebugUtils
{
    public static void Assert(bool assertion)
    {
        DebugUtils.Assert(assertion, "Assert Err");
    }

    public static void Assert(bool assertion, string msg)
    {
        if(!assertion)
        {
            throw new System.Exception(msg);
        }
    }
}
