using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace f_manager
{
     public class Read_command
    {
        static public List<string> Read(string path)
        {
            List<string> command = new List<string>();
            string[] comm;

            Console.Write(path + " ->");
                        
            string temp = Console.ReadLine();
            comm = temp.Trim().Split('-', ' ');

            foreach (string str in comm)
                if (str.Length != 0)
                    command.Add(str);

            return command;
        }

    }
}
