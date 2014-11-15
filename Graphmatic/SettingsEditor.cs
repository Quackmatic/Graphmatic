﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.XPath;
using Graphmatic.Properties;

namespace Graphmatic
{
    public partial class SettingsEditor : Form
    {
        public SettingsEditor()
        {
            InitializeComponent();
            InitializeSettings();
        }

        private void MakeCharBox(TextBox textBox, char defaultValue, Action<char> onChange)
        {
            textBox.ReadOnly = true;
            textBox.Text = defaultValue.ToString();
            textBox.KeyPress += delegate(object sender, KeyPressEventArgs e)
            {
                textBox.Text = e.KeyChar.ToString();
                onChange(e.KeyChar);
            };
            textBox.BackColor = SystemColors.Window;
        }

        private void InitializeSettings()
        {
            colorChooserDefaultPageColor.Color = Settings.Default.DefaultPageColor;
            colorChooserDefaultPageColor.ColorChanged += (sender, e) =>
                Settings.Default.DefaultPageColor = colorChooserDefaultPageColor.Color;

            textBoxUserName.Text = Settings.Default.Username;
            textBoxUserName.TextChanged += (sender, e) =>
                Settings.Default.Username = textBoxUserName.Text;

            MakeCharBox(textBoxDefaultPlottedVariable,
                Settings.Default.DefaultPlottedVariable,
                c => Settings.Default.DefaultPlottedVariable = c);

            MakeCharBox(textBoxDefaultVaryingVariable,
                Settings.Default.DefaultVaryingVariable,
                c => Settings.Default.DefaultVaryingVariable = c);
        }

        private void Options_Load(object sender, EventArgs e)
        {
            
        }

        private void Options_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
