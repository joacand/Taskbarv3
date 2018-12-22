using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;
using Taskbarv3.Core.Models.Events;
using Taskbarv3.UI.Models;
using PubSub.Extension;
using Taskbarv3.Core.Extensions;

namespace Taskbarv3.UI.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private readonly IConfigHandler configHandler;
        private readonly IHueService hueService;
        private readonly ISongViewerService songViewerService;
        private readonly IMediaControlService mediaControlService;
        private readonly IFavoritesService favoritesService;
        private readonly IWindowService windowService;
        private readonly IShortcutsHandler shortcutsHandler;
        private readonly IWorkAreaService workAreaService;
        private readonly IStatusService statusService;
        private readonly ICpuUsageService cpuUsageService;
        private readonly AddShortcutViewModel addShortcutViewModel;
        private readonly SettingsViewModel settingsViewModel;

        private static readonly string SONGVIEWER_OFFLINE = "SongViewer offline";
        private Timer uiUpdateTimer = new Timer();
        private Timer statusTimer = new Timer();
        private Timer hueSliderTimer = new Timer();
        private string dateText = string.Empty;
        private string songViewerText = SONGVIEWER_OFFLINE;
        private int cpuProgressBarValue;
        private int hueSliderValue;
        private MainConfig config;
        private string lastSong;
        private bool songChanged;
        private bool customWorkArea;
        private string statusMsg = "";
        private bool statusOn;
        private string songViewerTextColor = TextColors.WhiteSmoke.ToString();

        private enum TextColors
        {
            WhiteSmoke,
            LimeGreen
        }

        // Hardcoded constant for now - should be accessed in another way
        private static readonly int WINDOW_HEIGHT = 45;

        public ObservableCollection<Shortcut> Shortcuts { get; set; } = new ObservableCollection<Shortcut>();
        public string DateText { get => dateText; set => SetProperty(ref dateText, value); }
        public int CpuProgressBarValue { get => cpuProgressBarValue; set => SetProperty(ref cpuProgressBarValue, value); }
        public int HueSliderValue { get => hueSliderValue; set { SetProperty(ref hueSliderValue, value); HueDimmer_Scroll(); } }
        public string SongViewerText { get => songViewerText; set => SetProperty(ref songViewerText, value); }
        public string SongViewerTextColor { get => songViewerTextColor; set => SetProperty(ref songViewerTextColor, value); }

        public ICommand ToggleSongViewerCommand { get; set; }
        public ICommand PlayCommand { get; set; }
        public ICommand SkipCommand { get; set; }
        public ICommand PlayFavoritesCommand { get; set; }
        public ICommand AddToFavoritesCommand { get; set; }
        public ICommand AddShortcutCommand { get; set; }
        public ICommand StartShortcutActionCommand { get; set; }
        public ICommand RemoveShortcutCommand { get; set; }
        public ICommand ToggleWorkAreaCommand { get; set; }
        public ICommand OpenSettingsCommand { get; set; }

        public MainWindowViewModel(
            IConfigHandler configHandler,
            IHueService hueService,
            ISongViewerService songViewerService,
            IMediaControlService mediaControlService,
            IFavoritesService favoritesService,
            IWindowService windowService,
            IShortcutsHandler shortcutsHandler,
            IWorkAreaService workAreaService,
            IStatusService statusService,
            ICpuUsageService cpuUsageService,
            AddShortcutViewModel addShortcutViewModel,
            SettingsViewModel settingsViewModel)
        {
            this.configHandler = configHandler;
            this.hueService = hueService;
            this.songViewerService = songViewerService;
            this.mediaControlService = mediaControlService;
            this.favoritesService = favoritesService;
            this.windowService = windowService;
            this.shortcutsHandler = shortcutsHandler;
            this.workAreaService = workAreaService;
            this.statusService = statusService;
            this.addShortcutViewModel = addShortcutViewModel;
            this.settingsViewModel = settingsViewModel;
            this.cpuUsageService = cpuUsageService;

            ToggleSongViewerCommand = new RelayCommand(OnToggleSongViewer);
            PlayCommand = new RelayCommand(OnPlayCommand);
            SkipCommand = new RelayCommand(OnSkipCommand);
            PlayFavoritesCommand = new RelayCommand(OnPlayFavoritesCommand);
            AddToFavoritesCommand = new RelayCommand(OnAddToFavoritesCommand);
            AddShortcutCommand = new RelayCommand(OnAddShortcutCommand);
            StartShortcutActionCommand = new RelayCommand(OnStartShortcutActionCommand);
            RemoveShortcutCommand = new RelayCommand(OnRemoveShortcutCommand);
            ToggleWorkAreaCommand = new RelayCommand(OnToggleWorkAreaCommand);
            OpenSettingsCommand = new RelayCommand(OnOpenSettingsCommand);

            this.Subscribe<ShortcutAddedEvent>(OnAddShortcutEvent);
            this.statusService.SetStatusAction = OnStatusChange;
            SetWorkArea();

            Task.Run(async () => await InitAsync());
            Task.Run(() => LoadShortcuts());
        }

        private async Task InitAsync()
        {
            UpdateUI();
            await Task.Run(() =>
            {
                config = LoadMainConfig();
                SongViewerText = SONGVIEWER_OFFLINE;
                InitHue();
            });
            InitTimer();
        }

        private MainConfig LoadMainConfig()
        {
            try
            {
                return configHandler.LoadFromFile();
            }
            catch (Exception e)
            {
                statusService.SetStatus($"Exception: {e}");
            }
            return null;
        }

        private void LoadShortcuts()
        {
            try
            {
                var shortcuts = shortcutsHandler.LoadFromFile();

                foreach (Shortcut shortcut in shortcutsHandler.LoadFromFile())
                {
                    Shortcuts.Add(shortcut);
                }
            }
            catch (Exception e)
            {
                statusService.SetStatus($"Exception: {e}");
            }
        }

        private void OnToggleSongViewer(object _)
        {
            lastSong = "";
            if (songViewerService.IsOnline)
            {
                songViewerService.Stop();
                SongViewerText = SONGVIEWER_OFFLINE;
                favoritesService.Reset();
            }
            else
            {
                songViewerService.Start();
                UpdateUI();
            }
        }

        private void OnPlayCommand(object _)
        {
            mediaControlService.PlayPause();
        }

        private void OnSkipCommand(object _)
        {
            mediaControlService.NextSong();
        }

        private void OnPlayFavoritesCommand(object _)
        {
            favoritesService.PlayFavorites();
        }

        private void OnAddToFavoritesCommand(object _)
        {
            bool successful = favoritesService.AddToFavorites();
            if (successful)
            {
                lastSong = "";
            }
        }

        private void OnAddShortcutCommand(object _)
        {
            windowService.ShowWindow(PopupWindow.AddShortcut, addShortcutViewModel);
        }

        private void OnStartShortcutActionCommand(object obj)
        {
            if (obj is Shortcut shortcut)
            {
                try
                {
                    shortcut.OpenProcess();
                }
                catch (Exception e)
                {
                    statusService.SetStatus($"Exception: {e}");
                }
            }
        }

        private void OnRemoveShortcutCommand(object obj)
        {
            RemoveShortcut(obj as Shortcut);
        }

        private void RemoveShortcut(Shortcut shortcut)
        {
            if (shortcut != null)
            {
                Shortcuts.Remove(shortcut);
                SaveShortcuts();
            }
        }

        private void OnToggleWorkAreaCommand(object _)
        {
            if (customWorkArea)
            {
                SetWorkArea(reset: true);
                statusService.SetStatus("Work area reset");
            }
            else
            {
                SetWorkArea(reset: false);
                statusService.SetStatus("Work area set");
            }
        }

        private void SetWorkArea(bool reset = false)
        {
            var original = workAreaService.GetWorkArea(); // Gets working area of primary monitor
            var newBounds = original;

            // Adjust so it fits the second monitor
            newBounds.Left = newBounds.Right;
            newBounds.Right = newBounds.Left + Convert.ToUInt32(System.Windows.SystemParameters.PrimaryScreenWidth);
            if (reset)
            {
                newBounds.Bottom = Convert.ToUInt32(System.Windows.SystemParameters.PrimaryScreenHeight);
            }
            else
            {
                newBounds.Bottom = Convert.ToUInt32(System.Windows.SystemParameters.PrimaryScreenHeight) - Convert.ToUInt32(WINDOW_HEIGHT);
            }
            customWorkArea = !reset;
            workAreaService.SetWorkArea(newBounds);
        }

        private void OnOpenSettingsCommand(object _)
        {
            windowService.ShowWindow(PopupWindow.Settings, settingsViewModel);
        }

        private void OnStatusChange(string message)
        {
            statusMsg = message;
            statusOn = true;

            SongViewerText = message;
            SongViewerTextColor = TextColors.LimeGreen.ToString();
            songChanged = true;
            UpdateSongLabelSize();

            statusTimer.Interval = 1000;
            statusTimer.Elapsed += StatusTimer_Elapsed;
            statusTimer.Start();
            lastSong = "";
        }

        private void StatusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            statusOn = false;
            statusMsg = "";
            statusTimer.Stop();
        }

        private void OnAddShortcutEvent(ShortcutAddedEvent obj)
        {
            var shortcutMetadata = obj.ShortcutMetaData;
            if (!string.IsNullOrWhiteSpace(shortcutMetadata.Name) &&
                !string.IsNullOrWhiteSpace(shortcutMetadata.ProcessPath) &&
                !string.IsNullOrWhiteSpace(shortcutMetadata.WorkingDirectory))
            {
                Shortcuts.Add(new Shortcut(shortcutMetadata.Name, shortcutMetadata.ProcessPath,
                    shortcutMetadata.IconPath, shortcutMetadata.WorkingDirectory));
                SaveShortcuts();
            }
        }

        private void SaveShortcuts()
        {
            try
            {
                shortcutsHandler.SaveToFile(Shortcuts);
            }
            catch (Exception e)
            {
                statusService.SetStatus($"Exception: {e}");
            }
        }

        private void InitTimer()
        {
            uiUpdateTimer.Interval = 1000;
            uiUpdateTimer.Elapsed += UiUpdateTimer_Elapsed;
            uiUpdateTimer.Start();
        }

        private void UiUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateUI();
        }

        private async void InitHue()
        {
            hueSliderTimer.Interval = 50;
            hueSliderTimer.Elapsed += HueSliderTimer_Elapsed;
            HueSliderValue = await hueService.GetBrightnessProcentage();
        }

        private async void HueSliderTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            hueSliderTimer.Stop();
            await hueService.DimLight(HueSliderValue);
        }

        private void HueDimmer_Scroll()
        {
            if (hueSliderTimer != null)
            {
                hueSliderTimer.Stop();
                hueSliderTimer.Start();
            };
        }

        private void UpdateUI()
        {
            UpdateClockDate();
            CpuProgressBarValue = cpuUsageService.NextValue;

            if (statusOn)
            {
                SongViewerText = statusMsg;
            }
            else
            {
                SongViewerTextColor = TextColors.WhiteSmoke.ToString();
                if (songViewerService.IsOnline)
                {
                    UpdateSongviewer();
                }
                else
                {
                    SongViewerText = SONGVIEWER_OFFLINE;
                    songChanged = true;
                    UpdateSongLabelSize();
                }
            }
        }

        private void UpdateSongviewer()
        {
            string song = songViewerService.CurrentSong;

            if (song.Equals(""))
            {
                SongViewerText = "No connection with music player";
                songChanged = true;
            }
            else
            {
                songChanged = !song.Equals(lastSong);
                SongViewerText = song;
            }
            UpdateSongLabelSize();
        }

        private void UpdateSongLabelSize()
        {
            string currentSong = songViewerService.CurrentSong;
            if (songChanged)
            {
                songChanged = false;
            }

            lastSong = currentSong;
        }

        private void UpdateClockDate()
        {
            string date =
                DateTime.Now.ToShortTimeString() +
                Environment.NewLine +
                DateTime.Now.TranslateDayToSwedish() +
                Environment.NewLine +
                DateTime.Now.ToShortDateString();
            if (!DateText.Equals(date))
            {
                DateText = date;
            }
        }
    }
}