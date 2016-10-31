using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaCenter.Modules.Showcase.CustomControl
{
    /// <summary>
    /// Interaction logic for MediaItemUserControl.xaml
    /// </summary>
    public partial class MediaItemUserControl : ButtonBase
    {
        public string ItemName
        {
            get { return (string)GetValue(ItemNameProperty); }
            set { SetValue(ItemNameProperty, value); }
        }

        public static readonly DependencyProperty ItemNameProperty =
        DependencyProperty.Register("ItemName", typeof(string), typeof(MediaItemUserControl));

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
        DependencyProperty.Register("Image", typeof(ImageSource), typeof(MediaItemUserControl));

        public ImageSource ImageSelected
        {
            get { return (ImageSource)GetValue(ImageSelectedProperty); }
            set { SetValue(ImageSelectedProperty, value); }
        }

        public static readonly DependencyProperty ImageSelectedProperty =
        DependencyProperty.Register("ImageSelected", typeof(ImageSource), typeof(MediaItemUserControl));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register("IsChecked", typeof(bool), typeof(MediaItemUserControl));

        //public double ImageWidth
        //{
        //    get { return (double)GetValue(ImageWidthProperty); }
        //    set { SetValue(ImageWidthProperty, value); }
        //}

        //public static readonly DependencyProperty ImageWidthProperty =
        //DependencyProperty.Register("ImageWidth", typeof(double), typeof(MediaItemUserControl));

        // public double ImageHeight
        //{
        //    get { return (double)GetValue(ImageHeightProperty); }
        //    set { SetValue(ImageHeightProperty, value); }
        //}

        //public static readonly DependencyProperty ImageHeightProperty =
        //DependencyProperty.Register("ImageHeight", typeof(double), typeof(MediaItemUserControl));

        public MediaItemUserControl()
        {
            InitializeComponent();
        }
    }
}
