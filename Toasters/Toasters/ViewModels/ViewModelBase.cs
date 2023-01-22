using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Toasters.Models;

namespace Toasters.ViewModels
{
    public class ViewModelBase : ObservableObject
    {
    }

    public abstract class FlyingObjectsViewModel : RectangleViewModel, IDisposable
    {
        private IDisposable _disposable;
        public abstract void Tick();

        protected Quadtree Tree { get; }

        public FlyingObjectsViewModel(Quadtree tree, TimeSpan interval, Vector position, Size size) : base(position,
            size)
        {
            Tree = tree;
            Tree.Insert(this);
            _disposable = Observable.Interval(interval).ObserveOn(AvaloniaScheduler.Instance).Subscribe(_ => Tick());
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            _disposable.Dispose();
        }
    }



}