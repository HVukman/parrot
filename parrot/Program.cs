// See https://aka.ms/new-console-template for more information



using System.Text.RegularExpressions;
using ForthError;
using Spectre.Console;
using Do_forth;
using System.Numerics;
using System.Diagnostics;
using LanguageExt.TypeClasses;
using Microsoft.Win32;
using LanguageExt.ClassInstances;
using LanguageExt.ClassInstances.Pred;
using parrot;
using static Microsoft.FSharp.Core.ByRefKinds;


namespace Parrot { 
public class Parrot
{

    public struct Struct_stact()

    {

            public List<string> stack { get; set; }
            public List<string> oldstack { get; set; }
            public OP_CODES modes { get; set; }
            public Dictionary<string, string> CustomVars { get; set; }
            public Dictionary<string, List<string>> CustomWords { get; set; }
            public List<bool> control_flow_stack { get; set; }
            public List<string> control_buffer_stack { get; set; }
            public List<int> loop_control_stack { get; set; }
            public bool do_loop_flag { get; set; }
            public bool while_flag { get; set; }
            public bool boolean_control_flow { get; set; }

            public void Init()
            {
                stack = new List<string>();
                oldstack = new List<string>();
                control_flow_stack = new List<bool>();
                control_buffer_stack = new List<string>();
                modes = OP_CODES.Interpret;
                CustomVars = new Dictionary<string, string>();
                CustomWords = new Dictionary<string, List<string>>();
                control_flow_stack = new List<bool>();
                control_buffer_stack = new List<string>();
                loop_control_stack = new List<int>();
                do_loop_flag = false;
                while_flag = false;
                boolean_control_flow = false;
            }

            public void Reset()
            {
                stack = new List<string>();
                
            }

    }

     public struct Variable()

        {
            string name { get; set; }
            string value { get; set; }
            Type type { get; set; }
        }

     public struct Word()
        {
            string name { get; set; }

            List<string> definition { get; set; }
        }


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
      

        ConsoleKeyInfo cki;

        var run = true;

        Struct_stact stact= new Struct_stact(); 
        

        stact.Init();

        char[] delimiterChars = {' ', ',', '.', ':', '\t' };
        List<string> oldstack = stact.oldstack;
            //string userinput;


            // ENUMS and FLAGS
        OP_CODES modes = stact.modes;
        IF_THEN iF_THEN_ELSE;


        List<string> oldinputs= new List<string>();
        List<string> stack = stact.stack;

        List<List<string>> cyclestack = new List<List<string>>();

        List<bool> control_flow_stack = stact.control_flow_stack;
        List<string> control_buffer_stack = stact.control_buffer_stack;

        List<string> pre_processed_words = new List<string>();
        List <string> words = new List<string>();
        List<string> commands= new List<string>(); ;
        List<string> command = new List<string>();


        List<int> loop_control_stack = stact.loop_control_stack;
        bool boolean_control_flow = stact.boolean_control_flow;


        var panel = new Panel("Hello There!");
        AnsiConsole.Write(
        new FigletText("This is Parrot")
        .LeftJustified()
        .Color(Color.Gold1));


        // Custom Words Dictionary
        Dictionary<string, List<string>> CustomWords =
                stact.CustomWords;

            // Custom Variables Dictionary
        Dictionary<string, string> CustomVars =
                stact.CustomVars;

        // system settings
            Dictionary<string, string> Systemvars =
                    new Dictionary<string, string>();

        bool do_loop_flag = stact.do_loop_flag;
        bool while_flag = stact.while_flag;

        BigInteger allot = 0;
        int register = 0;
        while (run)
        
            
        {

            // Type instruction in REPL
            string userinput = AnsiConsole.Ask<string>("What's your [gold1]input[/]?");
            oldinputs.Add(userinput);
            userinput = userinput.Trim();
                // Console.WriteLine("Enter Instruction ->");

                // Create a string variable and get user input from the keyboard and store it in the variable


                // var userinput = Console.ReadLine().Trim();


                //commands = userinput.Split(delimiterChars);



                commands = Regex.Matches(userinput, @"\""(\""\""|[^\""])+\""|[^ ]+",
                RegexOptions.ExplicitCapture)
                  .Cast<Match>()
                  .Select(m => m.Value)
                  .ToList();


                //pre_processed_words = commands.ToList();

                // words = commands.ToList();
                //string pattern = @"^""[\w\s!:\(\)]+""$";

                string semicolon_pattern =@"(?<=\w)(?=;)";


            // Separate seimicolon from word
            
            foreach (string word in commands) {
                
                if (word[word.Length()-1]==';' && word!=";")
                {
                    string[] semicolon_strings = [word.Substring(0, word.Length - 1), word.Substring(word.Length - 1,1)];
                    foreach (var semicolon_string in semicolon_strings)
                    {
                        
                        words.Add(semicolon_string.Trim().ToLower());
                        // Console.WriteLine(semicolon_string);
                    }
                }
                else if (word=="" || word==" ")
                {
                    continue;
                }
                
                else
                {
                    words.Add (word.Trim());
                }
                
            }

            int input_length= words.Count();
            commands.Clear();
            Console.WriteLine("input length: " + input_length.ToString());

                /*
                Stopwatch sw;
                sw = Stopwatch.StartNew();
                */


                if (userinput != null)
                {
                    

                    
                    var violate = false;
                    // register = 0;

                    if (userinput.ToLower() == "bye")
                    {
                        run = false;
                    }



                    while (register < words.Length())
                        
                    {
                        if (violate == false) 
                        {
                            string word = words[register];
                            word = word.ToLower();
                           

                            (violate, stack, CustomVars, modes, CustomWords, control_flow_stack,
                                    control_buffer_stack, register, do_loop_flag, while_flag, boolean_control_flow, allot) =
                                 ParseStack.Main(stack,
                                oldstack, word, modes, CustomVars,
                                CustomWords, control_flow_stack, control_buffer_stack, loop_control_stack,
                                boolean_control_flow, do_loop_flag, while_flag, register, run, words, allot 
                               );

                     //   Console.WriteLine("register " + modes.ToString());
                        }

                        else if (violate == true)
                        {
                            modes = OP_CODES.Interpret;
                            control_buffer_stack.Clear();
                            loop_control_stack.Clear();
                            do_loop_flag = false;
                            while_flag = false;
                            register = 0;
                            break;
                            

                        }
                        

                    }
                }

                if (run != false)
                {
                    register = 0;
                    control_flow_stack.Clear();
                    loop_control_stack.Clear();
                    do_loop_flag = false;
                    while_flag = false;                  
                    control_buffer_stack.Clear();
                    // Console.WriteLine("control stack clear");
                    if (userinput != "echo")
                    {
                        DoForth.Printstack(stack);
                    }


                    modes = OP_CODES.Interpret;
                    words.Clear();
                }


                // Print Stack after each word!
                // DoForth.Printstack(stack);
            }

            //sw.Stop();
            //Console.WriteLine("Elapsed time: "+ sw.ElapsedMilliseconds);

        }
    }

}




