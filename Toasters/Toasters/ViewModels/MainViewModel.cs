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
        private int _widthGrid;
        private int _heightGrid;
        private List<(int x, int y)> _excludedPositions = null!;
        
        public QuadTree? Tree { get; private set; }
        public ObservableCollection<FlyingObjectsViewModel> FlyingObjects { get; } = new();

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewBounds))
            {
                if (ViewBounds.Width == 0 || ViewBounds.Height == 0) return;

                Tree = new QuadTree(new RectangleViewModel(0, 0, ViewBounds.Width,
                    ViewBounds.Height));

                foreach (var flyingObject in FlyingObjects)
                {
                    flyingObject.Dispose();
                }

                FlyingObjects.Clear();
                Tree.Clear();

                // Turn the active view into a square grid indices.
                _widthGrid = (int)Math.Round(_viewBounds.Width / 64f);
                _heightGrid = (int)Math.Round(_viewBounds.Height / 64f);

                // When the flying objects are out of view, they must not respawn on the active view.
                _excludedPositions = Enumerable.Range(0, _heightGrid + 1)
                    .SelectMany(y => Enumerable.Range(0, _widthGrid + 1).Select(x => (x, y))).ToList();

                // Select random positions with flexible probability from the square grid indices.
                var potentialPositions = Enumerable.Range(1, _heightGrid - 2)
                    .SelectMany(y => Enumerable.Range(1, _widthGrid - 2).Select(x => (x, y)))
                    .Where(_ => Random.Shared.NextDouble() <= 0.4)
                    .Distinct()
                    .Select(w => new Vector(w.x, w.y) * 64);

                foreach (var pos in potentialPositions)
                {
                    GenerateFlyingObject(pos);
                }
            }

            base.OnPropertyChanged(e);
        }

        private void GenerateFlyingObject(Vector v)
        {
            if (Random.Shared.NextDouble() >= 0.3)
            {
                FlyingObjects.Add(new ToasterViewModel(this, v));
            }
            else
            {
                FlyingObjects.Add(new BreadViewModel(this, v));
            }
        }

        public void ObjectOutOfBounds(FlyingObjectsViewModel flyingObject)
        {
            if (Tree is null) return;
            var attempt = 10;
            Vector potentialPosition;
            do
            {
                attempt--;

                potentialPosition = Enumerable.Range(1, 3)
                    .Select(x => -x)
                    .Concat(Enumerable.Range(0, _heightGrid))
                    .SelectMany(y =>
                        Enumerable.Range(Random.Shared.Next(0, (int)Math.Round(_widthGrid / 2d)), _widthGrid + 5)
                            .Select(x => (x, y)))
                    .Where(x => !_excludedPositions.Contains(x))
                    .OrderBy(_ => Random.Shared.NextDouble())
                    .Select(w => new Vector(w.x, w.y) * 64)
                    .First();

                if (Tree.Query(new RectangleViewModel(potentialPosition.X, potentialPosition.Y, 64, 64)).Any())
                    continue;
                break;
            } while (attempt > 0);
            FlyingObjects.Remove(flyingObject);
            flyingObject.Dispose();
            
            GenerateFlyingObject(potentialPosition);
        }
    }
}