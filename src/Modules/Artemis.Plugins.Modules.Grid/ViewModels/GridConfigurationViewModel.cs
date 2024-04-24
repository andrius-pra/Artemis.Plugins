#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.Modules.Grid.Models;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using OpenRGB.NET;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Artemis.UI.Screens.Device;
using System;
using RGB.NET.Core;
using System.IO;
using System.Text.Json;
using Color = RGB.NET.Core.Color;
using Artemis.Plugins.Modules.Grid.Services;

namespace Artemis.Plugins.Modules.Grid.ViewModels
{
    public class GridConfigurationViewModel : PluginConfigurationViewModel
    {
        //private readonly PluginSettings settings;

        private readonly PluginSetting<int> _rowsSettings;
        private readonly PluginSetting<int> _columnsSetting;
        private readonly PluginSetting<List<DeviceConfigModel>> _devicesSetting;       

        private readonly INotificationService notificationService;
        private readonly IInputService inputService;
        private readonly IDeviceService _deviceService;
        private readonly IWindowService _windowService;
        private readonly IRenderService _renderService;
        private readonly IOpenRgbVisualMapService _openRgbVisualMapService;
        private int _wizardPage;
        private bool _busy;
        private bool _isArrangeInProgress;

        public ReactiveCommand<Unit, Unit> AutoArrange { get; }
        public ReactiveCommand<Unit, Unit> DetectInput { get; }
        public ReactiveCommand<Unit, Unit> Debug { get; }
        public ReactiveCommand<Unit, Unit> StartArrange { get; }
        public ReactiveCommand<Unit, Unit> StopArrange { get; }
        public ReactiveCommand<Unit, Unit> ExportOpenRgbVisualMapConfig { get; }

        private Dictionary<IRGBDevice, ListLedGroup> _deviceLedGroups = new Dictionary<IRGBDevice, ListLedGroup>();

        public int Index { get; set; }
        //public int Row => Index / 5;
        //public int Column => Index % 5;


        private int _rows;
        public int Rows
        {
            get => _rows;
            set => RaiseAndSetIfChanged(ref _rows, value);
        }

        private int _columns;
        public int Columns
        {
            get => _columns;
            set => RaiseAndSetIfChanged(ref _columns, value);
        }


        public GridConfigurationViewModel(
            Artemis.Core.Plugin plugin,
            PluginSettings settings,
            INotificationService notificationService,
            IInputService inputService,
            IDeviceService deviceService,
            IWindowService windowService,
            IRenderService renderService,
            IOpenRgbVisualMapService openRgbVisualMapService) : base(plugin)
        {
            _rowsSettings = settings.GetSetting("Rows", 12);
            _columnsSetting = settings.GetSetting("Columns", 6);
            _devicesSetting = settings.GetSetting("devices", new List<DeviceConfigModel>());


            Rows = _rowsSettings.Value;
            Columns = _columnsSetting.Value;

            //this.settings = settings;
            this.notificationService = notificationService;
            this.inputService = inputService;
            this._deviceService = deviceService;
            this._windowService = windowService;
            this._renderService = renderService;
            this._openRgbVisualMapService = openRgbVisualMapService;
            AutoArrange = ReactiveCommand.CreateFromTask(ExecuteAutoArrange, this.WhenAnyValue(vm => vm.Busy).Select(l => !l));
            DetectInput = ReactiveCommand.CreateFromTask(ExecuteDetectInput, this.WhenAnyValue(vm => vm.IsArrangeInProgress).Select(l => !l));
            Debug = ReactiveCommand.CreateFromTask(ExecuteDebug, this.WhenAnyValue(vm => vm.IsArrangeInProgress).Select(l => !l));
            StartArrange = ReactiveCommand.CreateFromTask(ExecuteStartArrange, this.WhenAnyValue(vm => vm.IsArrangeInProgress).Select(l => !l));
            StopArrange = ReactiveCommand.CreateFromTask(ExecuteStopArrange, this.WhenAnyValue(vm => vm.IsArrangeInProgress).Select(l =>l));
            ExportOpenRgbVisualMapConfig = ReactiveCommand.CreateFromTask(_openRgbVisualMapService.ExportSettings, this.WhenAnyValue(vm => vm.IsArrangeInProgress, vm => vm.IsArrangeInProgress).Select((item) => !item.Item1 && !item.Item2));

            this.WhenAnyValue(vm => vm.Rows).Subscribe(SaveRows);
            this.WhenAnyValue(vm => vm.Columns).Subscribe(SaveColumns);


            this._windowService = windowService;
        }

        public int WizardPage
        {
            get => _wizardPage;
            set => RaiseAndSetIfChanged(ref _wizardPage, value);
        }

        public bool Busy
        {
            get => _busy;
            set => RaiseAndSetIfChanged(ref _busy, value);
        }

        public bool IsArrangeInProgress
        {
            get => _isArrangeInProgress;
            set => RaiseAndSetIfChanged(ref _isArrangeInProgress, value);
        }

