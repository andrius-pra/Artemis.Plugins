﻿using System;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.LayerBrushes.Ambilight.ScreenCapture;
using Microsoft.Win32;
using ScreenCapture.NET;
using Serilog;

namespace Artemis.Plugins.LayerBrushes.Ambilight
{
    public class AmbilightBootstrapper : PluginBootstrapper
    {
        private ILogger? _logger;
        private IPluginManagementService? _managementService;
        private PluginFeatureInfo? _brushProvider;

        #region Properties & Fields

        internal static IScreenCaptureService? ScreenCaptureService { get; private set; }

        #endregion

        #region Methods

        public override void OnPluginEnabled(Plugin plugin)
        {
            _logger = plugin.Resolve<ILogger>();
            _managementService = plugin.Resolve<IPluginManagementService>();
            _brushProvider = plugin.GetFeatureInfo<AmbilightLayerBrushProvider>();

            IScreenCaptureService screenCaptureService = OperatingSystem.IsWindows() ? new DX11ScreenCaptureService() : new X11ScreenCaptureService();
            ScreenCaptureService ??= new AmbilightScreenCaptureService(screenCaptureService);
            SystemEvents.DisplaySettingsChanged += SystemEventsOnDisplaySettingsChanged;
        }

        public override void OnPluginDisabled(Plugin plugin)
        {
            ScreenCaptureService?.Dispose();
            ScreenCaptureService = null;
            SystemEvents.DisplaySettingsChanged -= SystemEventsOnDisplaySettingsChanged;
        }

        private void SystemEventsOnDisplaySettingsChanged(object? sender, EventArgs e)
        {
            if (_brushProvider?.Instance == null || !_brushProvider.Instance.IsEnabled)
                _logger?.Debug("Display settings changed, but ambilight feature is disabled");
            else
            {
                _logger?.Debug("Display settings changed, restarting ambilight feature");

                _managementService?.DisablePluginFeature(_brushProvider.Instance, false);
                ScreenCaptureService?.Dispose();
                IScreenCaptureService screenCaptureService = OperatingSystem.IsWindows() ? new DX11ScreenCaptureService() : new X11ScreenCaptureService();
                ScreenCaptureService = new AmbilightScreenCaptureService(screenCaptureService);
                _managementService?.EnablePluginFeature(_brushProvider.Instance, false);
            }
        }

        #endregion
    }
}