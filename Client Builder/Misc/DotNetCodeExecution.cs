using System;
using System.CodeDom.Compiler;
using System.Windows.Forms;

namespace Client_Builder.Misc
{
    internal static class DotNetCodeExecution
    {
        

        internal static void RowAdder(string lib, DataGridView importLib)
        {
            int rowId = importLib.Rows.Add();
            DataGridViewRow row = importLib.Rows[rowId];
            row.Cells["Column31"].Value = lib;
        }

        internal static void TryLocally(CodeDomProvider codeDomProvider, string source, string platform, string[] referencedAssemblies)
        {
            try
            {
                var compilerOptions = $"/target:winexe /platform:{platform} /optimize- /unsafe";

                var compilerParameters = new CompilerParameters(referencedAssemblies)
                {
                    GenerateExecutable = true,
                    GenerateInMemory = true,
                    CompilerOptions = compilerOptions,
                    TreatWarningsAsErrors = false,
                    IncludeDebugInformation = false,
                };
                var compilerResults = codeDomProvider.CompileAssemblyFromSource(compilerParameters, source);

                if (compilerResults.Errors.Count > 0)
                {
                    foreach (CompilerError compilerError in compilerResults.Errors)
                    {
                        MessageBox.Show(string.Format("{0}\nLine: {1}", compilerError.ErrorText, compilerError.Line), "Code", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        break;
                    }
                }
                else
                {
                    compilerResults = null;
                    MessageBox.Show("Code working !", "Working", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

    }
}
