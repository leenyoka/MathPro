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

namespace MathPro.ProblemBuilder
{
    public partial class BasicCalculator : PhoneApplicationPage
    {
        public BasicCalculator()
        {
            InitializeComponent();
            var numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, -1, -2, -3, -4, -5, -6, -7, -8, -9 };
            var numbersP = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9};
            var chars = new string[] {"x","a", "b", "c","d", "e", "f", "g", "h", "i", "j", "k", "l",
                "m", "n", "o", "p", "q","r", "s", "t", "u", "v", "w", "y", "z"};
            var signs = new string[] { "+", "-", "*", "/", "+", "-", "*", "/" };

            base1.ItemsSource = chars;
            base2.ItemsSource = chars;
            coefficient1.ItemsSource = numbers;
            coefficient2.ItemsSource = numbers;
            power1.ItemsSource = numbersP;
            power2.ItemsSource = numbersP;
            sign.ItemsSource = signs;
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            MathBase.Equation eq = null;
            MathBase.Expression expression = new MathBase.Expression();
            expression.AddToExpression(GetTerm1(), true);
            //expression.AddToExpression(new MathBase.Sign(sign.SelectedItem.ToString()), true);
            //expression.AddToExpression(GetTerm2(), true);
            if (sign.SelectedItem.ToString() != "/")
            {
                MathBase.Expression expression2 = new MathBase.Expression();
                expression2.AddToExpression(GetTerm2(), true);

                eq = new MathBase.Equation(new MathBase.iEquationPiece[] { expression, 
                new MathBase.Sign(sign.SelectedItem.ToString()), expression2});
            }
            else
            {
                expression.AddToExpression(GetTerm2(), false);
                eq = new MathBase.Equation(new MathBase.iEquationPiece[] { expression });
            }
            App.Builder = new ProblemBuilder.GeneralBuilder(MathBase.EquationType.Algebraic, MathBase.Action.Simplify);
            App.ProblemDefinition = eq;

            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/SolutionViewer/BasicSolutionPlayer.xaml", UriKind.Relative));
            });

        }
        private MathBase.Term GetTerm1()
        {
            int coeffient = int.Parse(coefficient1.SelectedItem.ToString());
            string sign = coeffient < 0 ? "-" : "+";
            coeffient = Math.Abs(coeffient);
            string myBase = base1.SelectedItem.ToString();
            int power = int.Parse(power1.SelectedItem.ToString());
            string powerSign = power < 0 ? "-" : "+";
            power = Math.Abs(power);

            MathBase.Term term = new MathBase.Term(coeffient);
            term.Constant = false;
            term.PowerSign = powerSign;
            term.Sign = sign;
            term.TermBase = myBase[0];
            term.Power = power;

            return term;
        }
        private MathBase.Term GetTerm2()
        {
            int coeffient = int.Parse(coefficient2.SelectedItem.ToString());
            string sign = coeffient < 0 ? "-" : "+";
            coeffient = Math.Abs(coeffient);
            string myBase = base2.SelectedItem.ToString();
            int power = int.Parse(power2.SelectedItem.ToString());
            string powerSign = power < 0 ? "-" : "+";
            power = Math.Abs(power);

            MathBase.Term term = new MathBase.Term(coeffient);
            term.Constant = false;
            term.PowerSign = powerSign;
            term.Sign = sign;
            term.TermBase = myBase[0];
            term.Power = power;

            return term;
        }
    }
}