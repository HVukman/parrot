using ForthError;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HexDump;
using System.Security.Cryptography.X509Certificates;
using Spectre.Console;
using LanguageExt.Common;
using System.Linq.Expressions;
using System.Numerics;
using Microsoft.VisualBasic;
using LanguageExt.ClassInstances.Pred;
using System.Collections;
using Spectre.Console.Cli;
//using static LanguageExt.Compositions<A>;
using Parrot;
using LanguageExt;
using LanguageExt.ClassInstances;
using System.Data;


namespace Do_forth {
    public class DoForth

    {

        public static void Printstack(List<string> myList)
        {
            var rule = new Spectre.Console.Rule("[gold1]Stack[/]");
            rule.Justification = Justify.Left;
            AnsiConsole.Write(rule);

            if (myList.Count == 0 ) {
                Console.WriteLine("Empty!");
                //var image = new CanvasImage("C:/Users/vukma/OneDrive/parrot/parrot/cat.jpg");

                // Set the max width of the image.
                // If no max width is set, the image will take
                // up as much space as there is available.
                //image.MaxWidth(16);

                // Render the image to the console
                //AnsiConsole.Write(image);
            }
            else {
                foreach (string a in myList)
                {
                    Console.Write(a.ToString() + " ");
                }
            }
            
            Console.WriteLine("\n");
        }

        public static void Dump(List<string> myList)


        {
            foreach (string a in myList)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(a);
                //System.Diagnostics.Debug.WriteLine(Utils.HexDump(bytes));
                Console.WriteLine(Utils.HexDump(bytes));
            }

        }

