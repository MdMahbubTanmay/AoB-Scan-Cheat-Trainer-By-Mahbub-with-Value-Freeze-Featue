using Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MyCheat
{
    public partial class Form1 : Form
    {
        Mem mem = new Mem();
        static string ProcessName = "Ld9BoxHeadless"; // Name of the emulator process



        List<long> Antenna_address = new List<long>(); //Add A list for address here


        public Form1()
        {
            InitializeComponent();
            //On Form Created

        }




        private async void button1_Click(object sender, EventArgs e)  //Modify Address Button
        {
            // Try to find emulator process
            Process? emulator = Process.GetProcessesByName(ProcessName).FirstOrDefault();
            if (emulator == null)
            {
                MessageBox.Show("Emulator (LdPlayer) not found.");
                return;
            }

            if (!mem.OpenProcess(emulator.Id))
            {
                MessageBox.Show("Failed to open emulator process.");
                return;
            }

            string aobPattern = "6B 70 D8 BF"; // Replace with your AOB pattern

            try
            {
                var scanResult = await mem.AoBScan(aobPattern, true, true);


                //Use A Variable to store the addresses. Here I used  Antenna_address
                Antenna_address = scanResult.ToList();

                if (Antenna_address.Count == 0) //use ur address variable.Here I used  Antenna_address
                {
                    MessageBox.Show("No addresses found.");
                }
                else
                {
                    MessageBox.Show($"{Antenna_address.Count} address(es) found and stored."); // update  Antenna_address with ur Variable
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("AOB Scan Error: " + ex.Message);
            }
        }






        private void button2_Click(object sender, EventArgs e) //Modify Address Button
        {
            if (Antenna_address.Count == 0)  //use ur address variable.Here I used  Antenna_address
            {
                MessageBox.Show("No addresses stored. Please scan first.");
                return;
            }

            foreach (var address in Antenna_address) //use ur address variable.Here I used  Antenna_address
            {
                bool success = mem.WriteMemory("0x" + address.ToString("X"), "float", "9999999"); // You can change type/value here. we can also write like  "bytes", "D8 70 D8 BF"
                if (!success)
                    MessageBox.Show($"Failed to write to address 0x{address:X}");
            }

            MessageBox.Show("All addresses updated.");
        }





        private void button3_Click(object sender, EventArgs e) //Freeze Button
        {

            if (Antenna_address.Count == 0) //use ur address variable.Here I used  Antenna_address
            {
                MessageBox.Show("No addresses stored. Please scan first.");
                return;

            }
            else
            {
               //This is the timer which will help us freezeing values.
                System.Windows.Forms.Timer
                AntennaFreezer = new System.Windows.Forms.Timer();
                AntennaFreezer.Interval = 500; // 0.5 seconds
                AntennaFreezer.Tick += (s, ev) =>
                 {
                  foreach (var address in Antenna_address) //use ur address variable.Here I used  Antenna_address
                     {
                        mem.WriteMemory("0x" + address.ToString("X"), "float", "9999999"); // You can change type/value here
                        
                     }
                 };

                 AntennaFreezer.Start();  
            }
          

        }
    }
}
