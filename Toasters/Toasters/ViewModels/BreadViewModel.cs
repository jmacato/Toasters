using System;
using System.Linq;
using Toasters.Models;

namespace Toasters.ViewModels;

public class BreadViewModel : FlyingObjectsViewModel
{
    public BreadViewModel(MainViewModel mainViewModel, Vector position) : base(mainViewModel,
        TimeSpan.FromMilliseconds(20), position, new Size(64, 64), new(-1, 1))
    {
        State = FlyingObjectState.Bread;
    }

    protected override void Tick()
    {
    }
}