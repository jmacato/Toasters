using System;
using System.Reactive.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Toasters.Models;

namespace Toasters.ViewModels;

public abstract partial class FlyingObjectsViewModel : RectangleViewModel, IDisposable
{
    private IDisposable _disposable;

    public enum FlyingObjectState
    {
        WingSlow1,
        WingSlow2,
        WingFast1,
        WingFast2,
        Bread
    }

    [ObservableProperty] private FlyingObjectState _state;

    public abstract void Tick();

    protected QuadTree Tree { get; }

    public FlyingObjectsViewModel(QuadTree tree, TimeSpan interval, Vector position, Size size) :
        base(position,
            size)
    {
        Tree = tree;
        Tree.Insert(this);
       // _disposable = Observable.Interval(interval).ObserveOn(AvaloniaScheduler.Instance).Subscribe(_ => Tick());
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _disposable?.Dispose();
    }
}