        public static void Writedump(List<string> myList)
        {
            using (StreamWriter outputFile = new StreamWriter("HexDump.txt"))
            {
                foreach (string line in myList)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(line);
                    string result = Utils.HexDump(bytes);
                    outputFile.WriteLine(result);
                }

            }

        }

        public static void Loaddump(List<string> myList)
        {
            try
            {
                myList.Clear();
                string bitString;
                // Open the text file using a stream reader.
                using (var sr = new StreamReader("HexDump.txt"))
                {
                    while (sr.Peek() >= 0)
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes(sr.ReadLine());
                        bitString = BitConverter.ToString(bytes);
                        myList.Add(bitString);
                        // Console.WriteLine(sr.ReadLine());
                    }
                    // Read the stream as a string, and write the string to the console.

                }
            }
            catch (IOException e)
            {
                // Console.WriteLine("The file could not be read:");
                // Console.WriteLine(e.Message);
                throw new DoForthErrorException("FIle could not be read!");
            }
        }

        public static void Today(List<string> MyList)
        {
            Console.WriteLine(DateTime.Today.ToString("dd/MM/yyyy"));
            MyList.Add(DateTime.Today.ToString("dd/MM/yyyy"));
        }
        
        public static void Rev(List<string> myList)
        // x -- 
        {
            myList.Reverse();
        }

        public static void Pop(List<string> myList)
        // x -- 
        {
            myList.RemoveAt(myList.Count - 1);
        }

        public static void Swap(List<string> myList)
        // x y -- y x
        {
            var y = myList.Last();
            myList.RemoveAt(myList.Count - 1);
            var x = myList.Last();
            myList.RemoveAt(myList.Count - 1);
            myList.Add(y);
            myList.Add(x);

        }

        public static void Dup(List<string> myList)
        // x -- x x
        {
            var last = myList.Last();
            myList.Add(last);

        }

        public static void Rot(List<string> myList)
        /// x y z --- y z x
        {
            var z = myList.Last();
            myList.RemoveAt(myList.Count - 1);
            var y = myList.Last();
            myList.RemoveAt(myList.Count - 1);
            var x = myList.Last();
            myList.RemoveAt(myList.Count - 1);


            myList.Add(y);
            myList.Add(z);
            myList.Add(x);
        }

        public static void Clear(List<string> myList)
        {
            // x y z -- 
            myList.Clear();
        }

        public static void Over(List<string> myList)
        // x y -- x y x
        {
            var y = myList.Last();
            myList.RemoveAt(myList.Count - 1);
            var x = myList.Last();
            myList.RemoveAt(myList.Count - 1);
            myList.Add(x);
            myList.Add(y);
            myList.Add(x);
        }


        public static void Nip(List<string> myList)
        // x y -- y
        {
            var y = myList.Last();
            myList.RemoveAt(myList.Count - 1);
            myList.RemoveAt(myList.Count - 1);
            myList.Add(y);
        }

        public static void Peek(List<string> myList)
        // x -- x
        {
            var y = myList.Last();
            Console.WriteLine("the first element is: " + y.ToString());
        }

        public static void Print(List<string> myList)
        // x -- x
        {
            var y = myList.Last();
            myList.RemoveAt(myList.Length() - 1);
            Console.WriteLine("the first element was: " + y.ToString());
        }

        public static bool Sum(List<string> myList, bool violate)
        // ... -- y
        {
            try {
                List<BigInteger> bigints = myList
                    .Select(s => { BigInteger i; return BigInteger.TryParse(s, out i) ? i : (BigInteger?)null; })
                    .Where(i => i.HasValue)
                    .Select(i => i.Value)
                    .ToList();

                var y = bigints.Aggregate(BigInteger.Add);
                myList.Clear();
                myList.Add(y.ToString());
                violate = false;
            }
            catch (InvalidOperationException) 
            {
                Console.WriteLine("Only ints in sum!");
                violate= true;
            }
        return violate;
            
        }

        public static bool Fetch(List<string> myList, Dictionary<string, string> CustomVars)
        {
            bool violate;
            string key = myList.Last().ToString();
            myList.RemoveAt(myList.Length() - 1);
            string value;
            if (CustomVars.TryGetValue(key, out value))
            {
                Console.WriteLine("Fetched value: {0}", value);
                myList.Add(value);
                violate = false;
            }
            else
            {
                Console.WriteLine("No such key: {0}", key);
                violate=true;
            }
            return violate;
        }

        public static bool Store(List<string> myList, Dictionary<string,string> CustomVars)
        {
            bool violate;

            try {
                string name = myList.Last();
                myList.RemoveAt(myList.Count - 1);
                var value = myList.Last().ToString();
                CustomVars.Add(name, value);
                myList.RemoveAt(myList.Count - 1);
                Console.WriteLine("stored " + name + " with value " + value);
                violate = false;
                }
            catch (ArgumentException)
            {
                Console.WriteLine("argument exception while defining!");
                violate = true;
            }
            return violate;

        }




        public static (bigint,bool) Allot(List<string> myList, bigint allot)
        {
            bool violate=false;
            try
            {
                BigInteger size = BigInteger.Parse(myList.Last());
                allot = size;
                Console.WriteLine($"{allot.ToString()} spaces reserved for word!" );
            }
            catch (FormatException)
            {
                Console.WriteLine("an int must be allocated!");
                allot = 0;
                violate = true;
            }

            return (allot, violate);
        }

        public static bool Bool_checks(string command, List<string> myList, List<bool> flow_check, Parrot.Parrot.OP_CODES mode , bool violate)
        {
            // command is given as argument
            bool check = false;
            // x y -- y

            var y = myList.Last();
            var x1 = myList[myList.Count - 2];


            var end = BigInteger.TryParse( y, out BigInteger result);
            var second_to_last = BigInteger.TryParse(y, out BigInteger result2);


            if ( end==true && second_to_last && true )
            {
                
                            switch (command)
                            {
                                case "=":
                                    check = (result == result2);
                                    violate = false;
                                    break;
                                case "!=":
                                    check = (result != result2);
                                    violate = false;
                                    break;
                                case ">":
                                    check = (result > result2);
                                    violate = false;
                                    break;
                                case "<":
                                    check = (result < result2);
                                    violate = false;
                                    break;
                                default:
                                    violate = true;
                                    break;
                            }


                        
            }

            else if ( end==false || second_to_last ==false ) 
                
            {
                    
                        switch (command)
                        {
                            case "=":
                                check = string.Equals(y,x1);
                                violate = false;
                                break;
                            case "!=":
                                check = (string.Equals(y, x1)==false);
                                violate = false;
                                break;
                            case ">":
                                check = (y.Length() < x1.Length());
                                violate = false;
                                break;
                            case "<":
                                check = (y.Length() > x1.Length());
                                violate = false;
                                break;
                            default:
                                violate = true;
                                break;
                        }

                   

            }



            switch (mode)
            {
                case  Parrot.Parrot.OP_CODES.Interpret:
                    myList.RemoveAt(myList.Count - 1);
                    myList.RemoveAt(myList.Count - 1);
                    myList.Add(check.ToString());
                    violate = false;
                    break;

                case Parrot.Parrot.OP_CODES.IF_Mode:
                    flow_check.Add(check);
                    violate = false;
                    break;
            }

            return violate;

        }


        public static (object,bool) Operation(List<string> myList, string command)
        {
            var success_add = false;
            object result = 0;

            try
            {
                var var1 = myList[myList.Count - 1];
                var var2 = myList[myList.Count - 2];

                BigInteger j;
                BigInteger l;
                bool i= BigInteger.TryParse(var1, out j);
                bool k= BigInteger.TryParse(var2, out l);

                //Type type_of_vars = var1.GetType();

                if ((i && k) ==true)        
                {
                    switch (command)
                    {
                        case "+":

                            result = BigInteger.Parse(var1) + BigInteger.Parse(var2);
                            success_add = true;
                            break;

                        case "-":

                            result = BigInteger.Parse(var1) - BigInteger.Parse(var2);
                            success_add = true;
                            break;

                        case "*":

                            result = BigInteger.Parse(var1) * BigInteger.Parse(var2);
                            success_add = true;
                            break;

                        case "/":

                            if (Int32.Parse(var2) != 0)
                            {
                                result = BigInteger.Parse(var1) / BigInteger.Parse(var2);
                                success_add = true;
                            }
                            else { success_add = false; }

                            break;

                        case "%":

                            if (Int32.Parse(var2) != 0)
                            {
                                result = BigInteger.Parse(var1) % BigInteger.Parse(var2);
                                success_add = true;
                            }
                            else { success_add = false; }
                            break;
                        default:
                            success_add = false;
                            result = 0;
                            break;
                    }
                }

                else
                {
                    switch (command)
                    {
                        case "+":

                            result = var1 + var2;
                            success_add = true;
                            break;

                        
                        default:
                            Console.WriteLine("Only + for string");
                            success_add = false;
                            result = 0;
                            break;
                    }
                }

   
            }
            catch (FormatException)
            {
                Console.WriteLine("No numbers added!");
                success_add = false;

            }

            return (result, success_add);
           
        }


            public static (List<string>, List<bool>, bool, Dictionary<string,string>, Dictionary<string, List<string>> ,bigint, Parrot.Parrot.OP_CODES) doSth(List<string> myList, List<bool> control_buffer_stack, 
                string command, Parrot.Parrot.OP_CODES mode, bool loop_flag, List<int> loop_control_stack, Dictionary<string,string> CustomVars,
                 Dictionary<string, List<string>> CustomWord, bigint allot)

            {
                int number;


                string[] mult_add_commands = Consts_Variables.mult_add_commands;
                string[] standard_stackops = Consts_Variables.standard_stackops;
                string[] extrawords = Consts_Variables.extrawords;
                string[] comparisons = Consts_Variables.comparisons;
                string[] extraops = Consts_Variables.ext_stackops;
                string[] definitions = Consts_Variables.definitions;
                bool violate= false;

                dynamic[] return_op = [0, false];
                object result = 0;
                bool success_add = false;

                
            // Perform mult add commands on stack

            if (mult_add_commands.Contains(command))
            {

                if (myList.Count > 1)
                {
                    (result, success_add) = Operation(myList, command);
                }
                else
                {
                    Console.WriteLine("Stack underflow!");
                    violate = true;
                }

            }

            // define words and vars

            

            else if (definitions.Contains(command))
            {

                if (myList.Count > 0)
                {   
                    switch (command)
                    {
                        /// store variable
                        case "store":
                            if (myList.Count > 1)
                            {
                                violate = Store(myList, CustomVars);
                            }
                            else 
                            {
                                Console.WriteLine("stack is too small!");
                                violate = true;
                            }
                                
                            break;
                        // define word in array
                        default:
                           
                            break;
                    }
                    
                }

                else
                {
                    Console.WriteLine("Stack too small for definitions");
                    violate = true;
                }
            }

            // Perform mult add commands on stack
            // Perform standard words on stack


            else if (standard_stackops.Contains(command))

            {
                if (myList.Count > 0)
                {
                    switch (command)

                    {
                        case "drop":
                            // drop last element
                            Pop(myList);
                            break;

                        case "fetch":
                            // fetch stored variable
                            violate=Fetch(myList,CustomVars);
                            break;

                        case "swap":
                            // swap top stack
                            if (myList.Count > 1)
                            {
                                Swap(myList);
                            }
                            else
                            {
                                Console.WriteLine("Stack underflow for swap! must be bigger than 1!");
                                violate = true;

                            }
                            break;

                        case "dup":
                            // double element on top

                            Dup(myList);
                            break;

                        case "rot":
                            // rotate first three elements

                            if (myList.Count > 2)
                            {
                                Rot(myList);
                            }
                            else

                            {
                                Console.WriteLine("Stack underflow! Must be bigger than 2!");
                                violate = true;
                            }
                            break;

                        case "nip":
                            // deletes second to last element
                            if (myList.Count > 1)
                            {
                                Nip(myList);
                            }
                            else
                            {
                                violate = true;
                                Console.WriteLine("Stack underflow for nip! must be bigger than 1!");

                            }
                            break;


                        case "over":
                            // doubles second to last element
                            if (myList.Count > 1)
                            {
                                Over(myList);
                            }
                            else
                            {

                                violate = true;
                                Console.WriteLine("Stack underflow for over! must be bigger than 1!");

                            }
                            break;


                        case "clear":
                            // clears stack
                            Clear(myList);
                            break;

                        case "rev":
                            // reverses stack
                            Rev(myList);
                            break;

                        case "dump":
                            // hexdump
                            Dump(myList);
                            break;

                        case "peek":
                            // nondestructive peek
                            Peek(myList);
                            break;

                        case "print":
                            // destructive print
                            Print(myList);
                            break;

                        case "allot":
                            // future: allot vector
                            (allot, violate)=Allot(myList,allot); 
                            break;

                        default:
                            Console.WriteLine("sorry not there yet!");
                            break;
                            
                    }
                }
                else

                {
                    Console.WriteLine("Stack underflow! Must be bigger than 0 for command " + command + "!");
                }

            }


            // Perform booleans on stack

            else if (comparisons.Contains(command))
            {
                //  Printstack(myList);
                try
                {
                    if (myList.Count > 1)
                    {
                        violate = Bool_checks(command, myList, control_buffer_stack, mode, violate);
                    }
                    else
                    {
                        violate = true;
                        throw new DoForthErrorException("Stack too small for comparisons!");

                    }
                }
                catch (DoForthErrorException)
                {
                    Printstack(myList);
                }
            }


            // perform special ops ons stack
            else if (extraops.Contains(command))
            {

                switch (command)
                {

                    case "sum":
                        violate= Sum(myList, violate);
                        break;
                    default:
                        break;
                }


            }
            // Perform extra words on stack

            else if (extrawords.Contains(command))
            {
                Console.WriteLine("extra");
                switch (command)
                {
                    case "writedump":
                        Writedump(myList);
                        break;
                    case "loaddump":
                        //Loaddump(myList);
                        break;
                    case "today":
                        Today(myList);
                        break;
                    default : break;
                }

            }

            // Print Stack

            else if (command == "echo")

            {
                Printstack(myList);
            }

            // Dump on screen

            else if (command == "dump")

            {
                Dump(myList);
            }

            else if (command == "today")
            {
                Today(myList);
            }

            else if (command == "inc" && loop_flag == true)

            {
                int inc = loop_control_stack[loop_control_stack.Length() - 2];
               // Console.WriteLine(inc);
                myList.Add(inc.ToString());
            }



            // Unknown words
            else
            {

                success_add = false;
                violate = true;
                Console.WriteLine("Unknown command: " + command);
                Printstack(myList);
                // throw new DoForthErrorException("Unknown command " + command); 

            }

                if (success_add == true)
                {
                    // if compile adds to stack
                    // else puts boolean on stack
                    switch (mode)
                    {
                        case Parrot.Parrot.OP_CODES.Interpret:
                            Pop(myList);
                            Pop(myList);

                            // Console.WriteLine(result);


                            myList.Add(result.ToString());
                            break;
                        case Parrot.Parrot.OP_CODES.IF_Mode:
                            myList.Add(result.ToString());
                            break;

                    }

                }
                return (myList, control_buffer_stack, violate, CustomVars, CustomWord, allot, mode);

            }

                public  (List<string> ,List<bool>, bool, Dictionary<string,string>, Dictionary<string, List<string>>, bigint, Parrot.Parrot.OP_CODES)  Main(List<string> myList, List<bool> control_buffer_stack, string command, Parrot.Parrot.OP_CODES mode, 
                    bool loop_flag, List<int> loop_control_stack, 
                    Dictionary<string, string> CustomVars, Dictionary<string, List<string>> CustomWord, bool violate, bigint allot)
               
                {
                            string pattern = @"^""[\w\s!:\(\)]+""$";
                            
                            int number;
                            // Add integer to stack
                            if (int.TryParse(command, out number) == true)
                            {
                                myList.Add(command);

                            }

                            // Add string to stack

                            else if (Regex.IsMatch(command, pattern))
                            {
                                myList.Add(command);

                            }


                            else
                            {
                                (myList, control_buffer_stack, violate, CustomVars, CustomWord, allot, mode) = doSth(myList, control_buffer_stack, command, mode, loop_flag, loop_control_stack, CustomVars, CustomWord, allot);
                            }

                            return (myList, control_buffer_stack, violate, CustomVars, CustomWord, allot, mode);

                }
        }


    }


