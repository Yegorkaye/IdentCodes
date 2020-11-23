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
using Tekla.Structures.Model;
using ICG = IdentCodes.IdentCodeGenerator;

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

            var selectedParts = GetSelectedParts();

            try
            {
                GenerateCodes(selectedParts);
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private List<Part> GetSelectedParts()
        {
            var selectedPartsEn = new Tekla.Structures.Model.UI
                .ModelObjectSelector().GetSelectedObjects();
            var selectedParts = new List<Part>();
            while (selectedPartsEn.MoveNext())
            {
                if (selectedPartsEn.Current is Part part)
                    selectedParts.Add(part);
            }

            return selectedParts;
        }

        private static void GenerateCodes(List<Part> parts)
        {
            foreach (var part in parts)
                GenerateCode(part);
        }

        private static void GenerateCode(Part part)
        {
            var matString = ICG.GetMaterialStringFromPart(part);

            var identCode = $"I{ICG.GetMaterialClassCode(part)}"    +
                            $"{ICG.GetMaterialTypeCode(part)}"      +
                            $"{ICG.GetToughnessCode(matString)}"    +
                            $"{ICG.GetRandomNumberForPlate(part)}"  +
                            $"{ICG.GetMaterialCode(matString)}"     +
                            $"{ICG.GetProfileCode(part)}";

            WriteCode(part, identCode);
        }

        public static void WriteCode(Part part, string code)
        {
            part.SetUserProperty("M_IDENT_CODE", code);
        }
    }
}
