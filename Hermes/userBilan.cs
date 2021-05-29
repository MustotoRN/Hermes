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
    public partial class UserBilan : UserControl
    {
        private PartyEvent currentEvent;
        public Panel ecran;
        public Panel setPanel
        {
            set { this.ecran = value; }
        }

        public UserBilan(PartyEvent currentEvent)
        {
            InitializeComponent();
            this.currentEvent = currentEvent;
            pnlDepense.HorizontalScroll.Enabled = false;
            pnlDepense.HorizontalScroll.Visible = false;
            pnlDepense.HorizontalScroll.Maximum = 0;
            pnlDepense.AutoScroll = true;

            pnlRemboursement.HorizontalScroll.Enabled = false;
            pnlRemboursement.HorizontalScroll.Visible = false;
            pnlRemboursement.HorizontalScroll.Maximum = 0;
            pnlRemboursement.AutoScroll = true;
        }


        private void Bilan_Load(object sender, EventArgs e)
        {
            List<Participant> listeParticipants = currentEvent.GetGuests();
            DataTable dataTableParticipants = Participant.toConcatenateDataTable(listeParticipants);
            cboParticipant.DataSource = dataTableParticipants;
            cboParticipant.DisplayMember = "Name";
            cboParticipant.ValueMember = "CodeParticipant";
        }

        private void depenseUser1_Load(object sender, EventArgs e)
        {

        }

        private void CboParticipant_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualisationDepense();
            ActualisationRemboursement();
        }

        private void ActualisationDepense()
        {
            pnlDepense.Controls.Clear();
            List<Participant> listeParticipants = currentEvent.GetGuests();
            Participant participant = listeParticipants[cboParticipant.SelectedIndex];
            List<UserSpendingRecord> listeDepense = Database.QuerySpendings(currentEvent.Id, participant.CodeParticipant);
            Decimal totalAmount = 0;
            foreach (UserSpendingRecord userSpending in listeDepense)
                totalAmount += userSpending.Amount;

            lblTotalDepnse.Text = totalAmount.ToString() + "€";
            for (int i = 0; i < listeDepense.Count; i++)
            {
                DepenseUser depenseUser = new DepenseUser(listeDepense[i].Date, listeDepense[i].Description, listeDepense[i].Amount);
                Point position = new Point(3, 0 + 129 * i);
                depenseUser.Location = position;
                pnlDepense.Controls.Add(depenseUser);
            }
        }

        private void ActualisationRemboursement()
        {
            pnlRemboursement.Controls.Clear();
            List<Participant> listeParticipants = currentEvent.GetGuests();
            Participant participant = listeParticipants[cboParticipant.SelectedIndex];
            List<UserParticipationRecord> listeRemboursement = Database.QueryParticipation(currentEvent.Id, participant.CodeParticipant);
            Decimal totalAmount = 0;
            foreach (UserParticipationRecord userParticipationRecord in listeRemboursement)
                totalAmount += userParticipationRecord.Amount;

            lblTotalRemboursement.Text = totalAmount.ToString() + "€";
            for (int i = 0; i < listeRemboursement.Count; i++)
            {
                RemboursementUser remboursementUser = new RemboursementUser(listeRemboursement[i].ExpenseTotalShares, listeRemboursement[i].Amount);
                Point position = new Point(1, 0 + 129 * i);
                remboursementUser.Location = position;
                pnlRemboursement.Controls.Add(remboursementUser);
            }
        }

        private void PnlDepense_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
