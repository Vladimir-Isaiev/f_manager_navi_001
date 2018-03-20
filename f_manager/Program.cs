using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace f_manager
{
    delegate void t_del(string st);
    class Program
    {

        //[DllImport("user32.dll", CharSet = CharSet.Unicode)]
        //public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

       
        static void Main(string[] args)
        {
            //MessageBox(new IntPtr(0), "Hello World!", "Hello Dialog", 0);

            

            // read_file rf = new read_file();
            //rf.read_f();
            //rf.read_d();

           
            Window main_l = new Window();
            Window main_r = new Window();
            Read_command r = new Read_command();

           // main_r.next_drive();
          

            Window_manager w_m = new Window_manager(main_l, main_r);
           
                w_m.Navi_m();
           
            Console.ReadKey();




            

        }

       
       
    }
}
