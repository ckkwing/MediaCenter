using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MediaCenter.Theme.CustomControl
{
    public class ImageButton : Button
    {
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
        DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton));

        /// <summary>
        /// Gets or sets the image over.
        /// </summary>
        /// <value>The image over.</value>
        public ImageSource ImageOver
        {
            get { return (ImageSource)GetValue(ImageOverProperty); }
            set { SetValue(ImageOverProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ImageOver.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ImageOverProperty =
        DependencyProperty.Register("ImageOver", typeof(ImageSource), typeof(ImageButton));

        /// <summary>
        /// Gets or sets the image over.
        /// </summary>
        /// <value>The image over.</value>
        public ImageSource ImagePressed
        {
            get { return (ImageSource)GetValue(ImagePressedProperty); }
            set { SetValue(ImagePressedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ImageOver.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ImagePressedProperty =
        DependencyProperty.Register("ImagePressed", typeof(ImageSource), typeof(ImageButton));

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>The width of the image.</value>
        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ImageWidthProperty =
        DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageButton));

        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        /// <value>The height of the image.</value>
        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ImageHeightProperty =
        DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImageButton));

        /// <summary>
        /// Gets or sets the text
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(ImageButton));

        public ImageButton()
        {
            this.Style = (Style)Application.Current.Resources["ImageButtonStyle"];
        }
    }
}
