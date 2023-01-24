using System;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Toasters.Utils;
using Toasters.ViewModels;

namespace Toasters.Behaviors;

public class FlyingObjectFrameBehavior : ViewModelBehaviors<FlyingObjectsViewModel, Image>
{
    protected override void OnAttached(CompositeDisposable disposables)
    {
        this.WhenAnyValue(x => x.TargetViewModel)
            .Subscribe(x =>
            {
                if (AssociatedObject is null || x is null) return;

                x.WhenAnyValue(z => z.State).Subscribe(y =>
                {
                    AssociatedObject.Classes.Clear();
                    AssociatedObject.Classes.Add(x.State.ToString());
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);
    }
}