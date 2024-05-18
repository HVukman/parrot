using Do_forth;
using ForthError;
using LanguageExt.ClassInstances;
using Microsoft.Win32;
using SixLabors.ImageSharp.PixelFormats;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Parrot.Parrot;

namespace parrot
{
    public class ParseStack
    {

        static private void Reset_stack(List<string> stack_fail, List<string> stack_old)
        {
            // reset stack

            stack_fail = stack_old.ToList();

        }

        static private (bool, List<string>, Dictionary<string,string>, OP_CODES, Dictionary<string, List<string>>, List<bool>,
        List<string>, int , bool, bool, bool, BigInteger) ParsingStack(List<string> stack,
            List<string> oldstack, string word, OP_CODES modes, Dictionary<string, string> CustomVars,
            Dictionary<string, List<string>> CustomWords, List<bool> control_flow_stack, List<string> control_buffer_stack, List<int> loop_control_stack,
            bool do_loop_flag, bool while_flag, int register, bool run, List<string> words, BigInteger allot, bool boolean_control_flow)
        
        {
            bool violate = false;

            List<string> standardwords = Consts_Variables.standardwords;
            string[] extrawords = Consts_Variables.extrawords;
            string[] ext_stackops = Consts_Variables.ext_stackops;
            string[] booleans = Consts_Variables.booleans;
            string[] mult_add_wors = Consts_Variables.mult_add_commands;
            string[] var = Consts_Variables.var;
            string[] func = Consts_Variables.func;
            string increment = "inc";
            string if_string = IF_THEN_STRING.IF;
            string pattern2 = @"^""^[a-zA-Z]+(?:\s +[a -zA -Z]+)+""$";
            string pattern = @"^""[ *\w\s!:\(\) *]+""$";



            DoForth parser = new DoForth();
            // if word 0 and 1 == /
            // then comment mode

            if (word[0] == '/' && modes != OP_CODES.COMMENT) {

                if (word.Length()>1 && word[1] == '/')
                {
                    modes = OP_CODES.COMMENT;
                }
                register++;
            }
                 
            
            else if (word == "")
            {
                // ignore whitespace
                Console.WriteLine("empty here");
                register++;
            }
            

            else if (modes == OP_CODES.COMMENT && word != "\n")
            {
                // Do nothing
                // Comment mode
                register++;
            }

            else if (modes == OP_CODES.Compile_Word)
            {


            }

            else if (CustomWords.ContainsKey(word))
            // if word is custom function calls itself
                {
                    string command;

                try {
                   
                    // DEBUG CUSTOM WORD
                    // Console.WriteLine("custom word " + word);
                    // 
                    List<string> _customs= CustomWords[word];

                    
                    foreach (string customword in _customs)
                    {
                    if (customword!="")
                        {
                            //DEBUG CUSTOMWORD
                            //Console.WriteLine(customword);
                            // dont increase register
                            command = customword;

                            (violate, stack, CustomVars, modes, CustomWords, control_flow_stack,
                                    control_buffer_stack, register, do_loop_flag, while_flag, boolean_control_flow, allot) = 
                                    ParsingStack(stack,
                                oldstack, command, modes, CustomVars,
                                CustomWords, control_flow_stack, control_buffer_stack, loop_control_stack,
                                do_loop_flag, while_flag, register, run, words, allot, boolean_control_flow);
                            
                        }
                    }
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("general exception");
                    violate = true;
                }
            }


            else if (((word == if_string) || mult_add_wors.Contains(word) || standardwords.Contains(word) || extrawords.Contains(word) || ext_stackops.Contains(word) || (BigInteger.TryParse(word, out BigInteger number)) || Regex.IsMatch(word, pattern))
                    && (modes == OP_CODES.Interpret)
                    && (violate == false))
            {
                // Exit program!



                if ((mult_add_wors.Contains(word) || ext_stackops.Contains(word) || extrawords.Contains(word) || Regex.IsMatch(word, pattern) || (BigInteger.TryParse(word, out BigInteger number2)) || standardwords.Contains(word))
                        )
                {
                    
                    // standard do words
                    //Console.WriteLine((int.TryParse(word, out int number3)));
                    if (while_flag == false)
                    {
                        (stack, control_flow_stack, violate, CustomVars, CustomWords, allot, modes) = parser.Main(stack, control_flow_stack, word.ToLower(),
                                                 modes, do_loop_flag, loop_control_stack, CustomVars, CustomWords, violate, allot);
                        register++;
                    }

                    if (while_flag == true)
                    {
                        (control_buffer_stack, control_flow_stack, violate, CustomVars, CustomWords, allot, modes) = parser.Main(control_buffer_stack, control_flow_stack, word.ToLower(),
                                                     modes, do_loop_flag, loop_control_stack, CustomVars, CustomWords, violate, allot);
                        register++;
                    }
                    // PRINT STATEMENT
                    // Console.WriteLine("register: " + register.ToString());
                }


                else if (word == if_string)
                {
                    // when enter if mode with "If", switch stack with control buffer
                    control_buffer_stack.Clear();
                    control_buffer_stack = stack.ToList();
                    // enter if_mode
                    // Leave this loop
                    modes = OP_CODES.IF_Mode;
                    register++;
                    // PRINT STATEMENT
                    //Console.WriteLine("Switch stack");
                    // Console.WriteLine(modes.ToString());

                }

            }

            else if (modes == OP_CODES.IF_Mode && word != "then")
            {
                
                try
                {
                    if (do_loop_flag == true && word == increment && loop_control_stack.Length() > 1)
                    // Use loop index while in loop
                    // Length check for own safety

                    {
                        int loop_i = loop_control_stack[loop_control_stack.Length() - 2];
                        control_buffer_stack.Add(loop_i.ToString());
                        register++;
                        //                    Console.Write("index i: ", loop_i);
                    }

                    else if (do_loop_flag == true && word == "break" && loop_control_stack.Length() > 1)
                    // Break loop

                    {
                        do_loop_flag = false;
                        while (word != "loop")
                        {
                            register += register;
                            word = words[register];

                        }

                    }

                    else if (do_loop_flag == true && word == "loop" && loop_control_stack.Length() > 1)

                    {
                        // Go back
                        if (loop_control_stack[loop_control_stack.Length() - 1] == loop_control_stack[loop_control_stack.Length() - 2])
                        {
                            do_loop_flag = false;
                        }
                        else
                        {
                            while (words[register] != "do")
                            {
                                register--;
                                word = words[register];

                            }
                        }

                    }

                    else if (word == ";")
                    {
                        modes = OP_CODES.Interpret;
                        register++;
                    }

                    else if ((mult_add_wors.Contains(word) || ext_stackops.Contains(word) || extrawords.Contains(word) || Regex.IsMatch(word, pattern) ||
                                (BigInteger.TryParse(word, out BigInteger number2)) || standardwords.Contains(word)))

                    {

                        (control_buffer_stack, control_flow_stack, violate, CustomVars, CustomWords, allot, modes) = parser.Main(control_buffer_stack, control_flow_stack, word.ToLower(),
                                                                       modes, do_loop_flag, loop_control_stack, CustomVars, CustomWords, violate, allot);

                        register++;
                    }

                    else
                    {
                        Console.WriteLine("are you here?");
                        violate = true;
                    }

                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("there is an out of range exception!");
                    violate = true;
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Index out of range in ifthenelse");
                    Console.WriteLine(do_loop_flag);
                    violate = true;
                }
                // performs action
               
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

                modes = OP_CODES.IF_THEN_Mode;
                try
                {
                    // access top of control buffer
                    boolean_control_flow = control_flow_stack[control_flow_stack.Count - 1];
                    register++;
                    // PRINT STATEMENT

                    // Console.WriteLine("boolean control flow: " + boolean_control_flow.ToString());

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

                
                
                try {
                    switch (boolean_control_flow)
                    {
                        // when flow true: do forth until else
                        case true:



                            if (do_loop_flag == true && word == increment)
                            // Use loop index while in loop
                            // Length check for own safety⌈

                            {
                                int loop_i = loop_control_stack[loop_control_stack.Length() - 2];
                                stack.Add(loop_i.ToString());
                                register++;
                                //                                      Console.Write("index i: ", loop_i);
                            }

                            else if ((mult_add_wors.Contains(word) || ext_stackops.Contains(word) || extrawords.Contains(word) || Regex.IsMatch(word, pattern) || 
                                (BigInteger.TryParse(word, out BigInteger number2)) || standardwords.Contains(word)) && word!=";")
                                
                            {


                                (violate, stack, CustomVars, modes, CustomWords, control_flow_stack,
                                         control_buffer_stack, register, do_loop_flag, while_flag, boolean_control_flow, allot)
                                         = ParsingStack(stack,
                                      oldstack, word, modes, CustomVars,
                                      CustomWords, control_flow_stack, control_buffer_stack, loop_control_stack,
                                      do_loop_flag, while_flag,
                                      register, run, words, allot, boolean_control_flow);
                                
                            }


                            else if (word == "else")
                            {
                                boolean_control_flow = false;
                                register++;

                            }

                            else if (word == ";" || word == "\n")
                            {

                                modes = OP_CODES.Interpret;
                                register++;
                            }

                            else if (word=="loop" && do_loop_flag==true)
                            {
                                while (word != "do")
                                {
                                    register--;
                                    word= words[register];

                                }
                            }

                            else
                            {
                                register++;
                            }

                            
                            break;


                        case false:

                            //             Console.WriteLine(word);
                            if (word != "else" && word!=";" && word!="\n")
                            {
                                register++;
                                //Console.WriteLine("do nothing");
                            }

                            else if (word == "else")
                            {
                                register++;
                                boolean_control_flow = true;

                            }
                            else if ((word == ";" || word == "\n"))
                            {
                                register++;
                                modes = OP_CODES.Interpret;
                            }

                            
                            break;
                    }

                }
                catch (IndexOutOfRangeException) {
                    Console.WriteLine("index out of range?");
                    Console.WriteLine(word);
                    Console.WriteLine(register);
                    violate = true;
                }
                

            }

            else if (word == "do" && modes != OP_CODES.COMMENT)
            {

                try
                {
                   

                     if (do_loop_flag == false && loop_control_stack.Count == 0)
                    {
                        // begin loop at empty control buffer stack
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
                            var stack_length = stack.Count;
                            stack.RemoveAt(stack_length-1);
                            stack_length = stack.Count;
                            stack.RemoveAt(stack_length - 1);
                            //loop_control_stack[loop_control_stack.Length() - 2] = loop_start;
                            //loop_control_stack[loop_control_stack.Length() - 1] = loop_end;

                            do_loop_flag = true;
                            register = register + 1;

                        }

                    }

                    else if (do_loop_flag == true && loop_control_stack.Count > 0)
                    {
                        register++;
                    }

                }

                catch (FormatException e)
                {
                    Console.WriteLine("Ints must be parsed! Syntax is int_end int_begin do .... loop");
                    violate = true;
                }

                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine(stack.Count);
                    Console.WriteLine("Stack is too small! Syntax is int_end int_begin do .... loop");
                    violate = true;
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Somewhere an index is wrong!");
                    violate = true;
                }

                catch (DoForthErrorException)
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
                    loop_control_stack[loop_control_stack.Length() - 2]++;
                    int end = loop_control_stack[loop_control_stack.Length() - 2];
                    int begin = loop_control_stack[loop_control_stack.Length() - 1];
                    if (end >= begin)

                    {
                        // go on in instruction and loop is false!

                        loop_control_stack.Clear();
                        do_loop_flag = false;
                        register++;
                    }

                    else
                    {

                        // if mode== if then -> then switch
                      /*  if (modes == OP_CODES.IF_THEN_Mode)
                        {
                            modes = OP_CODES.Interpret;

                        } */

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

            else if (word == "begin-while")

            {
         //       Console.WriteLine("begin while");
                while_flag = false;
                register++;
            }

            else if (word == "while")
            {
                if (while_flag == false)
                {
                    while_flag = true;
                    control_buffer_stack = stack.ToList();
                    register++;
                    // Console.WriteLine("while only in while loop!");
                    // violate= true;
                }

                if (while_flag == true)
                {
                    register++;
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
                        // continue
                        while_flag = false;
                        register++;

                    }
                    else if (end_check == false)
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
                    violate = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine("There must be a bool check after the while!");
                    violate = true;
                }
                
            }

            else if (word == ";" && modes != OP_CODES.COMMENT)

            // Exits to Interpret mode in any case!

            {

                modes = OP_CODES.Interpret;
                
                //    Console.WriteLine("Back to interpret mode");
                // Break out of loop statement
                do_loop_flag = false;
                register++;
            }

            else if (word == "\n" && modes == OP_CODES.COMMENT)
            {
                modes = OP_CODES.Interpret;
                register++;
            }

            else
            {

                stack = oldstack.ToList();
                Console.WriteLine(modes);
                Console.WriteLine(do_loop_flag);
                Console.WriteLine(word + " not recognized!");
                modes = OP_CODES.Interpret;
                violate = true;
            }

            // Violation means invalid word, or structure and resets the stack
            if (violate == true)
            // breaks loop under any cicumstance
            {
                stack = oldstack.ToList();
                Console.WriteLine("Something went wrong. See error message. ");
                modes = OP_CODES.Interpret;
            }




            return (violate, stack, CustomVars, modes, CustomWords, control_flow_stack,
                                   control_buffer_stack, register, do_loop_flag, while_flag, boolean_control_flow, allot);
        }

        
        
        
        public static (bool, List<string>, Dictionary<string, string>, OP_CODES,
            Dictionary<string, List<string>>, List<bool>, List<string>, int, bool, bool, bool, BigInteger) Main(List<string> stack,
            List<string> oldstack, string word, OP_CODES modes, Dictionary<string, string> CustomVars,
            Dictionary<string, List<string>> CustomWords, List<bool> control_flow_stack, 
            List<string> control_buffer_stack, List<int> loop_control_stack,
            bool boolean_control_flow, bool do_loop_flag, bool while_flag, int register, bool run, List<string> words,
            BigInteger allot
            )
        
