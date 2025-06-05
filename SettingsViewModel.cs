namespace Flow.Launcher.Plugin.FileFolderOpener
{
    public class SettingsViewModel
    {
        public SettingsViewModel(Settings settings)
        {
            Settings = settings;
        }

        public Settings Settings { get; }
    }
}
