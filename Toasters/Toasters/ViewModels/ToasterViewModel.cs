using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Toasters.Models;

namespace Toasters.ViewModels;

public class ToasterViewModel : FlyingObjectsViewModel
{
    private readonly Vector _velocity = new(-1, 1);

    public ToasterViewModel(MainViewModel mainViewModel, Vector position) : base(mainViewModel.Tree,
        TimeSpan.FromMilliseconds(20), position, new Size(64, 64))
    {
    }

    private int _animationTickCount, _fastSlowTimeOutTickCount, _fastSlowTimeOut = 40;
    private bool _isFast, _highState, _lowState;

    public override void Tick()
    {
        if (_fastSlowTimeOutTickCount >= _fastSlowTimeOut)
        {
            _isFast = Random.Shared.NextDouble() >= 0.5;
            _fastSlowTimeOutTickCount = 0;
            _fastSlowTimeOut = Random.Shared.Next(3, 8) * 10;
        }

        var curVelocity = _isFast ? _velocity * 2 : _velocity;

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

        if (_animationTickCount >= (_isFast ? 5 : 10))
        {
            if (_isFast)
            {
                State = _highState ? FlyingObjectState.WingFast1 : FlyingObjectState.WingFast2;
                _highState = !_highState;
            }
            else
            {
                State = _lowState ? FlyingObjectState.WingSlow1 : FlyingObjectState.WingSlow2;
                _lowState = !_lowState;
            }

            _animationTickCount = 0;
        }

        _animationTickCount++;
        _fastSlowTimeOutTickCount++;

        Tree.Update(this);
    }
}

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