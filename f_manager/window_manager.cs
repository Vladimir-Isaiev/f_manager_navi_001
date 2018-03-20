using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace f_manager
{
    class Window_manager
    {
        //левая  и правая панели
        internal Window w_l;
        internal Window w_r;

        //активная и неактивная панели
        internal Window activ_w;
        internal Window inactiv_w;

        //true -- current w_l, false -- current w_r
        private bool lr;

        //списки директорий/файлов панелей
        List<string> l_list = null;
        List<string> r_list = null;

        //ширина и высота панели
        int len;       
        int rows;           

        public Window_manager(Window l, Window r)
        {
            w_l = l;
            w_r = r;
            lr = false;

            l_list = w_l.Get_list_name();
            r_list = w_r.Get_list_name();
            activ_w = w_r;
            inactiv_w = w_l;
        }


        internal void Navi_m()
        {
            Consol.Next_bkg_col();
            Consol.Prev_bkg_col();

            Print_m(activ_w.pos);
            Nav1_menu1();

        }


        internal void Print_m(int pos0)
        {
            len = Console.WindowWidth / 2 - 3;
            rows = Console.WindowHeight - 9;

            string lin = "-".PadLeft(Console.WindowWidth - 1, '-');

            string menu1 = String.Format("{0}\n", lin) +
                         "F1 - help      F2 - next drive   F3 - read txt    F4 - compare txt    F5 - copy        F6 - move\n" +
                         "F7 - create    F8 - rename       DEL - del        F10 - exit          S - setting      F12 - command line ";
           

            string line_l;
            string line_r;
            string st;
            

            int ind_l = w_l.ind_start;
            int ind_r = w_r.ind_start;

            string l_name = w_l.current_dir.FullName.PadRight(len, ' ');
            string r_name = w_r.current_dir.FullName.PadRight(len, ' ');

            if (l_name.Length > len)
                l_name = l_name.Remove(0, l_name.Length - len);
            if (r_name.Length > len)
                r_name = r_name.Remove(0, r_name.Length - len);

            Console.Clear();
            foreach (DriveInfo dr in w_l.alldrives)
                Console.Write(dr.Name + " (" + dr.VolumeLabel + ")\t");

            Console.WriteLine("\n" + lin + "\n" +l_name + " | " + r_name + "\n" + lin + "\n");


            for (int k = 0; k < rows; ++k)
            {
                if ((ind_l > w_l.ind_end && ind_r > w_r.ind_end) || (ind_l >= w_l.list_all.Count && ind_r >= w_r.list_all.Count))
                    break;


                if (ind_l == -1)
                    line_l = ". ..";
                else
                {
                    if (ind_l < l_list.Count)
                    {
                        st = string.Format("{0}", (ind_l + 1).ToString()) + ". ";

                        line_l = string.Concat(st, l_list[ind_l]);
                        if (line_l.Length > len)
                            line_l = line_l.Remove(len);
                    }
                    else
                        line_l = " ";
                }
                line_l = line_l.PadRight(len, ' ');

                if (ind_r == -1)
                    line_r = ". ..";
                else
                {
                    if (ind_r < r_list.Count)
                    {
                        st = string.Format("{0}", (ind_r + 1).ToString()) + ". ";

                        line_r = string.Concat(st, r_list[ind_r]);
                        if (line_r.Length > len)
                            line_r = line_r.Remove(len);
                    }
                    else
                        line_r = " ";
                }
                line_r = line_r.PadRight(len, ' ');


                //меняем цвет и фон курсора
                if (lr && ind_l == pos0)
                {
                    Consol.Next_bkg_col();
                    Consol.Next_frg_col();
                    Console.Write(line_l);
                    Consol.Prev_bkg_col();
                    Consol.Prev_frg_col();
                    Console.Write(" | ");
                    Console.WriteLine(line_r);
                }
                else
                    if (!lr && ind_r == pos0)
                {
                    Console.Write(line_l);
                    Console.Write(" | ");
                    Consol.Next_bkg_col();
                    Consol.Next_frg_col();
                    Console.WriteLine(line_r);
                    Consol.Prev_bkg_col();
                    Consol.Prev_frg_col();
                }
                else
                    Console.WriteLine(line_l + " | " + line_r);//без изменения цвета

                ++ind_l;
                ++ind_r;

            }
            Console.WriteLine(menu1);
        }


        //установка актуальных значений
        internal void Set_variables()
        {
            if(lr)
            {
                activ_w = w_l;
                inactiv_w = w_r;
            }
            else
            {
                activ_w = w_r;
                inactiv_w = w_l;
            }

            l_list = w_l.Get_list_name();
            r_list = w_r.Get_list_name();
        }


        //обработка событий окна
        internal void Nav1_menu1()
        {
            ConsoleKeyInfo inp;
            do
            {
                inp = Console.ReadKey();

                switch (inp.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (activ_w.Up())
                            Print_m(activ_w.pos);
                        break;

                    case ConsoleKey.DownArrow:
                        if (activ_w.Down())
                            Print_m(activ_w.pos);
                        break;

                    case ConsoleKey.Enter:
                        if (activ_w.Select())
                        {
                            Set_variables();
                            Print_m(activ_w.pos);
                        }
                        break;

                    case ConsoleKey.F1://man
                        activ_w.Mann(new List<string> { "man" }, null);
                        Print_m(activ_w.pos);
                        break;

                    case ConsoleKey.F2://next drive
                        activ_w.Next_drive();
                        Set_variables();
                        Print_m(activ_w.pos);
                        break;

                    case ConsoleKey.F3://read .txt
                        if (activ_w.pos >= 0)
                        {
                            string name_file = activ_w.list_all[activ_w.pos].FullName;
                            List<string> ext = null;
                            if (activ_w.Read(new List<string> { "read", name_file }, out ext))
                            {
                                Console.Clear();
                                foreach (string st in ext)
                                    Console.WriteLine(st);
                                Console.WriteLine("press any key");
                                Console.ReadKey();
                                Print_m(activ_w.pos);
                            }
                            Print_m(activ_w.pos);
                        }
                        break;

                    case ConsoleKey.F4://compare .txt
                        break;
                    case ConsoleKey.F5://copy
                        if (activ_w.pos == -1)
                            break;

                        string dest_dir = inactiv_w.current_dir.FullName;
                        string sours = activ_w.list_all[activ_w.pos].FullName;
                        string f;
                        //int ll = w_l.sub_dir_list.Count();

                        if (activ_w.pos < activ_w.sub_dir_list.Count())
                            f = "d";
                        else
                            f = "f";
                        
                        if (activ_w.Copy(new List<string> { "copy", sours, dest_dir, f }))
                        {
                            //ll = w_l.sub_dir_list.Count();
                            Set_variables();
                            inactiv_w.Refresh();
                            Print_m(activ_w.pos);
                         }
                        break;

                    case ConsoleKey.F6://move
                        string dest_dir1 = inactiv_w.current_dir.FullName;
                        string sours1 = activ_w.list_all[activ_w.pos].FullName;
                        string f1;
                        if (activ_w.pos < activ_w.sub_dir_list.Count())
                            f1 = "d";
                        else
                            f1 = "f";
                        if (activ_w.Copy(new List<string> { "copy", sours1, dest_dir1, f1 }))
                        {
                           if (f1 == "d")
                                ((DirectoryInfo)activ_w.list_all[activ_w.pos]).Delete(true);
                            else
                                activ_w.list_all[activ_w.pos].Delete();
                            Set_variables();
                            inactiv_w.Refresh();
                            activ_w.Refresh();
                            Print_m(activ_w.pos);
                        }
                        break;

                    case ConsoleKey.F7://create
                        Console.Clear();
                        Console.WriteLine("dir - press d, file -- press f");
                        ConsoleKeyInfo t = Console.ReadKey();
                        Console.WriteLine("name?");
                        string name = Console.ReadLine();

                        if (activ_w.Create(new List<string> { "create", name, t.KeyChar.ToString() }))
                        {
                            Set_variables();
                            Print_m(activ_w.pos);
                        }
                        break;

                    case ConsoleKey.F8://rename
                        Console.Clear();
                        Console.WriteLine("name?");
                        string name1 = Console.ReadLine();

                        if (activ_w.pos < activ_w.sub_dir_list.Count())
                            f = "d";
                        else
                            f = "f";

                        if (activ_w.Rename(new List<string> { "rename", activ_w.list_all[activ_w.pos].FullName, name1, f }))
                        {
                            Set_variables();
                            Print_m(activ_w.pos);
                        }
                        break;

                    case ConsoleKey.Delete://del
                        if (activ_w.pos < activ_w.sub_dir_list.Count())
                            f = "d";
                        else
                            f = "f";

                        if (activ_w.pos != -1 && activ_w.Del(new List<string> { "del", activ_w.list_all[activ_w.pos].FullName, f }))
                        {
                            Set_variables();
                            --activ_w.pos;
                            Print_m(activ_w.pos);
                        }
                        break;

                    case ConsoleKey.F10://exit
                        Environment.Exit(0);
                        break;

                    case ConsoleKey.S://setting
                        activ_w.Set_cons();
                        Print_m(activ_w.pos);
                        break;

                    case ConsoleKey.F12://command line
                        activ_w.Navi();
                        Set_variables();
                        inactiv_w.Refresh();
                        Print_m(activ_w.pos);
                        break;

                    case ConsoleKey.Tab:
                        lr = !lr;
                        Set_variables();
                        Print_m(activ_w.pos);
                        break;
                }

            } while (true);
        }





    }
}
