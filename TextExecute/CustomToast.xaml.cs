using LibToast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TextExecute
{
    /// <summary>
    /// Interaction logic for CustomToast.xaml
    /// </summary>
    public partial class CustomToast : ToastWindow
    {
        public CustomToast() : base(Corner.TopRight, Direction.Left, TimeSpan.FromSeconds(.25), TimeSpan.FromSeconds(2), 144)
        {
            InitializeComponent();
        }
    }
}
