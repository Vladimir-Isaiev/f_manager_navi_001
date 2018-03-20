using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace f_manager
{
      static class Consol
    {
        static private ConsoleColor[] colors;
        static private int current_bkg;
        static private int current_frg;

        static private int saveBufferWidth;
        static private int saveBufferHeight;
        static private int saveWindowHeight;
        static private int saveWindowWidth;

        static Consol()
        {
            colors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));

            saveBufferWidth = Console.BufferWidth;
            saveBufferHeight = Console.BufferHeight;
            saveWindowHeight = Console.WindowHeight;
            saveWindowWidth = Console.WindowWidth;

            current_bkg = 7;
            current_frg = 0;

            Console.BackgroundColor = colors[current_bkg];
            Console.ForegroundColor = colors[current_frg];
            Console.Clear();
        }



        static public void Next_bkg_col()
        {
            if (current_bkg == colors.Length - 1)
                current_bkg = 0;
            else
                ++current_bkg;

            Console.BackgroundColor = colors[current_bkg];
            

        }

        static public void Next_frg_col()
        {
            if (current_frg == colors.Length - 1)
                current_frg = 0;
            else
                ++current_frg;

            Console.ForegroundColor = colors[current_frg];
            
        }


        static public void Prev_bkg_col()
        {
            if (current_bkg == 0)
                current_bkg = colors.Length - 1;
            else
                --current_bkg;

            Console.BackgroundColor = colors[current_bkg];
        }

        static public void Prev_frg_col()
        {
            if (current_frg == 0)
                current_frg = colors.Length - 1;
            else
                --current_frg;

            Console.ForegroundColor = colors[current_frg];
        }


        //высота окна
        static public void Set_h(string s)
        {
            int rows = -1;
            try
            {
                rows = Convert.ToInt32(s);
            }
            catch
            {
                return;
            }
            if (rows > 14 && rows <= Console.LargestWindowHeight)
                Console.WindowHeight = rows;
            
        }


        //ширина окна
        static public void Set_w(string s)
        {
            int col = -1;
            try
            {
                col = Convert.ToInt32(s);
            }
            catch
            {
                return;
            }
            if (col > 29 && col <= Console.LargestWindowWidth)
                Console.WindowWidth = col;
           
        }
    }
}
