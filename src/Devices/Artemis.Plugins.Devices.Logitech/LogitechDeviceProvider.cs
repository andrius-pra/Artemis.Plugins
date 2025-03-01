﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using HidSharp;
using Microsoft.Win32;
using RGB.NET.Core;
using Serilog;
using Serilog.Events;
using RGBDeviceProvider = RGB.NET.Devices.Logitech.LogitechDeviceProvider;

namespace Artemis.Plugins.Devices.Logitech
{
    public class LogitechDeviceProvider : DeviceProvider
    {
        private const int VENDOR_ID = 0x046D;
        private readonly ILogger _logger;
        private readonly IPluginManagementService _pluginManagementService;
        private readonly Plugin _plugin;
        private readonly IRgbService _rgbService;

        public LogitechDeviceProvider(IRgbService rgbService, ILogger logger, IPluginManagementService pluginManagementService, Plugin plugin)
        {
            _rgbService = rgbService;
            _logger = logger;
            _pluginManagementService = pluginManagementService;
            _plugin = plugin;
        }

        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

        public override void Enable()
        {
            RGBDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "LogitechLedEnginesWrapper.dll"));
            RGBDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "LogitechLedEnginesWrapper.dll"));

            RgbDeviceProvider.Exception += Provider_OnException;
            _rgbService.AddDeviceProvider(RgbDeviceProvider);

            if (_logger.IsEnabled(LogEventLevel.Debug))
                LogDeviceIds();

            Subscribe();
        }

        public override void Disable()
        {
            Unsubscribe();

            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Exception -= Provider_OnException;
            RgbDeviceProvider.Dispose();
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Logitech Exception: {message}", args.Exception.Message);

        private void LogDeviceIds()
        {
            List<HidDevice> devices = DeviceList.Local.GetHidDevices(VENDOR_ID).DistinctBy(d => d.ProductID).ToList();
            _logger.Debug("Found {count} Logitech device(s)", devices.Count);
            foreach (HidDevice hidDevice in devices)
            {
                try
                {
                    _logger.Debug("Found Logitech device {name} with PID 0x{pid}", hidDevice.GetFriendlyName(), hidDevice.ProductID.ToString("X"));
                }
                catch (Exception)
                {
                    _logger.Debug("Found Logitech device with PID 0x{pid}", hidDevice.ProductID.ToString("X"));
                }
            }
        }

        #region Event handlers

        private void Subscribe()
        {
            Thread thread = new(() =>
            {
                try
                {
                    SystemEvents.SessionSwitch += SystemEventsOnSessionSwitch;
                }
                catch (Exception e)
                {
                    _logger.Warning(e, "Could not subscribe to SessionSwitch");
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void Unsubscribe()
        {
            Thread thread = new(() => SystemEvents.SessionSwitch -= SystemEventsOnSessionSwitch);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void SystemEventsOnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (!IsEnabled || e.Reason != SessionSwitchReason.SessionUnlock)
                return;

            Task.Run(async () =>
            {
                // Disable the plugin
                _logger.Debug("Detected PC unlock, reloading Logitech plugin");
                _pluginManagementService.DisablePlugin(_plugin, false);

                // Enable the plugin with the management service, allowing retries 
                await Task.Delay(5000);
                _pluginManagementService.EnablePlugin(_plugin, false);
            });
        }

        #endregion
    }
}