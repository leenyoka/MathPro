using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
using MathBase;
using MathPro.Display;

namespace MathPro
{
	 [Activity(Label = "MathPro", MainLauncher = false, Icon = "@drawable/icon")]
	public class SolutionPlayer : Activity
	{
		
		 private SolveForXStepType _type;
		 private MathBase.Action _action;
		 private readonly Helper _helper = new Helper();

		 protected override void OnCreate(Bundle bundle)
		 {
			 base.OnCreate(bundle);
			 SetContentView(Resource.Layout.Player);

			 var value1 = _helper.ReadPreferent("SolveForXStepType");
			 var value2 = _helper.ReadPreferent("Action");

			_type = (SolveForXStepType)Enum.Parse(typeof(SolveForXStepType), value1, true);
			_action = (MathBase.Action)Enum.Parse(typeof(MathBase.Action), value2, true);

			ChangeZoom(0);
			 ZoomSetup();
		 }

		 private void ZoomSetup()
		 {

			 (FindViewById(Resource.Id.zoomIn)).Click += ZoomIn;


			 (FindViewById(Resource.Id.zoomOut)).Click += ZoomOut;
		 }

		 private void ZoomIn(object sender, EventArgs e)
		 {
			 ChangeZoom(1);
		 }

		 private void ZoomOut(object sender, EventArgs e)
		 {
			 ChangeZoom(-1);
		 }
		 public void ChangeZoom(int change)
		 {
			 _helper.ChangeZoom(change, ((Button) FindViewById(Resource.Id.zoomIn)),
				 ((Button) FindViewById(Resource.Id.zoomOut)), SetUp);
		 }
		 public void SetUp()
		 {
			 var host = FindViewById<LinearLayout>(Resource.Id.hostess);
			 host.RemoveAllViews();
			 var problemDefinition = _helper.GetProblemDefinition(_type,_action,this);
			 //try
			 //{
				 if (problemDefinition.GetStepType() == SolveForXStepType.Equation)
				 {
					 #region Equation
					 if (_action == MathBase.Action.Solve)
					 {
						 var prob = new SolveForX((Equation)problemDefinition);
						 var sol = new ViewSolveForXSolution(prob,this);
						 host.AddView(sol);
					 }
					 else if (_action == MathBase.Action.Simplify)
					 {
						 #region Simplify

						 var eq = (Equation)problemDefinition;

						 var pieces = new List<ISimplificationPiece>();
						 for (var i = 0; i < eq.Left.Count; i++)
							 if (eq.Left[i].GetEquationPieceType() == EquationPieceType.Sign)
								 pieces.Add(((Sign)eq.Left[i]));
							 else
							 {
								 var expSim = new SimplificationExpression((Expression)eq.Left[i]);
								 pieces.Add(expSim);
							 }

						 var prob = new SimpificationEquation(pieces.ToArray());
						 var sol = new ViewSimpificationSolution(prob,this);
						 host.AddView(sol);

						 #endregion Simplify
					 }
					 else if (_action == MathBase.Action.Factor)
					 {
						 var prob =
							 new FactorDistributeSolution((Expression)((Equation)problemDefinition).Left[0], true);
						 var sol = new ViewFactorDistributeSolution(prob,this);
						 host.AddView(sol);
					 }
					 else if (_action == MathBase.Action.Distribute)
					 {
						 var prob =
						 new FactorDistributeSolution((Expression)((Equation)problemDefinition).Left[0], false);
						 var sol = new ViewFactorDistributeSolution(prob,this);
						 host.AddView(sol);
					 }
					 else if (_action == MathBase.Action.Inequalities)
					 {
						 var prob = new Inequalities((Equation)problemDefinition);
						 var sol = new ViewInequalitiesSolution(prob,this);
						 host.AddView(sol);
					 }
					 #endregion Equation

				 }
				 else if (problemDefinition.GetStepType() == SolveForXStepType.ExpoEquation)
				 {
					 var prob =
					 new SolveForExponentialEquations((ExponentialEquation)problemDefinition);
					 var sol = new ViewExponentialSolution(prob, this);
					 host.AddView(sol);
				 }
				 else if (problemDefinition.GetStepType() == SolveForXStepType.LogEquation)
				 {
					 var prob =
						 new SolveForLogarithmicEquations((LogarithmicEquation)problemDefinition);
					 var sol = new ViewLogarithmicSolution(prob,this);
					 host.AddView(sol);
				 }
			 //}
			 //catch(Exception ex)
			 //{
			 //	int x = 0;
			 //	//MessageBox.Show("Could not solve your problem. Please ensure you entered it correctly, if so, the app can not solve that problem, sorry", "What?", MessageBoxButton.OK);
			 //	//GoodForShow = false;
			 //	// NavigationService.GoBack();
			 //}
		 }

	}
}