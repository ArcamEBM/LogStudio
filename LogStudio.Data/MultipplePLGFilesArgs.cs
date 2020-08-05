using System;

namespace LogStudio.Data
{
    public class MultipplePLGFilesArgs : EventArgs
    {
        public MultipplePLGFilesArgs(string[] availableFilenames)
        {
            AvailableFilenames = availableFilenames;
        }

        public string[] AvailableFilenames { get; private set; }

        public string SelectedFilename { get; set; }

        public bool Cancel { get; set; }
    }
}
