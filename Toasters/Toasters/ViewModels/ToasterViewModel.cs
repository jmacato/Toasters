using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Toasters.Models;

namespace Toasters.ViewModels;

public partial class ToasterViewModel : FlyingObjectsViewModel
{
    public enum ToasterState
    {
        WingLow1,
        WingLow2,
        WingHigh1,
        WingHigh2
    }

    [ObservableProperty] private ToasterState _state;

    private readonly Vector _velocity = new (-1, 1);

    public ToasterViewModel(MainViewModel mainViewModel, Vector position) : base(mainViewModel.Tree,
        TimeSpan.FromMilliseconds(20), position, new Size(64, 64))
    {
    }

    private int _animationTickCount, _highLowTimeOutTickCount, _highLowTimeOut = 40;
    private bool _currentState, _highState, _lowState;

    public override void Tick()
    {
        if (_highLowTimeOutTickCount >= _highLowTimeOut)
        {
            _currentState = Random.Shared.NextDouble() >= 0.5;
            _highLowTimeOutTickCount = 0;
            _highLowTimeOut = Random.Shared.Next(40, 80);
        }

        var curVelocity = _currentState ? _velocity * 2 : _velocity;
        Position += curVelocity;

        var k = Tree.Retrieve(this);

        if (k.Count > 1)
        {
            Position -= curVelocity;
        }

        if (_animationTickCount >= (_currentState ? 5 : 10))
        {
            if (_currentState)
            {
                State = _highState ? ToasterState.WingHigh1 : ToasterState.WingHigh2;
                _highState = !_highState;
            }
            else
            {
                State = _lowState ? ToasterState.WingLow1 : ToasterState.WingLow2;
                _lowState = !_lowState;
            }

            _animationTickCount = 0;
        }

        _animationTickCount++;
        _highLowTimeOutTickCount++;
 
            
        Tree.Update(this);
    }
}