        {
            bool violate=false;

            if (word == "define")
            {
                try { 
                    register += 1;
                    string name = words[register];
                    List<string> value = new List<string>();
                    List<string> buffer = new List<string> ();
                    CustomWords.Add(name, value);
                    register += 1;
                    while (words[register] != ";" || words[register] == "\n")
                    {
                        if (words[register] != " " && words[register] != "")  
                        {
                            value.Add(words[register]);
                            buffer.Add(words[register]);
                        }
                        
                        register += 1;
                    }
                    Console.WriteLine("defined " + name + " as:");
                    foreach (var defword in buffer)
                    {
                        Console.Write(defword + " ");
                    }
                    Console.Write("\n");
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Stack is too small! Syntax is: define name (definition here) ;");
                    violate = true;
                }
                catch (ArgumentNullException)
                {
                    Console.WriteLine("Argument is null, somehow!");
                    violate = true;
                }
                
            }

            // if not compile do this!
            else

                try {
                    {
                        (violate, stack, CustomVars, modes, CustomWords, control_flow_stack,
                                           control_buffer_stack, register, do_loop_flag, while_flag, boolean_control_flow, allot)
                                           = ParsingStack(stack,
                                        oldstack, word, modes, CustomVars,
                                        CustomWords, control_flow_stack, control_buffer_stack, loop_control_stack,
                                        do_loop_flag, while_flag,
                                        register, run, words, allot, boolean_control_flow);

                    }

                }
                catch (IndexOutOfRangeException) {
                    if (word == "")
                    {
                        Console.WriteLine("empty");
                       // Console.WriteLine(words[register]);
                    }
                    Console.WriteLine("index out of range at " + word);
                    
                    Console.WriteLine(modes.ToString());
                    Console.WriteLine(register);
                    //Console.WriteLine(word);
                    violate = true;
                
                }

            return (violate, stack, CustomVars, modes, CustomWords, control_flow_stack,
                                   control_buffer_stack, register, do_loop_flag, while_flag, boolean_control_flow, allot);
        }
    }
}
