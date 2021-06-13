﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hermes.DataModel;

namespace Hermes
{
    public partial class AjNouvelleDepense : UserControl
    {
        Panel ecran;
        int indice;
        private bool wentBack;
        private DateTime date;
        private string description;
        private int codePayeur;
        private Decimal amount;
        private int indicePayePar;
        public AjNouvelleDepense(int indice)
        {
            InitializeComponent();
            this.indice = indice;
        }

        public AjNouvelleDepense(bool wentBack, int codeEvenement, DateTime date, string description, int codePayeur, Decimal amount, int indiceEvenement, int indicePayePar, Panel ecran)
        {
            InitializeComponent();
            this.indice = codeEvenement;
            this.wentBack = wentBack;
            this.date = date;
            this.description = description;
            this.codePayeur = codePayeur;
            this.amount = amount;
            this.indicePayePar = indicePayePar;
            this.ecran = ecran;

        }

        public Panel setPanel
        {
            set { this.ecran = value; }
        }

        private void AjNouvelleDepense_Load(object sender, EventArgs e)
        {
            FontFamily helvetica = FontManager.GetFontFamily(AppFont.HelveticaNeue);
            cboEvenements.Font = new Font(helvetica, cboEvenements.Font.Size);
            dtp.Font = new Font(helvetica, dtp.Font.Size);
            txtWhere.Font = new Font(helvetica, txtWhere.Font.Size);
            cboPayePar.Font = new Font(helvetica, cboPayePar.Font.Size);
            numAmount.Font = new Font(helvetica, numAmount.Font.Size);

            DataTable table = Database.FetchUncompletedEvents().ToDataTable();
            cboEvenements.DataSource = table;
            cboEvenements.DisplayMember = "Name";
            cboEvenements.ValueMember = "Id";
            cboEvenements.SelectedIndex = indice - 2;

            PartyEvent selectedEvent = PartyEvent.GetFromId(int.Parse(cboEvenements.SelectedValue.ToString()));

            updateGuests();
            if (!wentBack)
            {
                dtp.Value = selectedEvent.StartDate;
            }
            dtp.MinDate = selectedEvent.StartDate;
        }

        public void updateGuests()
        {
            PartyEvent selectedEvent = PartyEvent.GetFromId(int.Parse(cboEvenements.SelectedValue.ToString()));
            DataTable guests = Participant.toConcatenateDataTable(selectedEvent.GetGuests());
            cboPayePar.DataSource = guests;
            cboPayePar.DisplayMember = "name";
            cboPayePar.ValueMember = "codeParticipant";
            if (wentBack)
            {
                txtWhere.Text = this.description;
                cboEvenements.SelectedValue = indice;
                cboPayePar.SelectedIndex = indicePayePar;
                numAmount.Value = amount;
                dtp.Value = date;
            }
        }

        private void appFontLabel7_Click(object sender, EventArgs e)
        {

        }

        private void appFontLabel7_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void lblAnnuler_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void cboEvenements_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PartyEvent selectedEvent = PartyEvent.GetFromId(int.Parse(cboEvenements.SelectedValue.ToString()));

            dtp.MinDate = DateTime.Parse("01/01/2000");
            dtp.MaxDate = DateTime.Parse("01/01/2200");
            dtp.Value = selectedEvent.StartDate;
            dtp.MinDate = selectedEvent.StartDate;
            dtp.MaxDate = selectedEvent.EndDate;

            updateGuests();
        }

        private void AppFontLabel8_Click(object sender, EventArgs e)
        {

        }


        private void AppFontLabel8_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void AppFontLabel8_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void txtWhere_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtWhere.BackColor == Color.LightPink)
                txtWhere.BackColor = Color.White;
        }

        private void numAmount_ValueChanged(object sender, EventArgs e)
        {
            if (numAmount.BackColor == Color.LightPink)
                numAmount.BackColor = Color.White;
        }

        private void numAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (numAmount.BackColor == Color.LightPink)
                numAmount.BackColor = Color.White;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            string message = "Voulez vous vraiment annuler l'ajout ?";
            string caption = "";
            result = result = MessageBox.Show(this, message, caption, buttons,
            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
            MessageBoxOptions.RightAlign);

            if (result == DialogResult.Yes)
            {
                this.ecran.Controls.Clear();
                Accueil a = new Accueil();
                a.setPanel = this.ecran;
                this.ecran.Controls.Add(a);
            }

        }

        private void BtnContinue_Click(object sender, EventArgs e)
        {
            bool done = true;
            PartyEvent currentEvent = PartyEvent.GetFromId(int.Parse(cboEvenements.SelectedValue.ToString()));

            //Les animations d'erreur sont juste de test ! Il faudra changer ABSOLUMENT !
            txtWhere.BackColor = Color.White;
            numAmount.BackColor = Color.White;

            if (txtWhere.Text == "")
            {
                done = false;
                txtWhere.Focus();
                txtWhere.BackColor = Color.LightPink;
            }
            if (numAmount.Value == 0)
            {
                done = false;
                numAmount.Focus();
                numAmount.BackColor = Color.LightPink;
            }

            if (done)
            {
                string description = txtWhere.Text.Replace('\'', ' ');

                this.ecran.Controls.Clear();
                AjNouvelleDepense2 suite = new AjNouvelleDepense2(int.Parse(cboEvenements.SelectedValue.ToString()), dtp.Value, description, int.Parse(cboPayePar.SelectedValue.ToString()), numAmount.Value, indice, cboPayePar.SelectedIndex);
                suite.setPanel = this.ecran;
                this.ecran.Controls.Add(suite);
            }
        }
    }
}
