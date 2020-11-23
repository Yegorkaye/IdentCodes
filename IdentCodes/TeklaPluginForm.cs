using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tekla.Structures.Dialog;

namespace IdentCodes
{
    public partial class TeklaPluginForm : PluginFormBase
    {
        public TeklaPluginForm()
        {
            InitializeComponent();
        }

        private void okApplyModifyGetOnOffCancel1_ApplyClicked(object sender, EventArgs e)
        {
            this.Apply();
        }

        private void okApplyModifyGetOnOffCancel1_CancelClicked(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okApplyModifyGetOnOffCancel1_GetClicked(object sender, EventArgs e)
        {
            this.Get();
        }

        private void okApplyModifyGetOnOffCancel1_ModifyClicked(object sender, EventArgs e)
        {
            this.Modify();
        }

        private void okApplyModifyGetOnOffCancel1_OkClicked(object sender, EventArgs e)
        {
            this.Apply();
            this.Close();
        }

        private void okApplyModifyGetOnOffCancel1_OnOffClicked(object sender, EventArgs e)
        {
            this.ToggleSelection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var selectedPartsEn = new Tekla.Structures.Model.UI.ModelObjectSelector().GetSelectedObjects();
            var selectedParts = new List<Tekla.Structures.Model.Part>();
            while (selectedPartsEn.MoveNext())
            {
                if (selectedPartsEn.Current is Tekla.Structures.Model.Part part)
                    selectedParts.Add(part);
            }

            try
            {
                IdentCodeGenerator.GenerateCodes(selectedParts);
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
