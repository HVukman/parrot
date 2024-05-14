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


namespace Parrot { 
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
      

        ConsoleKeyInfo cki;

        var run = true;
 

        char[] delimiterChars = {' ', ',', '.', ':', '\t' };
        List<string> oldstack = [];
        //string userinput;


        // ENUMS and FLAGS
        OP_CODES modes = OP_CODES.Interpret;
        IF_THEN iF_THEN_ELSE;


        List<string> oldinputs= new List<string>();
        List<string> stack = new List<string>();
        List<List<string>> cyclestack = new List<List<string>>();

        List<bool> control_flow_stack = new List<bool>();
        List<string> control_buffer_stack = new List<string>();

        List<string>  pre_processed_words = new List<string>();
        List <string> words = new List<string>();
        string[] commands;
        List<string> command = new List<string>();      


        List<int> loop_control_stack = new List<int>();
        bool boolean_control_flow=false;


        var panel = new Panel("Hello There!");
        AnsiConsole.Write(
        new FigletText("This is Parrot")
        .LeftJustified()
        .Color(Color.Gold1));


        // Custom Words Dictionary
        Dictionary<string, List<string>> CustomWords =
            new Dictionary<string, List<string>>();

        // Custom Variables Dictionary
        Dictionary<string, string> CustomVars =
            new Dictionary<string, string>();

        bool do_loop_flag = false;
        bool while_flag = false;

        BigInteger allot = 0;

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
                
                if (word[word.Length()-1]==';')
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

                    
                    var violate = false;
                    // register = 0;

                    if (userinput.ToLower() == "bye")
                    {
                        run = false;
                    }



                    for (int register = 0; register < input_length; register++)
                        
                    {
                        if (violate == false) 
                        {
                            string word = words[register];
                            

                            (violate, stack, CustomVars, modes, CustomWords, control_flow_stack,
                                    control_buffer_stack, register, do_loop_flag, while_flag, boolean_control_flow, allot) =
                                 ParseStack.Main(stack,
                                oldstack, word, modes, CustomVars,
                                CustomWords, control_flow_stack, control_buffer_stack, loop_control_stack,
                                boolean_control_flow, do_loop_flag, while_flag, register, run, words, allot 
                               );
                        }

                        else if (violate == false)
                        {
                            break;
                        }
                        

                    }
                }
                if (run != false)
                {
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




