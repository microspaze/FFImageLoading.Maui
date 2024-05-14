using System;
using System.Diagnostics;

namespace FFImageLoading
{
    public static class ObjectExtensions
    {
        public static bool TryDispose(this IDisposable obj)
        {
            try
            {
                if (obj != null)
                {
                    obj?.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
				Debug.WriteLine(ex.Message);
            }

            return false;
        }
    }
}
