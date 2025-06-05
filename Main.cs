using Flow.Launcher.Plugin.SharedCommands;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.FileFolderOpener
{
    public class Main : IPlugin, ISettingProvider
    {
        internal static PluginInitContext _context;

        private Settings _settings;
        private SettingsViewModel _viewModel;
        private const string _app_icon_path = "Images\\app.png";
        private const string _folder_icon_path = "Images\\folder.png";
        private const string _file_icon_path = "Images\\file.png";


        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = _context.API.LoadSettingJsonStorage<Settings>();
            _viewModel = new SettingsViewModel(_settings);
        }
        public List<Result> Query(Query query)
        {
            string search = query.Search?.Trim().ToLower();
            if (string.IsNullOrEmpty(search))
                return new List<Result>();

            var keywordMatches = _settings.Folders
                .Where(item => !item.ActionKeyword.Equals(string.Empty) && search.StartsWith(item.ActionKeyword))
                .ToList();

            if (!keywordMatches.Any())
                return new List<Result>();
            var results = new List<Result>();
        
            if(_settings.EnableBulkOpen && keywordMatches.Count>1)
            {
                results.Add(new Result
                {
                    Title = $"Bulk Open ({keywordMatches.Count()} items)",
                    SubTitle = "Open all matching files and folders at once",

                    IcoPath = _app_icon_path,
                    Score = 150,
                    Action = e =>
                    {
                        try
                        {
                            keywordMatches.ForEach(x => _context.API.OpenDirectory(x.Path));
                            return true;
                        }
                        catch { return false; }
                    }
                });
            }

            results.AddRange(CreateResults(keywordMatches));
            return results;
        }

        public List<Result> CreateResults(IEnumerable<FileFolder> folders)
        {
            var folderResults = new List<Result>();
            var fileResults = new List<Result>();

            foreach (var folder in folders)
            {
                bool isFile = FilesFolders.FileExists(folder.Path);
                bool isFolder = FilesFolders.LocationExists(folder.Path);

                if (!isFile && !isFolder)
                    continue;

                string iconPath = folder.IconPath;

                if (string.IsNullOrEmpty(iconPath) || !File.Exists(iconPath))
                {
                    iconPath = isFolder ? _folder_icon_path : _file_icon_path;
                }

                var result = new Result
                {
                    Title = folder.Title,
                    SubTitle = folder.Path,
                    Score = 100,
                    Action = e =>
                    {
                        try
                        {
                            _context.API.OpenDirectory(folder.Path);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    },
                    IcoPath = iconPath
                };
                (isFolder ? folderResults : fileResults).Add(result);
            }

            return folderResults.Concat(fileResults).ToList();
        }



        public Control CreateSettingPanel()
        {
            return new SettingsControl(_context, _viewModel);
        }
    }
}