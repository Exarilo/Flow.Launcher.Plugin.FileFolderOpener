namespace Flow.Launcher.Plugin.FileFolderOpener
{
    public class FileFolder : BaseModel
    {
        public FileFolder() { }
        private string title { get; set; } = string.Empty;
        private string actionKeyword { get; set; } = string.Empty;
        private string path { get; set; } = string.Empty;
        private string iconPath= string.Empty;

        public string Path
        {
            get => path;
            set
            {
                if (path != value)
                {
                    path = value?.Trim() ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }
        public string Title
        {
            get => title;
            set
            {
                if (title != value)
                {
                    title = value?.Trim() ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }
        public string ActionKeyword
        {
            get => actionKeyword;
            set
            {
                if (actionKeyword != value)
                {
                    actionKeyword = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        public string IconPath
        {
            get => iconPath;
            set
            {
                if (iconPath != value)
                {
                    iconPath = value?.Trim() ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }
    }
}
