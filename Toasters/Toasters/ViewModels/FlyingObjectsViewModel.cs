using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Toasters.Models;
using Vector = Toasters.Models.Vector;

namespace Toasters.ViewModels;

public abstract partial class FlyingObjectsViewModel : RectangleViewModel, IDisposable
{
    private readonly IDisposable _disposable;

    public enum FlyingObjectState
    {
        WingSlow1,
        WingSlow2,
        WingFast1,
        WingFast2,
        Bread
    }

    [ObservableProperty] private FlyingObjectState _state;

    protected abstract void Tick();
    private static readonly object FlyingObjLock = new ();
    private readonly QuadTree _tree;
    private readonly MainViewModel _mainViewModel;

    protected Vector Velocity { get; set; }

    protected FlyingObjectsViewModel(MainViewModel mainViewModel, TimeSpan interval, Vector position, Size size,
        Vector velocity) :
        base(position,
            size)
    {
        _mainViewModel = mainViewModel;
        Velocity = velocity;
        _tree = mainViewModel.Tree;
        _tree.Insert(this);
        _disposable = Observable.Interval(interval).ObserveOn(AvaloniaScheduler.Instance).Subscribe(HandleTick);
    }

    private void HandleTick(long _)
    {
        lock (FlyingObjLock)
        {
            Tick();

            var curVelocity = Velocity;

            Location += curVelocity;

            if (_tree.Query(this).Count() > 1)
            {
                Location -= curVelocity;

                Location += new Vector(0, curVelocity.Y);

                if (_tree.Query(this).Count() > 1)
                {
                    Location -= new Vector(0, curVelocity.Y);
                }
            }

            if (Location.X < 0 && Location.Y >= _mainViewModel.ViewBounds.Height)
            {
                _mainViewModel.ObjectOutOfBounds(this);
                return;
            }

            _tree.Update(this);
        }
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _tree.Remove(this);
        _disposable.Dispose();
    }
}