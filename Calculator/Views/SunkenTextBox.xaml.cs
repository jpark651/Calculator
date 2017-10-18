using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator.Views
{
    /// <summary>
    /// Interaction logic for SunkenTextBox.xaml
    /// </summary>
    public partial class SunkenTextBox : UserControl
    {
        #region Text DP

        /// <summary>
        /// Gets or sets the Label which is displayed next to the field
        /// </summary>
        public String Text
        {
            get { return (String)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Text", typeof(string),
              typeof(SunkenTextBox), new PropertyMetadata(""));

        #endregion

        public SunkenTextBox()
        {
            InitializeComponent();

            SunkenText.DataContext = this;
        }
    }
}