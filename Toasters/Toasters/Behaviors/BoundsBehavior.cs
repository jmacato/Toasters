using System.Reactive.Disposables;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;
using Toasters.Utils;
using System;
using System.Reactive.Linq;
using Avalonia;
using Toasters.ViewModels;

namespace Toasters.Behaviors;

public abstract class ViewModelBehaviors<TViewModel, TControl> : DisposingBehavior<TControl> 
    where TControl : class, IAvaloniaObject 
    where TViewModel : ViewModelBase
{
    private TViewModel? _targetViewModel;

    public static readonly DirectProperty<ViewModelBehaviors<TViewModel, TControl>, TViewModel?> TargetViewModelProperty =
        AvaloniaProperty.RegisterDirect<ViewModelBehaviors<TViewModel, TControl>, TViewModel?>(
            "TargetViewModel", o => o.TargetViewModel, (o, v) => o.TargetViewModel = v);

    public TViewModel? TargetViewModel
    {
        get => _targetViewModel;
        set => SetAndRaise(TargetViewModelProperty, ref _targetViewModel, value);
    }
}

public class BoundsBehavior : ViewModelBehaviors<MainViewModel, UserControl>
{
    protected override void OnAttached(CompositeDisposable disposables)
    {
        AssociatedObject
            ?.WhenAnyValue(x => x.Bounds)
            .Subscribe(u =>
            {
                if (TargetViewModel is { })
                    TargetViewModel.ViewBounds = new Toasters.Models.Size((float)u.Size.Width, (float)u.Size.Height);
            }).DisposeWith(disposables);
    }
}

public class ToasterFrameBehavior : ViewModelBehaviors<ToasterViewModel, Image>
{
    protected override void OnAttached(CompositeDisposable disposables)
    {
        this.WhenAnyValue(x => x.TargetViewModel)
            .Subscribe(x =>
            {
                if (AssociatedObject is null || x is null) return;

                x.WhenAnyValue(z => z.State).Subscribe(y =>
                {
                    switch (y)
                    {
                        case ToasterViewModel.ToasterState.WingLow1:
                        case ToasterViewModel.ToasterState.WingLow2:
                        case ToasterViewModel.ToasterState.WingHigh1:
                        case ToasterViewModel.ToasterState.WingHigh2:
                            AssociatedObject.Classes.Clear();
                            AssociatedObject.Classes.Add(x.State.ToString());
                            break;
                    }
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);
    }
}