﻿using System.Threading;
using System.Threading.Tasks;
using BeatLeader.Interop;
using BeatLeader.Models;
using BeatLeader.Models.Replay;
using BeatLeader.Replayer;
using BeatLeader.Utils;
using BeatSaberMarkupLanguage.Attributes;
using JetBrains.Annotations;
using UnityEngine;
using static BeatLeader.Models.FileStatus;

namespace BeatLeader.Components {
    internal class ReplayDetailPanel : ReeUIComponentV2 {
        #region Configuration

        private const string WatchText = "Watch";
        private const string DownloadText = "Download map";

        #endregion

        #region UI Components

        [UIValue("replay-info-panel"), UsedImplicitly]
        private ReplayStatisticsPanel _replayStatisticsPanel = null!;

        [UIValue("download-beatmap-panel"), UsedImplicitly]
        private DownloadBeatmapPanel _downloadBeatmapPanel = null!;

        [UIValue("player-info-panel"), UsedImplicitly]
        private HorizontalMiniProfileContainer _miniProfile = null!;

        #endregion

        #region Action Buttons

        [UIValue("watch-button-text"), UsedImplicitly]
        private string? WatchButtonText {
            get => _watchButtonText;
            set {
                _watchButtonText = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("watch-button-interactable"), UsedImplicitly]
        private bool WatchButtonInteractable {
            get => _watchButtonInteractable;
            set {
                _watchButtonInteractable = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("delete-button-interactable"), UsedImplicitly]
        private bool DeleteButtonInteractable {
            get => _deleteButtonInteractable;
            set {
                _deleteButtonInteractable = value;
                NotifyPropertyChanged();
            }
        }

        private bool _deleteButtonInteractable;
        private bool _watchButtonInteractable;
        private string? _watchButtonText;

        #endregion

        #region Init

        private ReplayerMenuLoader? _menuLoader;
        private bool _isInitialized;

        public void Setup(ReplayerMenuLoader loader) {
            _menuLoader = loader;
            _isInitialized = true;
        }

        protected override void OnInstantiate() {
            _replayStatisticsPanel = Instantiate<ReplayStatisticsPanel>(transform);
            _downloadBeatmapPanel = Instantiate<DownloadBeatmapPanel>(transform);
            _miniProfile = Instantiate<HorizontalMiniProfileContainer>(transform);

            _replayStatisticsPanel.SetData(null, null, true, true);
            _miniProfile.SetPlayer(null);

            _miniProfile.PlayerLoadedEvent += HandlePlayerLoaded;
            _downloadBeatmapPanel.BackButtonClickedEvent += HandleDownloadMenuBackButtonClicked;
            _downloadBeatmapPanel.DownloadAbilityChangedEvent += HandleDownloadAbilityChangedEvent;

            SetDownloadPanelActive(false);
        }

        #endregion

        #region Download Panel

        private void SetDownloadPanelActive(bool active) {
            _downloadBeatmapPanel.SetRootActive(active);
            _replayStatisticsPanel.SetRootActive(!active);
            _isIntoDownloadMenu = active;
        }

        #endregion

        #region Data

        private CancellationTokenSource _cancellationTokenSource = new();
        private IReplayHeader? _header;
        private Player? _player;

        private bool _beatmapIsMissing;
        private bool _isIntoDownloadMenu;
        private bool _isWorking;

        public void SetData(IReplayHeader? header) {
            if (!_isInitialized) return;
            if (_isWorking) {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = new();
            }
            if (_isIntoDownloadMenu) SetDownloadPanelActive(false);
            _isWorking = true;
            var invalid = header is null || header.FileStatus is Corrupted;
            _replayStatisticsPanel.SetData(null, null, invalid, header is null);
            _header = header;
            DeleteButtonInteractable = header is not null;
            WatchButtonInteractable = false;
            if (!invalid) {
                _ = ProcessDataAsync(header!, _cancellationTokenSource.Token);
                return;
            }
            _miniProfile.SetPlayer(null);
            _isWorking = false;
        }

        private async Task ProcessDataAsync(IReplayHeader header, CancellationToken token) {
            _miniProfile.SetPlayer(header.ReplayInfo?.playerID);
            DeleteButtonInteractable = false;
            var replay = await header.LoadReplayAsync(default);
            if (token.IsCancellationRequested) return;
            DeleteButtonInteractable = true;
            var stats = default(ScoreStats?);
            var score = default(Score?);
            if (replay is not null) {
                await Task.Run(() => stats = ReplayStatisticUtils.ComputeScoreStats(replay), token);
                score = ReplayUtils.ComputeScore(replay);
            }
            if (token.IsCancellationRequested) return;
            _replayStatisticsPanel.SetData(score, stats, score is null || stats is null);
            await RefreshAvailabilityAsync(header.ReplayInfo!, token);
        }

        private async Task RefreshAvailabilityAsync(ReplayInfo info, CancellationToken token) {
            var beatmap = await _menuLoader!.LoadBeatmapAsync(
                info.hash, info.mode, info.difficulty, token);
            if (token.IsCancellationRequested) return;
            var invalid = beatmap is null;
            WatchButtonText = invalid ? DownloadText : WatchText;
            WatchButtonInteractable = invalid || SongCoreInterop.ValidateRequirements(beatmap!);
            _beatmapIsMissing = invalid;
            if (invalid) _downloadBeatmapPanel.SetHash(info.hash);
            _isWorking = false;
        }

        #endregion

        #region Callbacks

        private void HandlePlayerLoaded(Player player) {
            _player = player;
        }

        private void HandleDownloadAbilityChangedEvent(bool ableToDownload) {
            WatchButtonInteractable = ableToDownload;
        }

        private void HandleDownloadMenuBackButtonClicked() {
            SetDownloadPanelActive(false);
            _isIntoDownloadMenu = false;
            _ = RefreshAvailabilityAsync(_header!.ReplayInfo!, _cancellationTokenSource.Token);
        }
        
        [UIAction("delete-button-click"), UsedImplicitly]
        private void HandleDeleteButtonClicked() {
            _header?.DeleteReplayAsync(default);
        }

        [UIAction("watch-button-click"), UsedImplicitly]
        private void HandleWatchButtonClicked() {
            if (_isIntoDownloadMenu) {
                _downloadBeatmapPanel.NotifyDownloadButtonClicked();
                return;
            }
            if (_beatmapIsMissing) {
                SetDownloadPanelActive(true);
                return;
            }
            if (!_isInitialized || _header is null || _header.FileStatus is Corrupted) return;
            _ = _menuLoader!.StartReplayAsync(_header.LoadReplayAsync(default).Result!, _player);
        }

        #endregion
    }
}