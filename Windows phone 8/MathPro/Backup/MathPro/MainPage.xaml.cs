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
using MathBase;
using System.Windows.Threading;
using MathPro.ProblemBuilder;
using System.Windows.Media.Imaging;

namespace MathPro
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        Calculator calc = new Calculator();
        DispatcherTimer timer;
        EventHandler _handler;
        public MainPage()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Start();
            App.Builder.Timer = timer;

            ContentPanel1.Children.Clear();
            this.ContentPanel1.Children.Add(App.Builder);

            App.Builder.Draw(ref timer);
            _handler = new EventHandler(ErrorBuilding);
            App.Builder.ErrorHandler = _handler;
            btnTest.Content = App.Builder.Action.ToString();


            #region Tests

            //MathBase.Expression exp = new MathBase.Expression();
            //exp.AddToExpression(true, new Term('x'));

            //ViewExpression viewExp = new ViewExpression(exp, new ViewPropeties[] { ViewPropeties.NoDenominators, ViewPropeties.NoDivisors });


            //viewExp.Select(0, -1, SelectedPieceType.Power, ref timer, true);

            //board.Children.Add(viewExp);


            //App.CurrentViewPlayerItem = 
            //NavigationService.Navigate(new Uri("/SolutionViewer/SolveForXAnswerPlayer.xaml", UriKind.Relative));















            ///         this.ContentPanel.Children.Add(new MathPro.ProblemBuilder.Keyboard());

            //TrigTerm one = new TrigTerm(new Term('x'), TrigFunction.Sin);
            //TrigTerm two = new TrigTerm(new Term('x',2), TrigFunction.Sin);


            //TrigExpression answer = calc.Calculate(one, two, MathFunction.Divide);

            //int done = 0;

            //MathBase.Expression a = new MathBase.Expression();
            //a.AddToExpression(new Term(1), true);
            //a.AddToExpression(false, new Term('x'), new Term(3, true));
            //a.AddToExpression(false, new Term(1), new Brace('('), new Term('x'), new Term(3, true), new Brace(')'));

            //MathBase.Expression b = new MathBase.Expression();
            //b.AddToExpression(new Term(1), true);
            //b.AddToExpression(false, new Term('x'), new Term(3));

            //MathBase.Expression c = new MathBase.Expression();
            //c.AddToExpression(new Term(10), true);
            //c.AddToExpression(false, new Term('x', 2), new Term(9, true));

            //Equation eq = new Equation(SignType.equal);
            //eq.Left.Add(a);
            //eq.Left.Add(new Sign("+"));
            //eq.Left.Add(b);
            //eq.Right.Add(c);

            //SolveForX solve = new SolveForX(eq);
            //ViewSolveForXSolution sol = new ViewSolveForXSolution(solve);





            //MathBase.Expression a = new MathBase.Expression(
            //    new IExpressionPiece[] { new Term('x', 2), 
            //        new Term('x'), new Term(6, true)}, null);

            //MathBase.Expression b = new MathBase.Expression();
            //b.AddToExpression(true, new Term(0));

            //Equation eq = new Equation(SignType.equal);
            //eq.Left.Add(a);
            //eq.Right.Add(b);

            //SolveForX solve = new SolveForX(eq);
            //ViewSolveForXSolution sol = new ViewSolveForXSolution(solve);
            //board.Children.Add(sol);

            int x = 0;



            //MathBase.Expression expressionD = new MathBase.Expression(new IExpressionPiece[] 
            //{new Brace('('), new Term('x'), new Term(2), new Brace(')'),
            //new Brace('('), new Term('x'), new Term(2), new Brace(')')}, null);

            //FactorDistributeSolution desDemo = new FactorDistributeSolution(expressionD, false);

            //ViewFactorDistributeSolution sol = new ViewFactorDistributeSolution(desDemo);

            //board.Children.Add(sol);








            //MathBase.Expression af = new MathBase.Expression();
            //af.AddToExpression(true, new Term(14));
            //MathBase.Expression bf = new MathBase.Expression();
            //bf.AddToExpression(true, new Term('x', 2, 1, true));
            //MathBase.Expression cf = new MathBase.Expression();
            //cf.AddToExpression(true, new Term(6));


            //Inequalities xf = new Inequalities(af, new Sign(">"), bf, new Sign(">"), cf);

            //ViewInequalitiesSolution sol = new ViewInequalitiesSolution(xf);

            //this.board.Children.Add(sol);



            //ExponentialEquation eqExpo = new ExponentialEquation(SignType.equal);
            //eqExpo.Left.Add(new ExponentialTerm(2, new MathBase.Expression(new IExpressionPiece[] { new Term('x', 5, 1), new Term(2, true) }, null)));
            //eqExpo.Right.Add(new ExponentialTerm(176));
            //SolveForExponentialEquations expX = new SolveForExponentialEquations(eqExpo);

            //ViewExponentialSolution sol = new ViewExponentialSolution(expX);




            //ExponentialEquation eqExpoSameaBase = new ExponentialEquation(SignType.equal);
            //eqExpoSameaBase.Left.Add(new ExponentialTerm(9, new MathBase.Expression(new IExpressionPiece[] { new Term('x', 4, 1), new Term(1, true) }, null)));
            //eqExpoSameaBase.Right.Add(new ExponentialTerm(27, new MathBase.Expression(new IExpressionPiece[] { new Term(5), new Term('x', true) }, null)));
            //SolveForExponentialEquations expXSamBase = new SolveForExponentialEquations(eqExpoSameaBase);

            //ViewExponentialSolution sol = new ViewExponentialSolution(expXSamBase);


            //board.Children.Add(sol);



            //LogarithmicEquation eqLogR = new LogarithmicEquation(
            //    new IAlgebraPiece[] { new LogTerm(1, '2', new MathBase.Expression(new IExpressionPiece[] { new Term('x', 2), new Term('x', 2, 1, true) }, null)) },
            //   SignType.equal,
            //   new IAlgebraPiece[] { new Term(3) });

            //SolveForLogarithmicEquations sFoxRelationship = new SolveForLogarithmicEquations(eqLogR);

            //ViewLogarithmicSolution sol = new ViewLogarithmicSolution(sFoxRelationship);

            //2 log b ^ (x) = log b^4 + log b ^(x-1)
            //LogarithmicEquation eqLog1 = new LogarithmicEquation(
            //    /*left Start */ new IAlgebraPiece[] { new LogTerm(2, 'b', new MathBase.Expression(new IExpressionPiece[] { new Term('x') }, null)) } /*left end */,
            //    SignType.equal,
            //    /* right Startt */ new IAlgebraPiece[] { new LogTerm(1, 'b', new MathBase.Expression(new IExpressionPiece[] 
            //    {new Term(4)}, null) ), new Sign( SignType.Add),
            //    new LogTerm(1, 'b', new MathBase.Expression(new IExpressionPiece[] { new Term('x'), new Term(1,true) }, null))});

            //SolveForLogarithmicEquations sForX1 = new SolveForLogarithmicEquations(eqLog1);

            //ViewLogarithmicSolution sol = new ViewLogarithmicSolution(sForX1);
            //board.Children.Add(sol);


            int xd = 0;













            //SimplificationExpression expressionOne = new SimplificationExpression(
            //new IExpressionPiece[] { new Term('x'), new Term(2) },
            //new IExpressionPiece[] { new Term('x', 2, 1), new Term(2, true) });

            //SimplificationExpression expressionTwo = new SimplificationExpression(
            //new IExpressionPiece[] { new Term('x'), new Term(1, true) },
            //new IExpressionPiece[] { new Term('x'), new Term(2) });

            //SimpificationEquation checker = new SimpificationEquation(expressionOne, new Sign("*"), expressionTwo);


            //MathBase.Expression expBraces = new MathBase.Expression(
            //    new IExpressionPiece[] {
            //        new Term(5), new Brace('('),
            //        new Term(2), new Term('x'),
            //        new Brace(')') , new Term(3),
            //            new Brace('('),
            //            new Term('x',5,1), new Term(4), 
            //            new Brace(')'), new Term('x',4,true)}, null);

            //SimpificationEquation checkern = new
            // SimpificationEquation(new SimplificationExpression(expBraces));
            //ViewSimpificationSolution piece = new ViewSimpificationSolution(checker);




            //int xijhd = 0;

            //board.Children.Add(piece);
            #endregion Tests

        }
        private void ErrorBuilding(object sender, EventArgs e)
        {
            MessageBox.Show("Not allowed");
        }
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            App.Builder.Top = App.Builder.Top.Replace("%", "");
            App.Builder.Bot = App.Builder.Bot.Replace("%", "");
            try
            {
                if (App.Builder.Action == MathBase.Action.Solve)
                {
                    if (App.Builder.Top.IndexOf('=') == -1)
                    {
                        App.Builder.Top += "=0";
                        App.Builder.Bot += "=0";
                    }
                }
                App.ProblemDefinition = App.Builder.GetEquation();
                App.problemBuilt = new string[] { App.Builder.Top, App.Builder.Bot };
            }
            catch
            {
                MessageBox.Show("Something went wrong and we dont know what! Please try again");
            }
            //App.CurrentViewPlayerItem = sol;

            Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/SolutionViewer/BasicSolutionPlayer.xaml", UriKind.Relative));
            });
        }
        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            WriteableBitmap bmp = new WriteableBitmap((int)this.ActualWidth, (int)this.ActualHeight);
            bmp.Render(this, null);
            bmp.Invalidate();

            Capture.Capturer capturer = new Capture.Capturer();
            capturer.SaveImage(bmp);
            //capturer.SendImage(bmp);
        }
        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            if (this.SupportedOrientations == SupportedPageOrientation.Landscape)
            {
                this.SupportedOrientations = SupportedPageOrientation.Portrait;
                this.Orientation = PageOrientation.Portrait;
            }
            else
            {
                this.SupportedOrientations = SupportedPageOrientation.Landscape;
                this.Orientation = PageOrientation.Landscape;
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //this.ContentPanel.Children.Remove(App.Builder);
            App.GoingBack = true;
        }
    }
}



