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
using System.Windows.Threading;
using MathBase;
using System.Windows.Media.Imaging;

namespace MathPro.SolutionViewer
{
    public partial class BasicSolutionPlayer : PhoneApplicationPage
    {
        DispatcherTimer timer;
        ViewPlayerItem sol;
        SettingsAssistant assistant = new SettingsAssistant();
        public BasicSolutionPlayer()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(TimerStep);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 65);
            HandleOrientation();
        }
        public void HandleOrientation()
        {
            if (assistant.ValuePairExists("Orientation"))
            {

                if (assistant.getValuePairFromSettings("Orientation").Value == "landscape")
                {
                    if (this.SupportedOrientations == SupportedPageOrientation.Portrait)
                        ChangeOrientation();
                }
                else if (this.SupportedOrientations == SupportedPageOrientation.Portrait)
                    ChangeOrientation();
            }
            
        }
        public void SetUp()
        {
            try
            {
                if (App.ProblemDefinition.GetStepType() == SolveForXStepType.Equation)
                {
                    #region Equation
                    if (App.Builder.Action == MathBase.Action.Solve)
                    {
                        MathBase.SolveForX prob = new SolveForX((Equation)App.ProblemDefinition);
                        ViewSolveForXSolution sol = new ViewSolveForXSolution(prob);
                        this.sol = sol;
                    }
                    else if (App.Builder.Action == MathBase.Action.Simplify)
                    {
                        #region Simplify

                        Equation eq = (Equation)App.ProblemDefinition;

                        List<ISimplificationPiece> pieces = new List<ISimplificationPiece>();
                        for (int i = 0; i < eq.Left.Count; i++)
                            if (eq.Left[i].GetEquationPieceType() == EquationPieceType.Sign)
                                pieces.Add(((MathBase.Sign)eq.Left[i]));
                            else
                            {
                                SimplificationExpression expSim = new SimplificationExpression((MathBase.Expression)eq.Left[i]);
                                pieces.Add(expSim);
                            }

                        SimpificationEquation prob = new SimpificationEquation(pieces.ToArray());
                        ViewSimpificationSolution sol = new ViewSimpificationSolution(prob);
                        this.sol = sol;

                        #endregion Simplify
                    }
                    else if (App.Builder.Action == MathBase.Action.Factor)
                    {
                        MathBase.FactorDistributeSolution prob =
                            new FactorDistributeSolution((MathBase.Expression)((Equation)App.ProblemDefinition).Left[0], true);
                        ViewFactorDistributeSolution sol = new ViewFactorDistributeSolution(prob);
                        this.sol = sol;
                    }
                    else if (App.Builder.Action == MathBase.Action.Distribute)
                    {
                        MathBase.FactorDistributeSolution prob =
                        new FactorDistributeSolution((MathBase.Expression)((Equation)App.ProblemDefinition).Left[0], false);
                        ViewFactorDistributeSolution sol = new ViewFactorDistributeSolution(prob);
                        this.sol = sol;
                    }
                    else if (App.Builder.Action == MathBase.Action.Inequalities)
                    {
                        MathBase.Inequalities prob = new Inequalities((Equation)App.ProblemDefinition);
                        ViewInequalitiesSolution sol = new ViewInequalitiesSolution(prob);
                        this.sol = sol;
                    }
                    #endregion Equation

                }
                else if (App.ProblemDefinition.GetStepType() == SolveForXStepType.ExpoEquation)
                {
                    MathBase.SolveForExponentialEquations prob =
                    new SolveForExponentialEquations((ExponentialEquation)App.ProblemDefinition);
                    ViewExponentialSolution sol = new ViewExponentialSolution(prob);
                    this.sol = sol;
                }
                else if (App.ProblemDefinition.GetStepType() == SolveForXStepType.LogEquation)
                {
                    MathBase.SolveForLogarithmicEquations prob =
                        new SolveForLogarithmicEquations((LogarithmicEquation)App.ProblemDefinition);
                    ViewLogarithmicSolution sol = new ViewLogarithmicSolution(prob);
                    this.sol = sol;
                }
            }
            catch
            {
                MessageBox.Show("Could not solve your problem. Please ensure you entered it correctly, if so, the app can not solve that problem, sorry", "What?", MessageBoxButton.OK);
                GoodForShow = false;
                NavigationService.GoBack();
            }
        }
        public void TimerStep(object sender, EventArgs e)
        {
            if (!ShowNext())
                timer.Stop();
        }
        private bool ShowNext()
        {
            return sol.ShowNext();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetUp();
            if (GoodForShow)
            {
                sol.HideAll();
                this.ContentPanel.Children.Add(sol);
                timer.Start();
            }
        }
        private bool GoodForShow = true;

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            WriteableBitmap bmp = new WriteableBitmap((int)this.ActualWidth, (int)this.ActualHeight);
            bmp.Render(this, null);
            bmp.Invalidate();

            Capture.Capturer capturer = new Capture.Capturer();
            capturer.SaveImage(bmp);
        }
        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            ChangeOrientation();
        }
        public void ChangeOrientation()
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


        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            //save
            if (App.ProblemDefinition.GetStepType() == SolveForXStepType.Equation)
            {
                #region Equation
                if (App.Builder.Action == MathBase.Action.Solve)
                {
                    Save("NumberOfSavedSolve"); 
                }
                else if (App.Builder.Action == MathBase.Action.Simplify)
                {
                    Save("NumberOfSavedSimplify");
                }
                else if (App.Builder.Action == MathBase.Action.Factor)
                {
                    Save("NumberOfSavedFactor");
                }
                else if (App.Builder.Action == MathBase.Action.Distribute)
                {
                    Save("NumberOfSavedDistribute");
                }
                else if (App.Builder.Action == MathBase.Action.Inequalities)
                {
                    Save("NumberOfSavedInequalities");
                }
                #endregion Equation
            }
            else if (App.ProblemDefinition.GetStepType() == SolveForXStepType.ExpoEquation)
            {
                Save("NumberOfSavedExponential");
            }
            else if (App.ProblemDefinition.GetStepType() == SolveForXStepType.LogEquation)
            {
                Save("NumberOfSavedLogarithmic");
            }
        }
        private void Save(string key)
        {
            try
            {
                valuePair pair = assistant.getValuePairFromSettings(key);
                string name = key.Replace("NumberOfSaved", "");
                name += (int.Parse(pair.Value) + 1).ToString();
                assistant.changeSetting(new valuePair(name + "top", App.problemBuilt[0].Replace("=", "equals").Replace(">", "greater").Replace("<", "smaller")));
                assistant.changeSetting(new valuePair(name + "bot", App.problemBuilt[1].Replace("=", "equals").Replace(">", "greater").Replace("<", "smaller")));
                assistant.changeSetting(new valuePair(pair.Name, (int.Parse(pair.Value) + 1).ToString()));
            }
            catch
            {

            }

            //valuePair testResults = assistant.getValuePairFromSettings("Logarithmic1top");
            //testResults.Value = testResults.Value.Replace("equals", "=");
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.GoingBack = true;
        }
    }
}