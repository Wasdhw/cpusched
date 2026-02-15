namespace WinFormsApp3
{
    partial class FormFormula
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            lblAlgoTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblDesc = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblFormula = new Guna.UI2.WinForms.Guna2TextBox();
            btnOK = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // lblAlgoTitle
            // 
            lblAlgoTitle.AutoSize = false;
            lblAlgoTitle.BackColor = Color.Transparent;
            lblAlgoTitle.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAlgoTitle.Location = new Point(12, 21);
            lblAlgoTitle.Name = "lblAlgoTitle";
            lblAlgoTitle.Size = new Size(166, 32);
            lblAlgoTitle.TabIndex = 0;
            lblAlgoTitle.Text = "guna2HtmlLabel1";
            // 
            // lblDesc
            // 
            lblDesc.AutoSize = false;
            lblDesc.AutoSizeHeightOnly = true;
            lblDesc.BackColor = Color.Transparent;
            lblDesc.Location = new Point(23, 70);
            lblDesc.Name = "lblDesc";
            lblDesc.Size = new Size(97, 16);
            lblDesc.TabIndex = 1;
            lblDesc.Text = "guna2HtmlLabel1";
            // 
            // lblFormula
            // 
            lblFormula.CustomizableEdges = customizableEdges3;
            lblFormula.DefaultText = "";
            lblFormula.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            lblFormula.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            lblFormula.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            lblFormula.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            lblFormula.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            lblFormula.Font = new Font("Segoe UI", 9F);
            lblFormula.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            lblFormula.Location = new Point(12, 177);
            lblFormula.Name = "lblFormula";
            lblFormula.PlaceholderText = "";
            lblFormula.SelectedText = "";
            lblFormula.ShadowDecoration.CustomizableEdges = customizableEdges4;
            lblFormula.Size = new Size(346, 135);
            lblFormula.TabIndex = 2;
            // 
            // btnOK
            // 
            btnOK.CustomizableEdges = customizableEdges1;
            btnOK.DisabledState.BorderColor = Color.DarkGray;
            btnOK.DisabledState.CustomBorderColor = Color.DarkGray;
            btnOK.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnOK.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnOK.Font = new Font("Segoe UI", 9F);
            btnOK.ForeColor = Color.White;
            btnOK.Location = new Point(113, 335);
            btnOK.Name = "btnOK";
            btnOK.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnOK.Size = new Size(135, 40);
            btnOK.TabIndex = 3;
            btnOK.Text = "OK";
            btnOK.Click += btnOK_Click;
            // 
            // FormFormula
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(370, 396);
            Controls.Add(btnOK);
            Controls.Add(lblFormula);
            Controls.Add(lblDesc);
            Controls.Add(lblAlgoTitle);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormFormula";
            StartPosition = FormStartPosition.Manual;
            Text = "Form2";
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2Button btnOK;
        private Guna.UI2.WinForms.Guna2TextBox lblFormula;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDesc;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblAlgoTitle;
    }
}