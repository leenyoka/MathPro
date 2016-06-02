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
    public partial class EquationsMenu : PhoneApplicationPage
    {
        public EquationsMenu()
        {
            InitializeComponent();

            //MathBase.Expression exp = new MathBase.Expression();
            //exp.AddToExpression(true, new MathBase.Term('x'));
            //exp.AddToExpression(false, new MathBase.Term('x',2));
            //exp.KillFraction();
            //MathBase.SimplificationExpression exp2 = new MathBase.SimplificationExpression(exp);
            //MathBase.SimpificationEquation eq = new MathBase.SimpificationEquation( exp2);
            //int x = 0;

        }
        private void btnBasic_Click(object sender, RoutedEventArgs e)
        {
            new GeneralBuilder(MathBase.EquationType.Algebraic, MathBase.Action.Solve);
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/Menu/BasicAlgebra.xaml", UriKind.Relative));
            });
        }

        private void btnExponential_Click(object sender, RoutedEventArgs e)
        {
            App.ProblemDefinition = new MathBase.ExponentialEquation(MathBase.SignType.equal);
            Go();
            //Dispatcher.BeginInvoke(() =>
            //{
            //    this.NavigationService.Navigate(new Uri("/ProblemBuilder/ExponentialBuilderViewer.xaml", UriKind.Relative));
            //});
        }
        private void Go()
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/SupportPages/SavedOrNew.xaml", UriKind.Relative));
            });
        }

        private void btnInequalities_Click(object sender, RoutedEventArgs e)
        {
            App.ProblemDefinition = new MathBase.Equation(MathBase.SignType.equal);
            App.Builder = new ProblemBuilder.GeneralBuilder(MathBase.EquationType.Algebraic, MathBase.Action.Inequalities);
            Go();
            //Dispatcher.BeginInvoke(() =>
            //{
            //    this.NavigationService.Navigate(new Uri("/ProblemBuilder/InequalitiesBuilderViewer.xaml", UriKind.Relative));
            //});
        }

        private void btnLogarithmicl_Click(object sender, RoutedEventArgs e)
        {
            App.ProblemDefinition = new MathBase.LogarithmicEquation(MathBase.SignType.equal);
            Go();
            //Dispatcher.BeginInvoke(() =>
            //{
            //    this.NavigationService.Navigate(new Uri("/ProblemBuilder/LogarithmicBuilderViewer.xaml", UriKind.Relative));
            //});
        }
    }
}