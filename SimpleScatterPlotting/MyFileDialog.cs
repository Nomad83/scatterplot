using System;
using System.Windows.Forms;

namespace SimpleScatterPlotting
{

    public class MyFileDialog
    {
        public static string filename;

        public MyFileDialog()
        {
        }

        public static bool ShowDialog()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
                return true;
            }
            return false;
        }
    }

}
