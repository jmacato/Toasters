using System.Reactive.Disposables;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Xaml.Interactivity;
using Toasters.Utils;
using System;
using System.Reactive.Linq;
using Toasters.ViewModels;

namespace Toasters.Behaviors;

public class BoundsBehavior : ViewModelBehaviors<MainViewModel, UserControl>
{
    protected override void OnAttached(CompositeDisposable disposables)
    {
        AssociatedObject
            ?.WhenAnyValue(x => x.Bounds)
            .Subscribe(u =>
            {
                if (TargetViewModel is { })
                    TargetViewModel.ViewBounds = new Models.Size((float)u.Size.Width, (float)u.Size.Height);
            }).DisposeWith(disposables);
    }
}