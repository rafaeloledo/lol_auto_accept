using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lol_auto_accept.util {
	internal class LogForm: Form {
		private TextBox textBox;

		public LogForm() {
			InitializeComponent();
		}

		private void InitializeComponent() {
			this.textBox = new System.Windows.Forms.TextBox();
			this.textBox.Text = "Test";
			this.Controls.Add(this.textBox);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.ShowDialog();
		}
	}
}
