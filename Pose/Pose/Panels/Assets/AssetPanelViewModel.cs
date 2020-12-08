using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pose.Domain.Documents.Messages;
using Pose.Domain.Editor;
using Pose.Domain.Editor.Messages;
using Pose.Framework;
using Pose.Framework.Messaging;
using Pose.Framework.ViewModels;

namespace Pose.Panels.Assets
{
    public class AssetPanelViewModel
    : ViewModel
    {
        private readonly IUiThreadDispatcher _uiThreadDispatcher;
        private readonly IAssetScanner _assetScanner;
        private readonly IAssetViewModelBuilder _assetViewModelBuilder;
        private readonly IAssetFolderWatcherFactory _assetFolderWatcherFactory;
        private readonly Editor _editor;
        private string _assetFolder;
        private FileSystemWatcher _assetFolderWatcher;
        private bool _isViewActive;
        private bool _sourcesDirty;
        private ObservableCollection<SpriteViewModel> _sprites;
        private SpriteViewModel _selectedSprite;
        private string _setFolderButtonTooltip;

        public AssetPanelViewModel(
            IUiThreadDispatcher uiThreadDispatcher,
            IAssetScanner assetScanner,
            IAssetViewModelBuilder assetViewModelBuilder,
            IAssetFolderWatcherFactory assetFolderWatcherFactory,
            Editor editor)
        {
            _uiThreadDispatcher = uiThreadDispatcher;
            _assetScanner = assetScanner;
            _assetViewModelBuilder = assetViewModelBuilder;
            _assetFolderWatcherFactory = assetFolderWatcherFactory;
            _editor = editor;

            MessageBus.Default.Subscribe<DocumentLoaded>(OnProjectLoadedEvent);
            MessageBus.Default.Subscribe<AssetFolderChanged>(OnAssetFolderChanged);
            MessageBus.Default.Subscribe<UserInterfaceReady>(OnViewLoadedEvent);
            MessageBus.Default.Subscribe<ViewActivatedEvent>(OnViewActivatedEvent);
            MessageBus.Default.Subscribe<ViewActivatedEvent>(OnViewDeactivatedEvent);

            UpdateSetFolderButtonTooltip();
        }

        public void DoSetAssetFolderWorkflow()
        {
            var selectedPath = _editor.HasDocument ? _editor.CurrentDocument.AssetFolder : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var dialog = new FolderBrowserDialog
            {
                SelectedPath = selectedPath,
                Description = "Pick the folder where you keep your images.",
                ShowNewFolderButton = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _editor.SetDocumentAssetFolder(dialog.SelectedPath);
            }
        }

        public void SpriteDoubleClick()
        {
            if (SelectedSprite != null)
            {
                _editor.AddSpriteNode(SelectedSprite.Label, SelectedSprite.Sprite);
            }
        }

        private void OnViewLoadedEvent(UserInterfaceReady obj)
        {
            _isViewActive = true;
        }

        private async void OnViewActivatedEvent(ViewActivatedEvent obj)
        {
            if (_sourcesDirty)
                await RefreshAssetsAsync();
            _isViewActive = true;
        }

        private void OnViewDeactivatedEvent(ViewActivatedEvent obj)
        {
            _isViewActive = false;
        }

        private async void OnProjectLoadedEvent(DocumentLoaded e)
        {
            await ChangeAssetFolderAsync(e.Document.AssetFolder);
        }

        private async void OnAssetFolderChanged(AssetFolderChanged msg)
        {
            await ChangeAssetFolderAsync(msg.Path);
        }

        private async Task ChangeAssetFolderAsync(string path)
        {
            if (_assetFolder != path)
            {
                _assetFolderWatcher?.Dispose();
                _assetFolderWatcher = null;
                Sprites = null;

                _assetFolder = path;
                UpdateSetFolderButtonTooltip();
                await StartShowAssetFolderContent();
            }
        }

        private void UpdateSetFolderButtonTooltip()
        {
            SetFolderButtonTooltip = _assetFolder == null ? "Set asset folder" : "Assetfolder: " + _assetFolder;
        }

        /// <summary>
        /// Shows the content of the assetfolder, and starts watching it for file changes.
        /// </summary>
        /// <returns></returns>
        private async Task StartShowAssetFolderContent()
        {
            if (_assetFolder == null)
                return;

            await RefreshAssetsAsync();
            _assetFolderWatcher?.Dispose();
            _assetFolderWatcher = _assetFolderWatcherFactory.Create(_assetFolder, MarkAssetsDirty);
        }

        private async Task RefreshAssetsAsync()
        {
            var assets = await _assetScanner.Scan(_assetFolder);
            Sprites = new ObservableCollection<SpriteViewModel>(assets.Select(_assetViewModelBuilder.Build));
            _sourcesDirty = false;
        }

        private async void MarkAssetsDirty()
        {
            await _uiThreadDispatcher.InvokeAsync(async () =>
            {
                if (!_isViewActive)
                {
                    _sourcesDirty = true;
                }
                else
                {
                    await RefreshAssetsAsync();
                }
            });
        }

        public ObservableCollection<SpriteViewModel> Sprites
        {
            get => _sprites;
            set
            {
                if (value == _sprites) return;
                _sprites = value;
                OnPropertyChanged();
            }
        }

        public SpriteViewModel SelectedSprite
        {
            get => _selectedSprite;
            set
            {
                if (value == _selectedSprite) return;
                _selectedSprite = value;
                OnPropertyChanged();
            }
        }

        public string SetFolderButtonTooltip
        {
            get => _setFolderButtonTooltip;
            set
            {
                if (value == _setFolderButtonTooltip) return;
                _setFolderButtonTooltip = value;
                OnPropertyChanged();
            }
        }
    }
}
