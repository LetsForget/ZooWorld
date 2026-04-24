using System;

namespace ContentLoading
{
    public class LoadingFailedException : Exception
    {
        public string Path { get; }

        public LoadingFailedException(string path)
        {
            Path = path;
        }
    }
}