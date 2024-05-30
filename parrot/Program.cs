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
using LanguageExt;
using static Parrot.Parrot;
using Parser_Namespace;

namespace Parrot { 
public class Parrot
{

    public struct Struct_stact()

    {

            public List<string> stack { get; set; }
            public List<string> oldstack { get; set; }
            public List<string> storedstack { get; set; }
            public OP_CODES modes { get; set; }
            public Dictionary<string, string> CustomVars { get; set; }
            public Dictionary<string, List<string>> CustomWords { get; set; }
            public List<bool> control_flow_stack { get; set; }
            public List<string> control_buffer_stack { get; set; }
            public List<int> loop_control_stack { get; set; }
            public bool do_loop_flag { get; set; }
            public bool while_flag { get; set; }
            public bool boolean_control_flow { get; set; }
            public bigint allot { get; set; }

            public void Init()
            {
                stack = new List<string>();
                oldstack = new List<string>();
                storedstack = new List<string>();
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
                allot = 0;
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


        List<string> oldinputs= new List<string>();
       

        // ?? List<List<string>> cyclestack = new List<List<string>>();

        List<string> pre_processed_words = new List<string>();
        List <string> words = new List<string>();
        List<string> commands= new List<string>(); ;
        List<string> command = new List<string>();

        OP_CODES modes = stact.modes;
        var panel = new Panel("Hello There!");
        AnsiConsole.Write(
        new FigletText("This is Parrot")
        .LeftJustified()
        .Color(Color.Gold1));

        // system settings
            Dictionary<string, string> Systemvars =
                    new Dictionary<string, string>();

        int register = 0;

        // MAIN LOOP
        while (run)
   
        {

            // Type instruction in REPL
            string userinput = AnsiConsole.Ask<string>("What's your [gold1]input[/]?");
            oldinputs.Add(userinput);
            userinput = userinput.Trim();
       

                // regex for strings like "hello world"
            commands = Regex.Matches(userinput, @"\""(\""\""|[^\""])+\""|[^ ]+",
                RegexOptions.ExplicitCapture)
                  .Cast<Match>()
                  .Select(m => m.Value)
                  .ToList();


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

                            (stact, register, violate, words) = Lexer.Main(stact, word, register, violate, words);
  
                             

                     //   Console.WriteLine("register " + modes.ToString());
                        }

                        else if (violate == true)
                        {
                            modes = OP_CODES.Interpret;
                            stact.control_buffer_stack.Clear();
                            stact.loop_control_stack.Clear();
                            stact.do_loop_flag = false;
                            stact.while_flag = false;
                            register = 0;
                            break;
                            

                        }

                    }
                }

                if (run != false)
                {
                    register = 0;
                    stact.control_flow_stack.Clear();
                    stact.loop_control_stack.Clear();
                    stact.do_loop_flag = false;
                    stact.while_flag = false;                  
                    stact.control_buffer_stack.Clear();
                    // Console.WriteLine("control stack clear");
                    if (userinput != "echo")
                    {
                        Parser.Printstack(stact.stack);
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