        public void OnActivated()
        {
            UpdateSettings();
        }

        private static object GetPropValue<T>(T src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        private async Task ExecuteAutoArrange()
        {
            if (Busy)
                return;

            Busy = true;
        }

        private async Task ExecuteStartArrange()
        {
            IsArrangeInProgress = true;
            inputService.KeyboardKeyDown += InputServiceOnKeyboardKeyDown;
        }
        
        private async Task ExecuteStopArrange()
        {
            IsArrangeInProgress = false;
            Index = 0;
            inputService.KeyboardKeyDown -= InputServiceOnKeyboardKeyDown;

            foreach (ListLedGroup group in _deviceLedGroups.Values)
            {
                group.Detach();
            }
            _deviceLedGroups = new Dictionary<IRGBDevice, ListLedGroup>();
        }
        
        private async Task ExecuteDebug()
        {
            string fileName = "settings.json";
            string? result = await _windowService.CreateSaveFileDialog()
                .HavingFilter(f => f.WithExtension("json").WithName("JSON"))
                .WithInitialFileName(fileName)
                .ShowAsync();
            
            if(result != null)
            {
                List<DeviceConfigModel> values = _devicesSetting.Value.OrderBy(x => x.Y).ThenBy(x => x.X).ToList();
                File.WriteAllText(result, JsonSerializer.Serialize(values, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            }
        }

    
        private void InputServiceOnKeyboardKeyDown(object sender, ArtemisKeyboardKeyEventArgs e)
        {
            if (e.Device == null || _deviceLedGroups.ContainsKey(e.Device.RgbDevice)) 
                return;

            int columnIndex = Index / this.Columns;
            int rowIndex = Index % this.Columns;

            e.Device.X = rowIndex * e.Device.Rectangle.Width;
            e.Device.Y = columnIndex * e.Device.Rectangle.Height;
  
            ListLedGroup ledGroup = new ListLedGroup(_renderService.Surface, e.Device.Leds.Select(l => l.RgbLed))
            {
                Brush = new SolidColorBrush(new Color(255, 255, 0)),
                ZIndex = 999
            };
            _deviceService.SaveDevice(e.Device);
            _deviceLedGroups.Add(e.Device.RgbDevice, ledGroup);;
            Index++;
        }

        private async Task ExecuteDetectInput()
        {
            HashSet<string> identifierSet = [];
            foreach (ArtemisDevice device in this._deviceService.Devices)
            {
                if (device.DeviceType != RGBDeviceType.Keyboard)
                    continue;

  
                do
                {
                    try
                    {
                        await _windowService.CreateContentDialog()
                            .WithTitle($"{device.RgbDevice.DeviceInfo.DeviceName} - Detect input")
                            .WithViewModel(out DeviceDetectInputViewModel viewModel, device)
                            .WithCloseButtonText("Cancel")
                            .ShowAsync();

                        string identifier = device.InputIdentifiers.FirstOrDefault()?.Identifier?.ToString();;
                        if (device.InputIdentifiers.Count == 1 && !identifierSet.Contains(identifier))
                        {
                            identifierSet.Add(identifier);
                            _deviceService.SaveDevice(device);
                        }
                        else
                        {
                            device.InputIdentifiers.Clear();
                        }
                    }
                    catch (Exception w)
                    {
                    }
                } while (device.InputIdentifiers.Count == 0);




            }

            UpdateSettings();
        }


        private void UpdateSettings()
        {           
            foreach (ArtemisDevice device in _deviceService.Devices)
            {
                string type = device.RgbDevice.DeviceInfo.GetType().FullName;

                if (type == "RGB.NET.Devices.OpenRGB.OpenRGBDeviceInfo")
                {
                    object openRgbDevice = GetPropValue(device.RgbDevice.DeviceInfo, "OpenRGBDevice");
                    string rgbDeviceLocation = (string)GetPropValue(openRgbDevice, "Location");

                    DeviceConfigModel settingsModel = _devicesSetting.Value.FirstOrDefault(x => x.RgbDeviceLocation == rgbDeviceLocation);

                    if (settingsModel == null)
                    {
                        settingsModel = new DeviceConfigModel { };
                        _devicesSetting.Value.Add(settingsModel);
                    }

                    settingsModel.RgbDeviceLocation = rgbDeviceLocation;
                    settingsModel.InputDeviceLocation = device.InputIdentifiers.FirstOrDefault()?.Identifier?.ToString();
                    settingsModel.X = device.X;
                    settingsModel.Y = device.Y;
                }
            }

            _devicesSetting.Save();
        }

        private void SaveRows(int rows)
        {
            if (!ValidationContext.IsValid)
                return;

            _rowsSettings.Value = rows;
            _rowsSettings.Save();
        }

        private void SaveColumns(int columns)
        {
            if (!ValidationContext.IsValid)
                return;

            _columnsSetting.Value = columns;
            _columnsSetting.Save();
        }
    }
}