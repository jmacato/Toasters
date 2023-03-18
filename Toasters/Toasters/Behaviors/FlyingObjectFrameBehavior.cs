using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Toasters.Utils;
using Toasters.ViewModels;

namespace Toasters.Behaviors;

public class FlyingObjectFrameBehavior : ViewModelBehaviors<FlyingObjectsViewModel, Image>
{
    protected override void OnAttached(CompositeDisposable disposables)
    {
        PropertyChangedExtensions.WhenAnyValue(this, x => x.TargetViewModel)
            .Subscribe(x =>
            {
                if (AssociatedObject is null || x is null) return;

                PropertyChangedExtensions.WhenAnyValue(x, z => z.State).ObserveOn(AvaloniaScheduler.Instance).Subscribe(y =>
                {
                    AssociatedObject.Classes.Clear();
                    AssociatedObject.Classes.Add(x.State.ToString());
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);
    }
}