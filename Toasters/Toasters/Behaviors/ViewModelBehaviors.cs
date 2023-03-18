using Avalonia;
using Avalonia.Xaml.Interactions.Custom;
using Toasters.ViewModels;

namespace Toasters.Behaviors;

public abstract class ViewModelBehaviors<TViewModel, TControl> : DisposingBehavior<TControl>
    where TControl :  AvaloniaObject
    where TViewModel : ViewModelBase
{
    private TViewModel? _targetViewModel;

    public static readonly DirectProperty<ViewModelBehaviors<TViewModel, TControl>, TViewModel?>
        TargetViewModelProperty =
            AvaloniaProperty.RegisterDirect<ViewModelBehaviors<TViewModel, TControl>, TViewModel?>(
                "TargetViewModel", o => o.TargetViewModel, (o, v) => o.TargetViewModel = v);

    public TViewModel? TargetViewModel
    {
        get => _targetViewModel;
        set => SetAndRaise(TargetViewModelProperty, ref _targetViewModel, value);
    }
}