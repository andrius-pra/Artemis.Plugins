using System;
using System.Reactive.Disposables;
using Artemis.Plugins.Modules.Grid.ViewModels;
using Artemis.Plugins.Modules.Grid.Views.Steps;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Navigation;
using ReactiveUI;

namespace Artemis.Plugins.Modules.Grid.Views;

public partial class GridConfigurationView : ReactiveUserControl<GridConfigurationViewModel>
{
    public GridConfigurationView()
    {
        InitializeComponent();
        this.WhenActivated(d => OnActivated(d));
    }


    private void ApplyWizardPage(int page)
    {
        switch (page)
        {
            case 0:
                ConfigurationFrame.NavigateToType(typeof(WelcomeStep), null, new FrameNavigationOptions());
                break;
        }
    }

    private void OnActivated(CompositeDisposable d)
    {
        ViewModel.OnActivated();
        ViewModel.WhenAnyValue(vm => vm.WizardPage).Subscribe(ApplyWizardPage).DisposeWith(d);
    }
}
