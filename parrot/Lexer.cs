using ForthError;
using System.Numerics;
using System.Text.RegularExpressions;
using static Parrot.Parrot;
using Parser_Namespace;
using System.Diagnostics;

namespace parrot
{
    public class Lexer
    {


        public readonly struct tokens()

        {
            public readonly string if_word = "if";
            public readonly string else_word = "else";
            public readonly string if_else_word = "if-else";
            public readonly string increment = "inc";
            public readonly string then_word = "then";
            public readonly string else_then = "else-then";
            public readonly string do_word = "do";
            public readonly string loop_word = "loop";

        }


        static private void Reset_stack(List<string> stack_fail, List<string> stack_old)
        {
            // reset stack

            stack_fail = stack_old.ToList();

        }

        static private (bool, bool, OP_CODES, int) while_check(List<string> myList, int register, bool violate, 
            bool while_flag,
            OP_CODES mode,  string word, List<string> words)
        {
            bool end_check;
            {

                try
                {

                    end_check= bool.Parse(myList.Last());
                    if (end_check == false)
                    {
                        // continue
                        while_flag = false;
                        register++;


                    }
                    else if (end_check == true)
                    {
                        mode=OP_CODES.Interpret;
                        while (word != "begin-while")
                        // decrease register until do!

                        {
                           
                            register--;
                            while_flag = true;
                            
                            // Console.WriteLine("register: ", register.ToString());
                            // Console.WriteLine("word: ", word);
                            word = words[register];

                        }
                        register++;
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
            return (violate, while_flag, mode, register);
        }

        static private (Struct_stact, int, bool, List<string>) 
            ParsingStack(Struct_stact struct_stack, string word, int register, bool run, List<string> words)
        
        {
            bool violate = false;
            tokens _tokens = new();

            List<string> standardwords = Consts_Variables.standardwords;
            string[] extrawords = Consts_Variables.extrawords;
            string[] ext_stackops = Consts_Variables.ext_stackops;
            string[] booleans = Consts_Variables.booleans;
            string[] mult_add_wors = Consts_Variables.mult_add_commands;
            string[] var = Consts_Variables.var;
            string[] func = Consts_Variables.func;

            string increment = _tokens.increment;
            string if_string = _tokens.if_word;
            string then_string = _tokens.then_word;
            string do_string = _tokens.do_word;
            string loop_string = _tokens.loop_word;
            string else_word = _tokens.else_word;

            bool parseint = BigInteger.TryParse(word, out BigInteger number);
            // pattern for strings
            string pattern = @"^""(([a-zA-Z!?*-_-§$%]+|\d+|)\s*)*""";


            var modes = struct_stack.modes;
            var CustomWords=struct_stack.CustomWords;
            var CustomVars = struct_stack.CustomVars;
            var stack = struct_stack.stack;
            var oldstack = struct_stack.oldstack;
            var do_loop_flag = struct_stack.do_loop_flag;
            var while_flag = struct_stack.while_flag;
            var control_buffer_stack = struct_stack.control_buffer_stack;
            var control_flow_stack = struct_stack.control_flow_stack;
            var boolean_control_flow = struct_stack.boolean_control_flow;
            var loop_control_stack = struct_stack.loop_control_stack;
            var allot = struct_stack.allot;

            var _parser = new Parser();

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
                    int customs_size = _customs.Count();
                    int offset_register = 0;
                    foreach (string customword in _customs)
                    {
                    // do each word in customword
                        if (customword!="")
                            {
                                //DEBUG CUSTOMWORD
                                //Console.WriteLine(customword);
                                // dont increase register
                                word = customword;
                                
                                (struct_stack, register, violate, words )
                                       = ParsingStack(struct_stack, word, register, run, words);


                        }
                    
                        }
                    // reset register +1 to escape loop
                    register = register - customs_size +1 ;
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("general exception");
                    violate = true;
                }
            }


            else if (((word == if_string) || mult_add_wors.Contains(word) || standardwords.Contains(word) || extrawords.Contains(word) || ext_stackops.Contains(word) || (BigInteger.TryParse(word, out BigInteger number5)) || Regex.IsMatch(word, pattern))
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
                        (struct_stack, violate) = Parser.Main(struct_stack, word, violate);

                        register++;
                    }

                    if (while_flag == true)
                    {
                        //struct_stack.stack = control_buffer_stack;
                        (struct_stack, violate) = Parser.Main(struct_stack, word, violate);
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

            else if (modes == OP_CODES.IF_Mode && word != then_string)
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
                        while (word != loop_string)
                        {
                            register += register;
                            word = words[register];

                        }

                    }

                    else if (do_loop_flag == true && word == loop_string && loop_control_stack.Length() > 1)

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
                                (BigInteger.TryParse(word, out BigInteger number4)) || standardwords.Contains(word)))

                    {
                        (struct_stack, violate) = Parser.Main(struct_stack, word, violate);
                        register++;
                    }
                      
                     else if (word=="end-while")
                    
