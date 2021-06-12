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
using Hermes.UI;

namespace Hermes
{
    public partial class BulleAjDepense2 : UserControl
    {
        private decimal montant;
        private string description;
        private int codeParticipant;
        private int codeEvenement;
        private DateTime dateDepense;


        private Panel pnlBulleEmplacement;
        private Panel pnlPrincipal;

        public BulleAjDepense2(decimal montant, string description, int codeParticipant, int codeEvenement, DateTime dateDepense, Panel pnlPrincipal, Panel pnlBulleEmplacement)
        {
            this.montant = montant;
            this.description = description;
            this.codeEvenement = codeEvenement;
            this.codeParticipant = codeParticipant;
            this.dateDepense = dateDepense;
            this.pnlPrincipal = pnlPrincipal;
            this.pnlBulleEmplacement = pnlBulleEmplacement;
            InitializeComponent();

            pnlBeneficiaire.HorizontalScroll.Enabled = false;
            pnlBeneficiaire.HorizontalScroll.Visible = false;
            pnlBeneficiaire.HorizontalScroll.Maximum = 0;
            pnlBeneficiaire.AutoScroll = true;

        }

        private void BulleAjDepense2_Load(object sender, EventArgs e)
        {
            PartyEvent evenement = PartyEvent.GetFromId(this.codeEvenement);
            List<Participant> guests = evenement.GetGuests();
            for (int i = 0; i < guests.Count; i++)
            {
                CheckBox chkGuest = new CheckBox();
                chkGuest.AutoSize = true;
                chkGuest.Text = guests[i].LastName + " " + guests[i].FirstName;
                chkGuest.Left = chkEveryOne.Left;
                chkGuest.Top = 50 + 30 * i;
                chkGuest.Tag = guests[i].CodeParticipant;
                pnlBeneficiaire.Controls.Add(chkGuest);
            }
        }

        private void ChkEveryOne_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEveryOne.Checked)
            {
                changeAllChk(true);
            }
            else
            {
                changeAllChk(false);
            }
        }

        public void changeAllChk(bool state)
        {
            for (int i = 0; i < pnlBeneficiaire.Controls.Count; i++)
            {
                CheckBox chkGuest = (CheckBox)pnlBeneficiaire.Controls[i];
                chkGuest.Checked = state;
            }

        }

        private void BtnContinuer_Click(object sender, EventArgs e)
        {
            foreach (CheckBox chk in pnlBeneficiaire.Controls)
            {
                if (chk != chkEveryOne)
                {
                    if ((int)chk.Tag == codeParticipant)
                    {
                        chk.Checked = true;
                    }
                }
            }

            
            string comment = rtxtCommentaire.Text;
            if(comment.Length <= 0)
            {
                rtxtCommentaire.BackColor = Color.LightPink;
            }
            else
            {
                Expense newExpenditure = new Expense()
                {
                    Amount = this.montant,
                    Description = this.description,
                    Comment = comment,
                    Date = this.dateDepense.Date,
                    EventId = this.codeEvenement,
                    AuthorId = this.codeParticipant,
                };

                Database.InsertExpense(newExpenditure, getBeneficiary());

                AppToast.CreateSuccessToast("La dépense a été ajoutée !")
                    .SetDurationInSeconds(15)
                    .ShowToast();

                this.pnlBulleEmplacement.Controls.Clear();
                this.pnlBulleEmplacement.Visible = false;

                this.pnlPrincipal.Controls.Clear();
                ViewExpenditures viewExpenditures = new ViewExpenditures(this.codeEvenement);
                viewExpenditures.setPanel = this.pnlPrincipal;
                this.pnlPrincipal.Controls.Add(viewExpenditures);
            }
            
        }

        public List<Participant> getBeneficiary()
        {
            List<Participant> beneficiary = new List<Participant>();
            foreach (CheckBox chk in pnlBeneficiaire.Controls)
            {
                if (chk.Checked && chk != chkEveryOne)
                {
                    beneficiary.Add(Participant.GetParticipant((int)chk.Tag));
                }
            }

            return beneficiary;
        }

        private void btnAnnuler_Click(object sender, EventArgs e)
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            string message = "Voulez vous vraiment annuler l'ajout ?";
            string caption = "";
            result = MessageBox.Show(this, message, caption, buttons,
            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
            MessageBoxOptions.RightAlign);
            if (result == DialogResult.Yes)
            {
                this.pnlBulleEmplacement.Controls.Clear();
                this.pnlBulleEmplacement.Visible = false;
            }
            
        }

        private void RtxtCommentaire_TextChanged(object sender, EventArgs e)
        {
            if(rtxtCommentaire.BackColor == Color.LightPink)
            {
                rtxtCommentaire.BackColor = Color.White;
            }
        }
    }
}