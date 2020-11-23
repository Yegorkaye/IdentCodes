using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Plugins;

namespace IdentCodes
{
    [Plugin("Назначить ident-коды")]
    [PluginUserInterface("IdentCodes.TeklaPluginForm")]
    public class IdentCodes : PluginBase
    {
        private TeklaPluginData Data { get; set; }

        public IdentCodes(TeklaPluginData data)
        {
            Data = data;
        }
        public override List<InputDefinition> DefineInput()
        {
            var input = new List<InputDefinition>();
            return input;
        }

        public override bool Run(List<InputDefinition> Input)
        {
            try
            {
                
            }
            catch (Exception exc)
            {

            }
            return true;
        }
    }
}
