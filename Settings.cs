using System.Collections.ObjectModel;

namespace Flow.Launcher.Plugin.FileFolderOpener
{
    public class Settings : BaseModel
    {
        public bool EnableBulkOpen { get; set; } = false;
        public ObservableCollection<FileFolder> Folders { get; set; } = new ObservableCollection<FileFolder>{};
    }
}
