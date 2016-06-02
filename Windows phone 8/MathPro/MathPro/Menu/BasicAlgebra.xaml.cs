using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using MathPro.ProblemBuilder;
using System.Windows.Threading;

namespace MathPro.Menu
{
    public partial class BasicAlgebra : PhoneApplicationPage
    {
        public BasicAlgebra()
        {
            InitializeComponent();
        }

        private void btnSolve_Click(object sender, RoutedEventArgs e)
        {
            App.Builder = new ProblemBuilder.GeneralBuilder(MathBase.EquationType.Algebraic, MathBase.Action.Solve);
            App.ProblemDefinition = new MathBase.Equation(MathBase.SignType.equal);
            Go();
        }

        private void btnDistribute_Click(object sender, RoutedEventArgs e)
        {
            App.Builder = new ProblemBuilder.GeneralBuilder(MathBase.EquationType.Algebraic, MathBase.Action.Distribute);
            App.ProblemDefinition = new MathBase.Equation(MathBase.SignType.equal);
            Go();
        }

        private void btnFactor_Click(object sender, RoutedEventArgs e)
        {
            App.Builder = new ProblemBuilder.GeneralBuilder(MathBase.EquationType.Algebraic, MathBase.Action.Factor);
            App.ProblemDefinition = new MathBase.Equation(MathBase.SignType.equal);
            Go();
        }

        private void btnSimplify_Click(object sender, RoutedEventArgs e)
        {
            App.Builder = new ProblemBuilder.GeneralBuilder(MathBase.EquationType.Algebraic, MathBase.Action.Simplify);
            App.ProblemDefinition = new MathBase.Equation(MathBase.SignType.equal);
            Go();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void Go()
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/SupportPages/SavedOrNew.xaml", UriKind.Relative));
            });
        }

    }
}