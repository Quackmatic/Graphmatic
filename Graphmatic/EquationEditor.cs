﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Graphmatic.Expressions;
using Graphmatic.Expressions.Parsing;
using Graphmatic.Interaction;
using Graphmatic.Interaction.Plotting;

namespace Graphmatic
{
    public partial class EquationEditor : Form
    {
        public Equation Equation
        {
            get;
            protected set;
        }

        public EquationEditor()
            : this(new Equation('y', 'x'))
        {
        }

        private void RefreshDisplay()
        {
            toolTip.SetToolTip(expressionDisplay, Equation.ParseTree.ToString());
            expressionDisplay.Refresh();
        }

        public EquationEditor(Equation equation)
        {
            InitializeComponent();
            Equation = equation;
            expressionDisplay.Expression = Equation.Expression;
            textBoxPlotted.Text = Equation.VerticalVariable.ToString();
            textBoxVarying.Text = Equation.HorizontalVariable.ToString();
            RefreshDisplay();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            ExpressionEditor inputWindow = new ExpressionEditor(Equation);
            inputWindow.Verify += inputWindow_Verify;

            if (inputWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                RefreshDisplay();
        }

        private void inputWindow_Verify(object sender, ExpressionVerificationEventArgs e)
        {
            try
            {
                e.Equation.Parse();
            }
            catch (ParseException ex)
            {
                MessageBox.Show(ex.Message, "Equation Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (ex.Cause != null)
                {
                    e.Cursor.Expression = ex.Cause.Parent;
                    e.Cursor.Index = ex.Cause.IndexInParent();
                }
                e.Failure = true;
            }
        }

        private void textBoxPlotted_KeyPress(object sender, KeyPressEventArgs e)
        {
            textBoxPlotted.Text = e.KeyChar.ToString();
            Equation.VerticalVariable = e.KeyChar;
        }

        private void textBoxVarying_KeyPress(object sender, KeyPressEventArgs e)
        {
            textBoxVarying.Text = e.KeyChar.ToString();
            Equation.HorizontalVariable = e.KeyChar;
        }

        private void EquationEditor_Load(object sender, EventArgs e)
        {
            Text = Equation.Name + " - Equation Editor";
            buttonEdit.Select();
        }

        private void EquationEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            // MessageBox.Show(e.CloseReason.ToString());
        }

        private void expressionDisplay_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            PictureBox pbDisplay = new PictureBox();
            pbDisplay.Size = ClientSize;
            pbDisplay.Location = Point.Empty;
            pbDisplay.Click += (sender2, e2) =>
            {
                Controls.Remove(pbDisplay);
                pbDisplay.Visible = false;
                pbDisplay.Dispose();
            };

            Graph graph = new Graph();
            graph.Add(Equation, Color.Red);

            PlotParameters parameters = new PlotParameters()
            {
                CenterHorizontal = 0,
                CenterVertical = 0,
                HorizontalPixelScale = 0.1,
                VerticalPixelScale = 0.1
            };

            Image image = graph.ToImage(pbDisplay.ClientSize, parameters);
            pbDisplay.Image = image;

            Controls.Add(pbDisplay);
            pbDisplay.BringToFront();
        }

        private void expressionDisplay_Load(object sender, EventArgs e)
        {

        }
    }
}
