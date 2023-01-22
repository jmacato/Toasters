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
        
        public Quadtree Tree { get;  } = new  ();
        public ObservableCollection<ToasterViewModel> Toasters { get; } = new ();

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewBounds))
            {
                
                foreach (var toaster in Toasters)
                {
                    toaster.Dispose();
                }
                Toasters.Clear();
                Tree.Clear();
                
                for (int i = 0; i < 10; i++)
                {
                    var attempts = 10;
                    do
                    {
                        var randomRect = new RectangleViewModel(Vector.Random(ViewBounds.Width, ViewBounds.Height) , new Size(64, 64));
                        if (Tree.Retrieve(randomRect).Count == 0)
                        {
                            Toasters.Add(new ToasterViewModel(this, randomRect.Position));
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