                    {

                        (violate, while_flag, modes, register) = while_check(control_buffer_stack, register, violate, 
                                    while_flag, modes, word, words);
                    
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


            else if (modes == OP_CODES.IF_Mode && word == then_string)
            {
                // when in if mode and then
                // switch modes to if_then_mode
                //modes = OP_CODES.IF_THEN_Mode;

                // PRINT STATEMENT
                //  Console.WriteLine("switched to if then");

                modes = OP_CODES.IF_THEN_Mode;
                try
                {
                    control_flow_stack.Add(bool.Parse(control_buffer_stack[control_buffer_stack.Count - 1]));

                    control_buffer_stack.RemoveAt(control_buffer_stack.Count - 1);
                    boolean_control_flow = control_flow_stack[control_flow_stack.Count - 1];
                    // bool ending_control_buffer_stack = bool.Parse(ending);
                    // access top of control buffer
                    control_flow_stack.RemoveAt(control_flow_stack.Count -1);
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

                }

                catch (FormatException)
                {
                    Console.WriteLine("there was no comparison made at the end");
                    violate = true;
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
                                
                                struct_stack.stack.Add(loop_i.ToString());
                                register++;
                                //  Console.Write("index i: ", loop_i);
                            }

                            else if ((mult_add_wors.Contains(word) || ext_stackops.Contains(word) || extrawords.Contains(word) || Regex.IsMatch(word, pattern) || 
                                (BigInteger.TryParse(word, out BigInteger number3)) || standardwords.Contains(word)) && word!=";")
                                
                            {

                                if (while_flag == true)
                                {
                                    struct_stack.stack = control_buffer_stack;
                                    (struct_stack, violate) = Parser.Main(struct_stack, word, violate);
                                   
                                    register++;
                                }

                                else {

                                    (struct_stack, violate) = Parser.Main(struct_stack, word, violate);
                                    register++;

                                }
                                
                            }

                            else if (word == else_word)
                            {
                                boolean_control_flow = false;
                                register++;

                            }

                            else if (word == ";" || word == "\n")
                            {

                                modes = OP_CODES.Interpret;
                                register++;
                            }

                            else if (word== loop_string && do_loop_flag==true)
                            {
                                while (word != do_string)
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

                            //  Console.WriteLine(word);
                            if (word != else_word && word!=";" && word!="\n")
                            {
                                register++;
                                //Console.WriteLine("do nothing");
                            }

                            else if (word == else_word)
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

            else if (word == do_string && modes != OP_CODES.COMMENT)
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
                            var stack_length = struct_stack.stack.Count;
                            struct_stack.stack.RemoveAt(stack_length-1);
                           
                            stack_length = struct_stack.stack.Count;
                            struct_stack.stack.RemoveAt(stack_length - 1);
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

            else if (word == loop_string && modes != OP_CODES.COMMENT)

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

                        while (word != do_string)
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
                while_flag = true;
                register++;
            }

            else if (word == "while")
            {
                
                if (while_flag == false)
                {
                    //while_flag = true;
                    // control_buffer_stack = stack.ToList();
                    
                    Console.WriteLine("while only in while loop!");
                    violate= true;
                }

                if (while_flag == true)
                {
                    control_buffer_stack=stack.ToList();
                    modes = OP_CODES.IF_Mode;
                    register++;
                    //Console.WriteLine("Only one while!");
                    // violate=true;

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

                struct_stack.stack = oldstack.ToList();
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
                struct_stack.stack = oldstack.ToList();
                Console.WriteLine("Something went wrong. See error message. ");
                modes = OP_CODES.Interpret;
            }


            struct_stack.modes=modes;
            
            
           
            struct_stack.oldstack = oldstack;
            struct_stack.do_loop_flag= do_loop_flag;
            struct_stack.while_flag= while_flag;
            struct_stack.control_buffer_stack= control_buffer_stack;
            struct_stack.control_flow_stack= control_flow_stack;
            struct_stack.boolean_control_flow=boolean_control_flow;
            struct_stack.loop_control_stack=loop_control_stack;

            return (struct_stack, register, violate, words);
        }

        
        
        
        public static (Struct_stact, int, bool, List<string>) 
            Main(Struct_stact struct_stack, string word, int register, bool violate, List<string> words)
            
            
        {
            var CustomWords = struct_stack.CustomWords;


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
                    
                    struct_stack.CustomWords = CustomWords;
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
                        (struct_stack, register, violate, words)
                                           = ParsingStack(struct_stack, word, register, violate, words);

                    }

                }
                catch (IndexOutOfRangeException) {
                    if (word == "")
                    {
                        Console.WriteLine("empty");
                       // Console.WriteLine(words[register]);
                    }
                    Console.WriteLine("index out of range at " + word);
                    
                    Console.WriteLine(struct_stack.modes.ToString());
                    Console.WriteLine(register);
                    //Console.WriteLine(word);
                    violate = true;
                
                }

            return (struct_stack, register, violate, words);
        }
    }
}
