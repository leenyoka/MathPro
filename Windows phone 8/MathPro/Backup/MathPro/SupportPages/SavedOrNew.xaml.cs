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
using MathBase;

namespace MathPro.SupportPages
{
    public partial class SavedOrNew : PhoneApplicationPage
    {
        public SavedOrNew()
        {
            InitializeComponent();
            App.GoingBack = false;
        }
        private string GetName()
        {
            if (App.ProblemDefinition.GetStepType() == SolveForXStepType.Equation)
            {
                #region Equation
                if (App.Builder.Action == MathBase.Action.Solve)
                {
                    return "Solve";
                }
                else if (App.Builder.Action == MathBase.Action.Simplify)
                {
                    return "Simplify";
                }
                else if (App.Builder.Action == MathBase.Action.Factor)
                {
                    return "Factor";
                }
                else if (App.Builder.Action == MathBase.Action.Distribute)
                {
                    return "Distribute";
                }
                else if (App.Builder.Action == MathBase.Action.Inequalities)
                {
                    return "Inequalities";
                }
                #endregion Equation

            }
            else if (App.ProblemDefinition.GetStepType() == SolveForXStepType.ExpoEquation)
            {
                return "Exponential";
            }
            else if (App.ProblemDefinition.GetStepType() == SolveForXStepType.LogEquation)
            {
                return "Logarithmic";
            }
            return "";
        }
        SettingsAssistant assistant = new SettingsAssistant();
        EventHandler handler;
        private void Initialize(string name)
        {
            assistant = new SettingsAssistant();
            SavedList.Items.Clear();
            handler = new EventHandler(Selected);
            int numberOfSaved = int.Parse(assistant.getValuePairFromSettings("NumberOfSaved" + name).Value);

            for (int i = 1; i <= numberOfSaved; i++)
            {
                string top =assistant.getValuePairFromSettings( name + i + "top").Value;
                string bot =assistant.getValuePairFromSettings( name + i + "bot").Value;

                if (top.IndexOf("deleted") == -1)
                {

                    if (App.ProblemDefinition.GetStepType() == SolveForXStepType.Equation)
                    {
                        #region Equation
                        if (App.Builder.Action == MathBase.Action.Solve)
                        {
                            GeneralBuilder builder = new ProblemBuilder.GeneralBuilder(name + i.ToString(), top, bot, handler, MathBase.EquationType.Algebraic, MathBase.Action.Solve);
                            //App.ProblemDefinition = builder.GetEquation();
                            //App.problemBuilt = new string[] { App.Builder.Top, App.Builder.Bot };
                            SavedList.Items.Add(builder);
                        }
                        else if (App.Builder.Action == MathBase.Action.Simplify)
                        {
                            GeneralBuilder builder = new ProblemBuilder.GeneralBuilder(name + i.ToString(), top, bot, handler, MathBase.EquationType.Algebraic, MathBase.Action.Simplify);
                            //App.ProblemDefinition = builder.GetEquation();
                            //App.problemBuilt = new string[] { App.Builder.Top, App.Builder.Bot };
                            SavedList.Items.Add(builder);
                        }
                        else if (App.Builder.Action == MathBase.Action.Factor)
                        {
                            GeneralBuilder builder = new ProblemBuilder.GeneralBuilder(name + i.ToString(), top, bot, handler, MathBase.EquationType.Algebraic, MathBase.Action.Factor);
                            //App.ProblemDefinition = builder.GetEquation();
                            //App.problemBuilt = new string[] { App.Builder.Top, App.Builder.Bot };
                            SavedList.Items.Add(builder);
                        }
                        else if (App.Builder.Action == MathBase.Action.Distribute)
                        {
                            GeneralBuilder builder = new ProblemBuilder.GeneralBuilder(name + i.ToString(), top, bot, handler, MathBase.EquationType.Algebraic, MathBase.Action.Distribute);
                            //App.ProblemDefinition = builder.GetEquation();
                            //App.problemBuilt = new string[] { App.Builder.Top, App.Builder.Bot };
                            SavedList.Items.Add(builder);
                        }
                        else if (App.Builder.Action == MathBase.Action.Inequalities)
                        {
                            InequalitiesBuilder builder = new ProblemBuilder.InequalitiesBuilder(name + i.ToString(), top, bot, handler);

                            //App.ProblemDefinition = builder.GetEquation();
                            //App.problemBuilt = new string[] { App.Builder.Top, App.Builder.Bot };
                            SavedList.Items.Add(builder);
                        }
                        #endregion Equation

                    }
                    else if (App.ProblemDefinition.GetStepType() == SolveForXStepType.ExpoEquation)
                    {
                        ExponentialBuilder builder = new ProblemBuilder.ExponentialBuilder(name + i.ToString(), top, handler);

                        //App.ProblemDefinition = builder.GetEquation();
                        //App.problemBuilt = new string[] { App.Builder.Top, App.Builder.Bot };
                        SavedList.Items.Add(builder);
                    }
                    else if (App.ProblemDefinition.GetStepType() == SolveForXStepType.LogEquation)
                    {
                        LogarithimsBuilder builder = new ProblemBuilder.LogarithimsBuilder(name + i.ToString(), top, bot, handler);

                        //App.ProblemDefinition = builder.GetEquation();
                        //App.problemBuilt = new string[] { App.Builder.Top, App.Builder.Bot };
                        SavedList.Items.Add(builder);
                    }
                }
                

            }
        }
        private void Selected(object sender, EventArgs e)
        {
            bool show = false;
            IProblemBuilder bu = sender as IProblemBuilder;


            if (bu.GetEquationType() == EquationType.Algebraic
                 || bu.GetEquationType() == EquationType.General)
            {
                

                GeneralBuilder builder = bu as GeneralBuilder;


                if (builder == null)
                {
                    InequalitiesBuilder built = bu as InequalitiesBuilder;// new InequalitiesBuilder(builder.Top, builder.Bot, handler);
                    if (built.Top.IndexOf("deleted") == -1)
                    {
                        App.ProblemDefinition = built.GetEquation();
                        App.problemBuilt = new string[] { built.Top, built.Bot };
                        show = true;
                    }
                }
                else
                {
                    if (builder.Top.IndexOf("deleted") == -1)
                    {
                        App.ProblemDefinition = builder.GetEquation();
                        App.problemBuilt = new string[] { builder.Top, builder.Bot };
                        show = true;
                    }
                }
            }
            else if (bu.GetEquationType() == EquationType.Exponential)
            {

                ExponentialBuilder builder = bu as ExponentialBuilder; if (builder.Top.IndexOf("deleted") == -1)
                {
                    App.ProblemDefinition = builder.GetEquation();
                    App.problemBuilt = new string[] { builder.Top, "" };
                    show = true;
                }

            }
            else if (bu.GetEquationType() == EquationType.Logarithmic)
            {
                LogarithimsBuilder builder = bu as LogarithimsBuilder;

                if (builder.Top.IndexOf("deleted") == -1)
                {

                    App.ProblemDefinition = builder.GetEquation();
                    App.problemBuilt = new string[] { builder.Top, builder.Bot };
                    show = true;
                }
            }


            if (show)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.NavigationService.Navigate(new Uri("/SolutionViewer/BasicSolutionPlayer.xaml", UriKind.Relative));
                });
            }

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Initialize(GetName());

            if (App.GoingBack)
                NavigationService.GoBack();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (App.ProblemDefinition.GetStepType() == SolveForXStepType.Equation)
            {

                if (App.Builder.Action == MathBase.Action.Inequalities)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        this.NavigationService.Navigate(new Uri("/ProblemBuilder/InequalitiesBuilderViewer.xaml", UriKind.Relative));
                    });
                }
                else
                {

                    Dispatcher.BeginInvoke(() =>
                    {
                        this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    });
                }
            }
            else if (App.ProblemDefinition.GetStepType() == SolveForXStepType.ExpoEquation)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.NavigationService.Navigate(new Uri("/ProblemBuilder/ExponentialBuilderViewer.xaml", UriKind.Relative));
                });

            }
            else if (App.ProblemDefinition.GetStepType() == SolveForXStepType.LogEquation)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.NavigationService.Navigate(new Uri("/ProblemBuilder/LogarithmicBuilderViewer.xaml", UriKind.Relative));
                });
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}