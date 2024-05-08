// See https://aka.ms/new-console-template for more information



using System.Text.RegularExpressions;
using ForthError;
using Spectre.Console;
using Do_forth;
using System.Numerics;
using System.Diagnostics;
public class Parrot
{

    [Flags]
    enum Boolean_Flag : int
    {
        True = 0, False = 1
    }


    [Flags]
    public enum OP_CODES
    {
        Interpret, // perform action
        Compile_Word,  // compile word 
        Compile_Func, // Compile FUunction
        COMMENT, // Comment
        IF_Mode,  // switches to if mode
        IF_THEN_Mode // performs if then check
    }


    public static class IF_THEN_STRING 
    
    { 


        public const string IF = "if"; 
    
    }


    [Flags]
    public enum IF_THEN: int
    {

        IF,
        IF_ELSE,
        THEN,
        ELSE,
        ELSEIF,
        ELSEIFIF,
        WHILE,
        BREAK
    }

    static public void Main()
    {
        
        
        static void Reset_stack(List<string> stack_fail, List<string> stack_old)
        {
            // reset stack
            
            stack_fail=stack_old.ToList();

        }

        ConsoleKeyInfo cki;
        //cki = Console.ReadKey(true);
        // Prevent example from ending if CTL+C is pressed.
        //Console.TreatControlCAsInput = true;

        string if_string= IF_THEN_STRING.IF;

        var run = true;
        string[] standardwords = Consts_Variables.standardwords;
        string[] extrawords = Consts_Variables.extrawords;
        string[] ext_stackops= Consts_Variables.ext_stackops;
        string[] booleans = Consts_Variables.booleans;
        string[] mult_add_wors = Consts_Variables.mult_add_commands;
        string[] var = Consts_Variables.var;
        string[ ] func = Consts_Variables.func;
        string increment = "inc";

        char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
        List<string> oldstack = [];
        //string userinput;


        // ENUMS and FLAGS
        OP_CODES modes = OP_CODES.Interpret;
        IF_THEN iF_THEN_ELSE;

       
        bool boolean_control_flow = false;
        // bool if_then_check = false;
        // OP_CODES mode = new OP_CODES();

        List<string> oldinputs= new List<string>();
        List<string> stack = new List<string>();
        List<List<string>> cyclestack = new List<List<string>>();

        List<bool> control_flow_stack = new List<bool>();
        List<string> control_buffer_stack = new List<string>();

        List<string>  pre_processed_words = new List<string>();
        List <string> words = new List<string>();
        string[] commands;
        List<string> command = new List<string>();      
        // for do .. loops
        // put 10 1 on stack
        // if != then +1
        // if = terminate and go on

        List<int> loop_control_stack = new List<int>();
        bool do_loop_flag=false;
        bool while_flag=false;

        //Console.BackgroundColor = ConsoleColor.DarkBlue;
        //Console.ForegroundColor = ConsoleColor.Black;


        var panel = new Panel("Hello There!");
        AnsiConsole.Write(
        new FigletText("This is Parrot")
        .LeftJustified()
        .Color(Color.Gold1));


        // Custom Words Dictionary
        Dictionary<string, string> CustomWords =
            new Dictionary<string, string>();

        // Custom Variables Dictionary
        Dictionary<string, string> CustomVars =
            new Dictionary<string, string>();


        // byte[] bytes = Encoding.ASCII.GetBytes("Hello There");
        //System.Diagnostics.Debug.WriteLine(Utils.HexDump(bytes));
        //Console.WriteLine(Utils.HexDump(bytes));
        while (run)
        {

            // Type instruction in REPL
            string userinput = AnsiConsole.Ask<string>("What's your [gold1]input[/]?");
            oldinputs.Add(userinput);
            userinput = userinput.Trim();
           // Console.WriteLine("Enter Instruction ->");

            // Create a string variable and get user input from the keyboard and store it in the variable

            
            // var userinput = Console.ReadLine().Trim();
           

            commands = userinput.Split(delimiterChars);
            pre_processed_words = commands.ToList();
            
            // words = commands.ToList();
            //string pattern = @"^""[\w\s!:\(\)]+""$";

            string semicolon_pattern =@"(?<=\w)(?=;)";


            // Separate seimicolon from word
            
            foreach (string word in pre_processed_words) {
                Match m = Regex.Match(word, semicolon_pattern, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    string[] semicolon_strings = [word.Substring(0, word.Length - 1), word.Substring(word.Length - 1,1)];
                    foreach (var semicolon_string in semicolon_strings)
                    {
                        
                        words.Add(semicolon_string.Trim().ToLower());
                        // Console.WriteLine(semicolon_string);
                    }
                }
                else if (word=="")
                {
                    continue;
                }
                
                else
                {
                    words.Add (word.Trim().ToLower());
                }
                
            }

            int input_length= words.Count();
            
            Console.WriteLine("input length: " + input_length.ToString());

            /*
            Stopwatch sw;
            sw = Stopwatch.StartNew();
            */


            if (userinput != null)
            {
                oldstack = stack.ToList();
                //Tokenizer_check checks = new Tokenizer_check();
                //words = commands.ToList();

                string pattern = @"^""[\w\s!:\(\)]+""$";

                var violate = false;
                // register = 0;

                for (int register=0; register<input_length; register++)
                {
                        
                        string word = words[register];

                        // Console.WriteLine ("word: " + word);

                       
                    // Loop for interpreting


                        if (var.Contains(word))
                        {

                            // Enter compile word mode
                            modes = OP_CODES.Compile_Word;
                            register++;

                        }

                        else 

                        {

                        // if word 0 and 1 == /
                        // then comment mode
                        if (word[0] == '/' && word[1]=='/' && modes!=OP_CODES.COMMENT)
                        {
                            modes = OP_CODES.COMMENT;
                        }

                        else if (modes==OP_CODES.COMMENT && word != "\n")
                        {
                            // Do nothing
                            // Comment mode
                        }


                        else if (modes == OP_CODES.Compile_Word)
                        // Compile Word and add to dict 
                        {
                            if (stack.Length() < 2)
                            {
                                Console.WriteLine("Stack too small!");
                                violate = true;
                            }

                            else

                            {

                                try { }
                                catch (Exception e) { Console.WriteLine(e.ToString()); }

                            }

                        }

                        else if (((word == if_string) || mult_add_wors.Contains(word) || standardwords.Contains(word) || extrawords.Contains(word) || ext_stackops.Contains(word) || (BigInteger.TryParse(word, out BigInteger number)) || Regex.IsMatch(word, pattern))
                            && (modes == OP_CODES.Interpret) 
                            && (violate == false))
                        {
                            // Exit program!

                            if (userinput == "bye")
                            {
                                Console.WriteLine(":( Bye!");
                                run = false;
                                break;
                            }


                            else if ((mult_add_wors.Contains(word) || ext_stackops.Contains(word) || Regex.IsMatch(word, pattern) || (BigInteger.TryParse(word, out BigInteger number2)) || standardwords.Contains(word))
                                    )
                            {
                                // standard do words
                                //Console.WriteLine((int.TryParse(word, out int number3)));
                                if (while_flag == false)
                                {
                                    (stack, control_flow_stack, violate, CustomWords) = DoForth.doSth(stack, control_flow_stack, word.ToLower(), 
                                        modes, do_loop_flag, loop_control_stack, CustomWords);
                                }
                                if (while_flag == true) 
                                {
                                    (stack, control_flow_stack, violate, CustomWords) = DoForth.doSth(control_buffer_stack, control_flow_stack, word.ToLower(), 
                                        modes, do_loop_flag, loop_control_stack, CustomWords);
                                }
                                // PRINT STATEMENT
                                // Console.WriteLine("register: " + register.ToString());
                            }


                            else if (word == if_string)
                            {
                                // when enter if mode with "If", switch stack with control buffer

                                control_buffer_stack = stack.ToList();
                                // enter if_mode
                                // Leave this loop
                                modes = OP_CODES.IF_Mode;

                                // PRINT STATEMENT
                                //Console.WriteLine("Switch stack");
                                // Console.WriteLine(modes.ToString());

                            }

                        }


                        else if (modes == OP_CODES.IF_Mode && word != "then")
                        {

                            // performs action
                            if (do_loop_flag == true && word == increment && loop_control_stack.Length() > 1)
                            // Use loop index while in loop
                            // Length check for own safety

                            {
                                int loop_i = loop_control_stack[loop_control_stack.Length() - 2];
                                control_buffer_stack.Add(loop_i.ToString());
            //                    Console.Write("index i: ", loop_i);
                            }

                            else 
                            {
                                //              Console.Write(word);
                                (stack, control_flow_stack, violate, CustomWords) = DoForth.doSth(control_buffer_stack, control_flow_stack,
                                    word.ToLower(), OP_CODES.IF_Mode, do_loop_flag, loop_control_stack, CustomWords);
                            }
                            
                            // DoForth.printstack(stack);
                            // 

                            // PRINT STATEMENT
                            //Console.WriteLine("register: " + register.ToString());
                        }


                        else if (modes == OP_CODES.IF_Mode && word == "then")
                        {
                            // when in if mode and then
                            // switch modes to if_then_mode
                            //modes = OP_CODES.IF_THEN_Mode;

                            // PRINT STATEMENT
                            //  Console.WriteLine("switched to if then");


                            try
                            {
                                // access top of control buffer
                                boolean_control_flow = control_flow_stack[control_flow_stack.Count - 1];

                                // PRINT STATEMENT

                                // Console.WriteLine("boolean control flow: " + boolean_control_flow.ToString());

                                modes = OP_CODES.IF_THEN_Mode;
                            }

                            catch (ArgumentOutOfRangeException)
                            {
                                // DoForth.Printstack(stack);

                                violate = true;

                                // PRINT STATEMENT
                                Console.WriteLine("There is nothing on the return stack!");

                                //Console.WriteLine(booleans.Contains(word).ToString());
                                //Console.WriteLine(modes.ToString());
                                //throw new DoForthErrorException("there is nothing on the control stack!");

                            }

                        }

                        else if (modes == OP_CODES.IF_THEN_Mode)
                        {
      

                                switch (boolean_control_flow)
                                {
                                    // when flow true: do forth until else
                                    case true:

                                        if (boolean_control_flow == true && (word != "else"))
                                        {
                                            if (word == ";")
                                            {
                                                modes = OP_CODES.Interpret;
                                            }
                                            
                                            else if (do_loop_flag == true && word == increment && loop_control_stack.Length() > 1)
                                            // Use loop index while in loop
                                            // Length check for own safety⌈

                                            {
                                                int loop_i = loop_control_stack[loop_control_stack.Length() - 2];
                                                stack.Add(loop_i.ToString());
          //                                      Console.Write("index i: ", loop_i);
                                            }

                                            else if (word != increment && do_loop_flag == true && loop_control_stack.Length() > 1)
                                            {
                                            (stack, control_flow_stack, violate, CustomWords) = DoForth.doSth(stack, control_flow_stack, word.ToLower(), OP_CODES.Interpret,
                                                    do_loop_flag, loop_control_stack, CustomWords);
                                            }


                                            else if (boolean_control_flow == true && word == "else")
                                            {
                                                boolean_control_flow = false;

                                            }
                                            else if (boolean_control_flow == false && (word == ";" || word == "\n"))
                                            {

                                                modes = OP_CODES.Interpret;
                                            }
                                            else
                                            {

                                                continue;
                                            }

                                    }
                                    break;


                                    case false:

                           //             Console.WriteLine(word);
                                        if (boolean_control_flow == false && word != "else")
                                        {

                                            continue;

                                        }

                                        else if (boolean_control_flow == false && word == "else")
                                        {

                                            boolean_control_flow = true;

                                        }
                                        else if (boolean_control_flow == true && (word == ";" || word == "\n"))
                                        {

                                            modes = OP_CODES.Interpret;
                                        }

                                        else if (do_loop_flag == true && word == increment && loop_control_stack.Length() > 1)
                                        // Use loop index while in loop
                                        // Length check for own safety

                                        {
                                            int loop_i = loop_control_stack[loop_control_stack.Length() - 2];
                                            stack.Add(loop_i.ToString());
                                            //Console.Write("index i: ", loop_i);
                                        }

                                        else if (boolean_control_flow == true && (word != ";" && word != "\n" && word!=increment))

                                        {
                                            // PRINT STATEMENT
                                            // Console.WriteLine(word);

                                            (stack, control_flow_stack, violate, CustomWords) = DoForth.doSth(stack, control_flow_stack,
                                                word.ToLower(), OP_CODES.Interpret, do_loop_flag, loop_control_stack, CustomWords);
                                            
                                        }
                                    break;

                            }

                        }

                        else if (word == "do" && modes!=OP_CODES.COMMENT)
                        {
                            
                            try
                            {
                                // check forth, parse and access exceptions!
                                if ((!int.TryParse(stack[stack.Length() - 1], out int age) && (!int.TryParse(stack[stack.Length() - 2], out int end))))
                                {

                                    throw new FormatException("");
                                    // Whatever
                                }

                                else if (do_loop_flag == false)
                                {

                                    int loop_end;
                                    int loop_start;
                                    loop_start = int.Parse(stack[stack.Length() - 2]);
                                    loop_end = int.Parse(stack[stack.Length() - 1]);

                                    if (loop_end < loop_start)
                                    {
                                        violate = true;
                                        Reset_stack(stack, oldstack);
                                        throw new DoForthErrorException("End must be bigger than Beginning! Syntax: Beginning End do .. loop ");

                                    }

                                    else
                                    {
                                        loop_control_stack.Add(loop_start);
                                        loop_control_stack.Add(loop_end);
                                        //loop_control_stack[loop_control_stack.Length() - 2] = loop_start;
                                        //loop_control_stack[loop_control_stack.Length() - 1] = loop_end;
                                        stack.RemoveAt(stack.Length() - 1);
                                        stack.RemoveAt(stack.Length() - 1);
                                        do_loop_flag = true;

                                    }

                                }
                                else if (do_loop_flag == true)
                                {
                                   
                                }
                            }

                            catch (FormatException e)
                            {
                                Console.WriteLine("Ints must be parsed! Syntax is int_end int_begin do .... loop");
                                violate = true;
                            }

                            catch (ArgumentOutOfRangeException e)
                            {
                                Console.WriteLine("Stack is too small! Syntax is int_end int_begin do .... loop");
                                violate = true;
                            }

                            catch (DoForthErrorException e)
                            {
                                violate = true;
                            }
                        }

                        else if (word == "loop" && modes != OP_CODES.COMMENT)

                        // sees loop
                        {
                            //  Console.WriteLine("sees loop");
                            if (do_loop_flag == false)
                            // loop only in do.. loop
                            {

                                Console.WriteLine("Loop only in loops! Syntax: int_beginning int_end do ... loop; ");
                                violate = true;
                            }
                            else
                            // all good
                            {
                                // increment index until ==
                                loop_control_stack[loop_control_stack.Length() -2]++;
                                if (loop_control_stack[loop_control_stack.Length() - 1] == loop_control_stack[loop_control_stack.Length() - 2])

                                {
                                    // go on in instruction and loop is false!

                                    loop_control_stack.Clear();   
                                    do_loop_flag = false;
                                }

                                else {

                                    // if mode== if then -> then switch
                                    if (modes == OP_CODES.IF_THEN_Mode)
                                    {
                                        modes = OP_CODES.Interpret;

                                    }
                                    
                                        while (word != "do")
                                        // decrease register until do!

                                        {

                                            register--;
                                            // Console.WriteLine("register: ", register.ToString());
                                            // Console.WriteLine("word: ", word);
                                            word = words[register];

                                        }

                                }

                            }
                        }

                        else if (word=="begin-while")
                        
                        {
                            Console.WriteLine("begin while");

                        }

                        else if (word=="while")
                        {
                            if (while_flag == false)
                            {
                                while_flag = true;
                                control_buffer_stack = stack.ToList();
                                // Console.WriteLine("while only in while loop!");
                                // violate= true;
                            }
                            if (while_flag== true)
                            {
                                //Console.WriteLine("Only one while!");
                                // violate=true;
                                
                            }
                        }

                        else if (word == "end-while")
                        {
                            while_flag = false;
                            try
                            {

                                bool end_check = bool.Parse(control_buffer_stack.Last());
                                if (end_check == true)
                                {
                                    
                                }
                                else if (end_check==false)
                                {
                                   
                                    while (word != "begin-while")
                                    // decrease register until do!

                                    {

                                        register--;
                                        // Console.WriteLine("register: ", register.ToString());
                                        // Console.WriteLine("word: ", word);
                                        word = words[register];

                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Console.WriteLine("there is nothing on the return stack!");
                                violate = true;
                            }
                            catch (ArgumentNullException)
                            {
                                Console.WriteLine("Stack is too small!");
                                violate=true;
                            }
                            catch (FormatException) 
                            {
                                Console.WriteLine("There must be a bool check after the while!");
                                violate=true; 
                            }

                        }

                        else if (word == ";" && modes != OP_CODES.COMMENT)

                        // Exits to Interpret mode in any case!

                        {

                            modes = OP_CODES.Interpret;

                        //    Console.WriteLine("Back to interpret mode");
                            // Break out of loop statement
                            do_loop_flag = false;
                        }

                        else if (word=="\n" && modes == OP_CODES.COMMENT)
                        {
                            modes=OP_CODES.Interpret;
                        }

                        else
                        {

                            stack = oldstack.ToList();
                            Console.WriteLine(modes);
                            Console.WriteLine(do_loop_flag);
                            Console.WriteLine(word + " not recognized!");
                            modes = OP_CODES.Interpret;
                            break;

                        }

                        // Violation means invalid word, or structure and resets the stack
                        if (violate == true)
                        // breaks loop under any cicumstance
                        {
                            stack = oldstack.ToList();
                            Console.WriteLine("Something went wrong. See error message. ");
                            modes = OP_CODES.Interpret;
                            break;
                        }
                    }
                }

                // Print Stack after each word!
                  //  DoForth.Printstack(stack);
                }


            //sw.Stop();
            //Console.WriteLine("Elapsed time: "+ sw.ElapsedMilliseconds);

            if (run!=false) 
            {
                control_flow_stack.Clear();
                loop_control_stack.Clear();
                do_loop_flag = false;
                // Console.WriteLine("control stack clear");
                DoForth.Printstack(stack);

                modes = OP_CODES.Interpret;
                words.Clear();
            }
            
           
        }


    }

}



