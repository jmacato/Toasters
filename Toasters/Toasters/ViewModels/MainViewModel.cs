using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Toasters.Models;

namespace Toasters.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty] private Size _viewBounds;

        public QuadTree Tree { get; private set; }
        public ObservableCollection<FlyingObjectsViewModel> FlyingObjects { get; } = new();

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewBounds))
            {
                if(ViewBounds.Width == 0 || ViewBounds.Height == 0) return;
                
                Tree = new QuadTree(new RectangleViewModel(0, 0, ViewBounds.Width,
                    ViewBounds.Height));

                foreach (var flyingObject in FlyingObjects)
                {
                    flyingObject.Dispose();
                }

                FlyingObjects.Clear();
                Tree.Clear();

                for (var i = 0; i < 50; i++)
                {
                    var attempts = 10;
                    do
                    {
                        var randomRect = new RectangleViewModel(Vector.Random(ViewBounds.Width, ViewBounds.Height),
                            new Size(64, 64));
                        if (!Tree.Query(randomRect).Any())
                        {
                            FlyingObjects.Add(new ToasterViewModel(this, randomRect.Location));
                            break;
                        }

                        attempts--;
                    } while (attempts > 0);
                }


                for (var i = 0; i < 20; i++)
                {
                    var attempts = 10;
                    do
                    {
                        var randomRect = new RectangleViewModel(Vector.Random(ViewBounds.Width, ViewBounds.Height),
                            new Size(64, 64));
                        if (!Tree.Query(randomRect).Any())
                        {
                            FlyingObjects.Add(new BreadViewModel(this, randomRect.Location));
                            break;
                        }

                        attempts--;
                    } while (attempts > 0);
                }
            }

            base.OnPropertyChanged(e);
        }
    }
}