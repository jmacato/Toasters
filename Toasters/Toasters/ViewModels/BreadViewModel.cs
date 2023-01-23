using System;
using System.Linq;
using Toasters.Models;

namespace Toasters.ViewModels;

public class BreadViewModel : FlyingObjectsViewModel
{
    private readonly Vector _velocity = new(-1, 1);

    public BreadViewModel(MainViewModel mainViewModel, Vector position) : base(mainViewModel.Tree,
        TimeSpan.FromMilliseconds(20), position, new Size(64, 64))
    {
        State = FlyingObjectState.Bread;
    }

    public override void Tick()
    {
        var curVelocity = _velocity;

        Location += curVelocity;

        if (Tree.Query(this).Count() > 1)
        {
            Location -= curVelocity;

            Location += new Vector(0, curVelocity.Y);

            if (Tree.Query(this).Count() > 1)
            {
                Location -= new Vector(0, curVelocity.Y);
            }
        }

        Tree.Update(this);
    }
}