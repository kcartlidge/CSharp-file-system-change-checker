    public class FileSystemChangeChecker
    {
        public enum CheckStates
        {
            NoChange, FolderRemoved, FileAdded, FileRemoved, FileChanged
        }

        private SortedList<string, DateTime> lastKnownFiles = new SortedList<string, DateTime>();

        public static string CheckStateText(CheckStates checkState)
        {
            switch (checkState)
            {
                case CheckStates.NoChange: return "No change.";
                case CheckStates.FolderRemoved: return "Folder(s) removed.";
                case CheckStates.FileAdded: return "File(s) added.";
                case CheckStates.FileRemoved: return "File(s) removed.";
                case CheckStates.FileChanged: return "File(s) changed.";
            }
            return "Unknown state!";
        }

        public CheckStates Check(IEnumerable<string> folders, string mask)
        {
            var currentState = new SortedList<string, DateTime>();

            var result = CheckStates.NoChange;

            foreach(var folder in folders)
            {
                if (!Directory.Exists(folder)) return CheckStates.FolderRemoved;
                foreach (var file in Directory.GetFiles(folder))
                {
                    currentState.Add(file, File.GetLastWriteTimeUtc(file));
                }
            }
            if (haveChangedTimeStamps(currentState)) result = CheckStates.FileChanged;
            if (haveLostExistingFiles(currentState)) result = CheckStates.FileRemoved;
            if (haveFoundExtraFiles(currentState)) result = CheckStates.FileAdded;

            lastKnownFiles = currentState;
            return result;
        }

        private bool haveFoundExtraFiles(SortedList<string, DateTime> latestFiles)
        {
            foreach (var existingFile in latestFiles)
            {
                if (!lastKnownFiles.ContainsKey(existingFile.Key)) return true;
            }
            return false;
        }

        private bool haveLostExistingFiles(SortedList<string, DateTime> latestFiles)
        {
            foreach (var previousFile in lastKnownFiles)
            {
                if (!latestFiles.ContainsKey(previousFile.Key)) return true;
            }
            return false;
        }

        private bool haveChangedTimeStamps(SortedList<string, DateTime> latestFiles)
        {
            foreach (var previousFile in lastKnownFiles)
            {
                if (latestFiles.ContainsKey(previousFile.Key))
                {
                    if (previousFile.Value != latestFiles[previousFile.Key]) return true;
                }
            }
            return false;
        }
    }
