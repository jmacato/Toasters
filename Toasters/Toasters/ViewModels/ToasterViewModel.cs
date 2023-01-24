using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Toasters.Models;

namespace Toasters.ViewModels;

public class ToasterViewModel : FlyingObjectsViewModel
{
    public ToasterViewModel(MainViewModel mainViewModel, Vector position) : base(mainViewModel,
        TimeSpan.FromMilliseconds(20), position, new Size(64, 64), new(-1, 1))
    {
    }

    private int _animationTickCount, _fastSlowTimeOutTickCount, _fastSlowTimeOut = 40;
    private bool _isFast, _highState, _lowState;

    protected override void Tick()
    {
        if (_fastSlowTimeOutTickCount >= _fastSlowTimeOut)
        {
            _isFast = Random.Shared.NextDouble() >= 0.5;
            _fastSlowTimeOutTickCount = 0;
            _fastSlowTimeOut = Random.Shared.Next(3, 8) * 10;
        }

        Velocity = _isFast ? new Vector(-2, 2) : new Vector(-1, 1);

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
    }
}