using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace f_manager
{
    static class Man
    {
        static internal List<string> l_man;
        static internal string create;
        static internal string rename;
        static internal string move;
        static internal string copy;
        static internal string find;
        static internal string read;
        static internal string compare;
        static internal string cd;
        static internal string restart;
        static internal string exit;
        static internal string cdd;
        static internal string set;
        static internal string del;
        static internal string F12;
        static internal string arrow_keys;

        static Man()
        {
            l_man = new List<string>();
            create = " create name -d\n" +
                                 "       - (or space)    -> separator\n" +
                                 "       name            -> new name (relative or absolute)\n" +
                                 "       d(or f)         -> create dir (or file)\n\n";

            rename = " rename old_name new_name -d\n" +
                                 "       - (or space)    -> separator" +
                                 "       old_name        -> old name (relative or absolute)\n" +
                                 "       new_name        -> new name (relative or absolute)\n" +
                                 "       d(or f)         -> rename dir (or file)\n\n";

            move = " move sourse_name dest_name -d\n" +
                                 "       - (or space)    -> separator\n" +
                                 "       source_name     -> source name (relative or absolute)\n" +
                                 "       dest_name       -> destination name (relative or absolute)\n" +
                                 "       d(or f)         -> move dir(or file)\n\n";

            copy = " copy sourse_name dest_name -d\n" +
                                 "       - (or space)    -> separator\n" +
                                 "       source_name     -> source name(relative or absolute)\n" +
                                 "       dest_name       -> destination name of dir (relative or absolute)\n" +
                                 "       d(or f)         -> copy dir(or file)\n\n";

            find = " find name -d" +
                                 "       - (or space)    -> separator\n" +
                                 "       name            -> name(with or without extension for file or *.extension)\n" +
                                 "       d(or f)         -> find dir(or file)\n\n";

            read = " read name.txt" +
                                 "      name             ->name file(relative or absolute)\n\n";

            compare = " compare file1_name file2_name\n" +
                                 "       - (or space)    -> separator\n" +
                                 "       file1_name      -> name (relative or absolute)\n" +
                                 "       file2_name      -> name (relative or absolute)\n\n";

            cd = " cd identifier\n" +
                                 "       identifier: Int32 -> index of dir from current dir\n" +
                                 "       identifier: root  -> go to root of current drive\n" +
                                 "       identifier: ..    -> go to parent dir\n\n";

            del = " del name -d(-f)\n" +
                                 "       name              -> name file or dir\n" +
                                 "       d(or f)           -> del dir(or file)\n\n";

            restart = " restart                            -> restart PC";
            exit = " exit                                  -> clouse app.";
            cdd = " cdd                                    -> next drive";
            set = " set                                    -> setting console";
            F12 = " F12                                    -> command line ";
            arrow_keys = " arrow                           -> uparrow, down arrow";

            l_man.Add(create);
            l_man.Add(rename);
            l_man.Add(move);
            l_man.Add(copy);
            l_man.Add(find);
            l_man.Add(read);
            l_man.Add(compare);
            l_man.Add(cd);
            l_man.Add(restart);
            l_man.Add(exit);
            l_man.Add(cdd);
            l_man.Add(set);
            l_man.Add(del);
            l_man.Add(F12);
            l_man.Add(arrow_keys);

        }
    }
}
