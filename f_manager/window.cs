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
    class Window
    {
        internal List<DriveInfo> alldrives;
        internal int current_drive;
        private List<DirectoryInfo> roots;

        internal DirectoryInfo current_dir;
        internal IEnumerable<DirectoryInfo> sub_dir_list;
        internal IEnumerable<FileInfo> file_list;
       
        internal int pos;//позиция курсора
        internal int ind_start;//начало отображаемого списка
        internal int ind_end;//конец отображаемого списка

        internal List<FileSystemInfo> list_all;//общий список -- директории и файлы
       

        public Window()
        {
            DriveInfo[] temp = DriveInfo.GetDrives();
            list_all = new List<FileSystemInfo>();
            alldrives = new List<DriveInfo>();

            for(int i = 0; i < temp.Length; ++i)
                try
                {
                    string st = temp[i].VolumeLabel;
                    alldrives.Add(temp[i]);
                }
                catch
                {
                    temp[i] = null;
                }

            current_drive = 0;
            roots = new List<DirectoryInfo>();
            current_dir = new DirectoryInfo(alldrives[0].RootDirectory.Name);
            Directory.SetCurrentDirectory(current_dir.Name);

            sub_dir_list = current_dir.EnumerateDirectories();
            file_list = current_dir.EnumerateFiles();


            foreach (DriveInfo dr in alldrives)
                roots.Add(dr.RootDirectory);

            ind_start = -1;
            ind_end = ind_start + 21;
            pos = -1;

            foreach (DirectoryInfo dir in sub_dir_list)
            {
                list_all.Add(dir);
            }

            foreach (FileInfo file in file_list)
            {
                list_all.Add(file);
            }
            
        }

        public void Next_drive()
        {
           if (current_drive == alldrives.Count - 1)
                current_drive = 0;
            else
                ++current_drive;

            current_dir = alldrives[current_drive].RootDirectory;
            Directory.SetCurrentDirectory(current_dir.Name);

            
            Refresh();
            pos = -1;
           
        }

        internal void Refresh()
        {
            sub_dir_list = current_dir.EnumerateDirectories();
            file_list = current_dir.EnumerateFiles();

            list_all.Clear();
            foreach (DirectoryInfo dir in sub_dir_list)
            {
                list_all.Add(dir);
            }

            foreach (FileInfo file in file_list)
            {
                list_all.Add(file);
            }
            
        }

        internal List<string> Get_list_name()
        {
          List<string> w = new List<string>();

            foreach (DirectoryInfo dri in sub_dir_list)
                w.Add(dri.Name);
            foreach (FileInfo fi in file_list)
                w.Add(fi.Name);
           
            return w;
        }

        internal bool Up()
        {
            if (pos > -1)
            {
                --pos;
                if (pos < ind_start)
                {
                    --ind_start;
                    --ind_end;
                }
                return true;
            }
            else
                return false; 

            
        }
          
        internal bool Down()
        {
            if (pos < list_all.Count - 1)
            {
                ++pos;

                if (pos >= ind_end)
                {
                    ++ind_start;
                    ++ind_end;
                }
                return true;
            }
            else
                return false;
        }          
                    
         internal bool Select()
        {
           if (pos == -1)
            {
                if (current_dir.FullName == alldrives[current_drive].RootDirectory.FullName)
                    return false;

                current_dir = current_dir.Parent;
                Directory.SetCurrentDirectory(current_dir.FullName);
            }
            else
            {
                if (pos < sub_dir_list.Count())
                {
                    try
                    {
                        current_dir = (DirectoryInfo)list_all[pos];
                        Directory.SetCurrentDirectory(current_dir.FullName);
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                    return false;
            }

            Refresh();
            pos = -1;
            ind_start = -1;
            ind_end = Console.WindowHeight - 11;
           
            return true;
        }          
                    
               
        //работа с командной строки
        internal void Navi()
        {
            List<string> inp;
           
                inp = Read_command.Read(current_dir.FullName);

              switch (inp[0])
                {
                    case "exit":
                        Environment.Exit(0);
                        break;

                    case "copy":
                        Copy(inp);
                        break;

                    case "move":
                        Move(inp);
                        break;

                    case "create":
                        Create(inp);
                        break;

                    case "read":
                    List<string> t = null;
                        
                   if (Read(inp, out t))
                    {
                        Console.Clear();
                        foreach (string st in t)
                            Console.WriteLine(st);
                        Console.WriteLine("press any key");
                        Console.ReadKey();
                    }
                    break;

                    case "restart":
                        Restart();
                        break;

                    case "find":
                        List<DirectoryInfo> di = new List<DirectoryInfo>();
                        List<FileInfo> fi = new List<FileInfo>();
                        int ind = 1;
                        ConsoleKeyInfo inp_key;

                        Find0(inp, di, fi);
                        Console.Clear();

                        foreach(DirectoryInfo d in di)
                           try
                            {
                                Console.WriteLine(ind++.ToString() + " " + d.FullName);
                            }
                            catch
                            {
                                Console.WriteLine("long name");
                            }
                        foreach (FileInfo f in fi)
                            try
                            {
                                Console.WriteLine(ind++.ToString() + " " + f.FullName);
                            }
                            catch
                            {
                                Console.WriteLine("long name");
                            }

                        Console.WriteLine("for go to a file or dir -- press space, go back -- any key");
                        inp_key = Console.ReadKey(true);

                        if (inp_key.Key == ConsoleKey.Spacebar)
                            Cd0(di, fi);

                        Refresh();
                        pos = 0;
                        ind_start = 0;
                        ind_end = ind_start + 21;
                    break;

                    case "rename":
                        Rename(inp);
                        break;

                    case "del":
                        Del(inp);
                        break;

                    case "compare":
                        //if(comp(inp))
                        //    Console.WriteLine("Y");
                        //else
                        //    Console.WriteLine("N");
                        //Console.WriteLine("press any key");
                        //Console.ReadKey();
                        break;

                    case "cd":
                        Cd(inp);

                    Refresh();


                    pos = -1;
                    ind_start = -1;
                    ind_end = ind_start + 21;
                    break;

                    case "cdd":
                        Next_drive();

                    Refresh();
                    pos = -1;

                    ind_start = -1;
                    ind_end = ind_start + 21;
                    break;

                    case "set":
                        Set_cons();
                        break;

                    case "man":
                    Mann(inp, null);
                            
                        break;
                }

           
        }

        internal bool Copy(List<string> l)
        {
            FileInfo sours_file = null;
            DirectoryInfo sours_dir = null;
            DirectoryInfo dest_dir = null;
            FileInfo new_file = null;
            
            if (l.Count != 4)
            {
                Mann(null, l[0]);
                   
                return false;
            }

            try
            {
                if (l[3] == @"d")
                {
                    sours_dir = new DirectoryInfo(l[1]);
                    dest_dir = new DirectoryInfo(l[2]);


                    if (!dest_dir.Exists)
                        dest_dir.Create();

                    Copy1(sours_dir, dest_dir);
                }
                else
                    if (l[3] == @"f")
                {
                    sours_file = new FileInfo(l[1]);
                    dest_dir = new DirectoryInfo(l[2]);

                    if (!dest_dir.Exists)
                        dest_dir.Create();

                    string t = dest_dir.FullName + @"\" + sours_file.Name;
                    new_file = sours_file.CopyTo(t);
                    FileStream fs_temp = new FileStream(new_file.FullName, FileMode.Open);
                    fs_temp.Close();

                    dest_dir.Refresh();
                    dest_dir.Attributes = FileAttributes.Normal;
                 }
                else
                    return false;
               
                Refresh();
               
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(current_dir.Name + "copy press any key");
                Console.ReadKey();
                return false;
            }
         }


        //копирование содержимого директории
        internal void Copy1(DirectoryInfo sours_dir, DirectoryInfo dest_dir)
        {
            IEnumerable<DirectoryInfo> sub_dir_list1 = null;
            IEnumerable<FileInfo> file_list1 = null;
            DirectoryInfo new_dir = null;
            
            FileInfo new_file = null;
            try
            {
                sub_dir_list1 = sours_dir.EnumerateDirectories();
                file_list1 = sours_dir.EnumerateFiles();

                new_dir = dest_dir.CreateSubdirectory(sours_dir.Name);

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(current_dir.Name + "copy1 press any key");
                Console.ReadKey();
            }


            foreach (FileInfo fi in file_list1)
            {
                new_file = fi.CopyTo(new_dir.FullName + @"\" + fi.Name);
                try
                {
                    FileStream temp = new FileStream(new_file.FullName, FileMode.Open);
                    temp.Close();
                }
                catch
                {
                    Console.WriteLine("Possible problem");
                }
                
            }

            foreach (DirectoryInfo di in sub_dir_list1)
                Copy1(di, new_dir);
           
        }

        internal bool Move(List<string> l)
        {
            FileInfo sours_file = null;
            DirectoryInfo sours_dir = null;
            DirectoryInfo dest_dir = null;
            
            if (l.Count != 4)
            {
                Mann(null, l[0]);
                    
                return false;
            }

            try
            {
                if(l[3] == @"d")
                {
                    sours_dir = new DirectoryInfo(l[1]);
                    dest_dir = new DirectoryInfo(l[2]);

                    if (!dest_dir.Exists)
                        dest_dir.Create();

                    sours_dir.MoveTo(dest_dir.FullName + @"\" + sours_dir.Name);
                }
                else
                if(l[3] == @"f")
                {
                    sours_file = new FileInfo(l[1]);
                    dest_dir = new DirectoryInfo(l[2]);

                    if (!dest_dir.Exists)
                        dest_dir.Create();

                    sours_file.MoveTo(dest_dir.FullName + @"\" + sours_file.Name);
                }
                else
                    return false;

                Refresh();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(current_dir.Name + "copy press any key");
                Console.ReadKey();
                return false;
            }
       }

        internal bool Create(List<string> l)
        {
            FileInfo dest_file = null;
            DirectoryInfo dest_dir = null;
            FileStream fs_temp = null; ;

            if (l.Count != 3)
            {
                Mann(null, l[0]);
                    
                return false;
            }

            try
            {
                if (l[2] == "d")
                {
                    dest_dir = new DirectoryInfo(l[1]);
                    dest_dir.Create();
                }
                else
                if (l[2] == "f")
                {
                    dest_file = new FileInfo(l[1]);
                    fs_temp = dest_file.Create();

                    if (fs_temp != null)
                        fs_temp.Close();
                }
                else
                    return false;

                Refresh();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("press any key");
                Console.ReadKey();
                return false;
            }
        }


        //запуск поиска
        internal void Find0(List<string> l, List<DirectoryInfo> di, List<FileInfo> fi)
        {
            if (l.Count == 3)
                foreach (DirectoryInfo d in roots)
                    Find(l, d, di, fi, 0);
            else
            {
                Mann(null, l[0]);
                  
                return;
            }
        }

        //поиск
        internal void Find(List<string> l, DirectoryInfo curent_dir, List<DirectoryInfo> di, List<FileInfo> fi, int c)
        {
            if(c<=3)
            {
                Console.Clear();
                Console.WriteLine(curent_dir.FullName);
            }
            ++c;
           

            IEnumerable<DirectoryInfo> dir; 
            IEnumerable<FileInfo> fil;
            string name = l[1];
            int ind = -1;
            string temp;

            try
                {
                    dir = curent_dir.EnumerateDirectories();
                 }
                catch
                {
                    return;
                }
             
            fil = curent_dir.EnumerateFiles();

            if(l[2] == "f")
            {
                if (name[0] == '*' && name[1] == '.')
                {
                    foreach (FileInfo f in fil)
                    {
                        try
                        {
                            if (f.Extension == name.Substring(1))
                                fi.Add(f);
                        }
                        catch
                        {
                            f.Delete();
                            Console.WriteLine("del");
                            Console.ReadKey();
                        }
                      
                    }
                }
                else
                {
                    foreach (FileInfo f in fil)
                    {
                        ind = name.LastIndexOf('.');
                        if (ind == -1)
                        {
                            ind = f.Name.LastIndexOf('.');
                            if (ind != -1)
                                temp = f.Name.Remove(ind);
                            else
                                temp = f.Name;

                            if (temp == name)
                                fi.Add(f);
                        }
                        else
                        {
                            if (f.Name == name)
                                fi.Add(f);
                        }
                    }
                }
            }
           
          

            foreach (DirectoryInfo d in dir)
            {
                if (l[2] == "d" && d.Name == name)
                    di.Add(d);
          
                FileAttributes attributes;

                try
                {
                   
                    attributes = File.GetAttributes(d.FullName);
                }
                catch
                {
                    continue;
                }
               
                if((attributes & FileAttributes.Hidden) != FileAttributes.Hidden &&
                    (attributes & FileAttributes.System) != FileAttributes.System &&
                    (attributes & FileAttributes.Temporary) != FileAttributes.Temporary &&
                    (attributes & FileAttributes.Compressed) != FileAttributes.Compressed &&
                    //(attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly &&
                    d.Name != "Windows")
                {
                    Find(l, d, di, fi, c);
                }
            }
            return;
        }


        internal bool Rename(List<string> l)
        {
            FileInfo sours_file = null;
            FileInfo dest_file = null;
            DirectoryInfo sours_dir = null;
            DirectoryInfo dest_dir = null;

            if (l.Count != 4)
            {
                Mann(null, l[0]);
                    
                return false;
            }

            try
            {
                if (l[3] == "d")
                {
                    sours_dir = new DirectoryInfo(l[1]);
                    dest_dir = new DirectoryInfo(l[2]);

                    sours_dir.MoveTo(dest_dir.FullName);
                 }
                else
                if (l[3] == "f")
                {
                    sours_file = new FileInfo(l[1]);
                    dest_file = new FileInfo(l[2]);
                    sours_file.MoveTo(dest_file.FullName);
                }
                else
                    return false;

                Refresh();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("press any key");
                Console.ReadKey();
                return false;
            }
        }

        internal bool Read(List<string> l, out List<string> ext)
        {
            FileStream fs = null;
            StreamReader sr_fs = null;
            List<string> l_str = new List<string>();

            if (l.Count < 2 || !l[1].EndsWith(".txt"))
            {
                Mann(null, l[0]);
                ext = null;
                return false;
             }

            try
            {
                fs = new FileStream(l[1], FileMode.Open);
                sr_fs = new StreamReader(fs);

                while (!sr_fs.EndOfStream)
                {
                    l_str.Add(sr_fs.ReadLine());
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("press any key");
                Console.ReadKey();
                ext = null;
                return false;
            }

            if(fs != null)
            {
                //fs.Flush();
                sr_fs.Close();
            }
           
            ext = l_str;
            return true;
        }

        //internal bool comp(List<string> l)
        //{
        //    List<string> f1 = new List<string>();
        //    List<string> f2 = new List<string>();

        //    List<string> arg = new List<string>();
        //    if (l.Count != 3)
        //    {
        //        mann(null, l[0]);
                    
        //        return false;
        //    }

        //    arg.Add(" ");
        //    arg.Add(l[1]);

        //    f1 = read(arg);

        //    if (f1 == null)
        //    {
        //        Console.WriteLine("path 1 invalid");
        //        Console.ReadKey();
        //        return false;
        //    }
        //    arg.Clear();
        //    arg.Add(" ");
        //    arg.Add(l[2]);

        //    f2 = read(arg);

        //    if(f2 == null)
        //    {
        //        Console.WriteLine("path 2 invalid");
        //        Console.ReadKey();
        //        return false;
        //    }

        //    if (f1.Count == f2.Count)
        //    {
        //        for (int i = 0; i < f1.Count; ++i)
        //        {
        //            if (f1[i] != f2[i])
        //                return false;
        //        }
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        internal void Restart()
        {
            Process.Start("shutdown", "/r /t 20");
        }


        internal void Cd(List<string> l)
        {
            int ind = -1;
            DirectoryInfo dest_dir = null;
           
            if (l.Count != 2)
            {
                Mann(null, l[0]);
                    
                return;
            }

           try
            {
                ind = Convert.ToInt32(l[1], 10);
            }
            catch
            {
                ind = -1;
            }

            if(ind >= 1 && ind <= sub_dir_list.Count())
            {
              foreach(DirectoryInfo d in sub_dir_list)
                {
                    if (ind == 1)
                        dest_dir = d;
                    --ind;
                }
                try
                {
                    dest_dir.EnumerateDirectories();
                }
                catch
                {
                    return;
                }
                current_dir = dest_dir;
                Directory.SetCurrentDirectory(current_dir.FullName);
            }

            if (ind == -1)
            {
                if (l[1] == "..")
                {
                    current_dir = current_dir.Parent;
                    Directory.SetCurrentDirectory(current_dir.FullName);
                }

                if (l[1] == "root")
                {
                    current_dir = current_dir.Root;
                    Directory.SetCurrentDirectory(current_dir.FullName);
               }
            }
            Refresh();

        }

        internal void Cd0(List<DirectoryInfo> dir_l, List<FileInfo> file_l)
        {
            string temp;
            int index = -1;

            if (dir_l.Count == 0 && file_l.Count == 0)
                return;
            Console.WriteLine("enter index dir or file");
            temp = Console.ReadLine();

            try
            {
                index = Convert.ToInt32(temp);
                if (index < 1)
                    return;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }

            if(index > 0 && dir_l.Count > 0 && index <= dir_l.Count)
            {
                string root_name = current_dir.Root.FullName;
                current_dir = dir_l[index - 1];
                Directory.SetCurrentDirectory(current_dir.FullName);

                Refresh();

                for (int i = 0; i < alldrives.Count; ++i)
                {
                    if (alldrives[i].RootDirectory.FullName == current_dir.Root.FullName)
                        current_drive = i;
                }

                return;
            }

            index = index - dir_l.Count;

            if(file_l.Count > 0 && index <= file_l.Count)
            {
                current_dir = file_l[index - 1].Directory;
                Directory.SetCurrentDirectory(current_dir.FullName);

                Refresh();

                for (int i = 0; i < alldrives.Count; ++i)
                {
                    if (alldrives[i].RootDirectory.FullName == current_dir.Root.FullName)
                        current_drive = i;
                }

                return;
            }

        }


        //настройка консоли
        internal void Set_cons()
        {
            string temp;
            ConsoleKeyInfo inp;
            do
            {
                Console.Clear();
                string menu = " prev_bkg_col ----------- 1\n" +
                              " next_bkg_col ----------- 2\n" +
                              " prev_frg_col ----------- 3\n" +
                              " next_frg_col ----------- 4\n" +
                              " WindowHeight (rows) ---- h\n" +
                              " WindowWidth (columns) -- w\n" +
                              " exit ------------------- esc";
                Console.WriteLine(menu);
                Console.WriteLine("Background -- " + Console.BackgroundColor.ToString());
                Console.WriteLine("Foreground -- " + Console.ForegroundColor.ToString());
                inp = Console.ReadKey(true);
                switch (inp.Key)
                {
                   case ConsoleKey.D1:
                        Consol.Prev_bkg_col();
                        Console.WriteLine(Console.BackgroundColor.ToString());
                        break;
                    case ConsoleKey.D2:
                        Consol.Next_bkg_col();
                        Console.WriteLine(Console.BackgroundColor.ToString());
                        break;
                    case ConsoleKey.D3:
                        Consol.Prev_frg_col();
                        Console.WriteLine(Console.ForegroundColor.ToString());
                        break;
                    case ConsoleKey.D4:
                        Consol.Next_frg_col();
                        Console.WriteLine(Console.ForegroundColor.ToString());
                        break;
                    case ConsoleKey.H:
                        Console.WriteLine("rows? (min = 15, max = {0})  ", Console.LargestWindowHeight);
                        temp = Console.ReadLine();
                        Consol.Set_h(temp);
                        break;
                    case ConsoleKey.W:
                        Console.WriteLine("columns? (min = 30, max = {0})  ", Console.LargestWindowWidth);
                        temp = Console.ReadLine();
                        Consol.Set_w(temp);
                        break;
                   }
            } while (inp.Key != ConsoleKey.Escape);
        }


        //справка
        internal void Mann(List<string> l, string s)
        {
            Console.Clear();
            List<string> outt = new List<string>();
            string comm = null;
           
            if(l != null)
            {
                if (l.Count == 1)
                    comm = "all";
                if (l.Count == 2)
                    comm = l[1];
            }
            else
            {
                if (s != null)
                    comm = s;
            }


            if(comm == "all")
            {
                foreach (string st in Man.l_man)
                    outt.Add(st);
            }
            else
            {
                foreach (string st in Man.l_man)
                    if (comm == st.Split(' ')[1])
                        outt.Add(st);
            }
            foreach (var st in outt)
            {
                Console.WriteLine(st);
            }
            Console.WriteLine("press any key");
            Console.ReadKey();
        }



        internal bool Del(List<string> l)
        {
            if (l.Count != 3)
            {
                Mann(null, l[0]);
                    
                return false;
            }

            FileInfo del_file = null;
            DirectoryInfo del_dir = null;
            try
            {
                if (l[2] == "f")
                {
                    del_file = new FileInfo(l[1]);
                    del_file.Delete();
                 }
                else
                if (l[2] == "d")
                {
                    del_dir = new DirectoryInfo(l[1]);
                    del_dir.Delete(true);
                }
                else
                    return false;

                Refresh();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("press any key");
                Console.ReadKey();
                return false;
            }
        }
    }
}
