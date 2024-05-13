

    // storing essential constants
    public static class Consts_Variables
    {
        public static string[] extrawords = ["writedump", "loaddump","today"];
        public static string[] var = ["var"];
        public static string[] func = ["func"];

        public static List<string> standardwords = ["bye", "+", "add", "*", "mul", "-", "sub", "/", "div", "drop", "dup", "swap", "rot", "over", "nip", "clear",
            "echo", "rev", "curry", "dump", "=", "<", ">", "!=","inc","store", "fetch" , 
            "define", "compile", "peek" ,"print", "allot","?vars","?consts","?funcs"];   
       
        //public static string[] standardwords = ["bye", "+", "add", "*", "mul", "-", "sub", "/", "div", "drop", "dup", "swap", "rot", "over", "nip", "clear", 
         //   "echo", "rev", "curry", "dump", "=", "<", ">", "!=","inc","store", "fetch"];
        public static string[] mult_add_commands = ["+", "-", "*", "/", "%"];
        public static string[] comparisons = ["=", "<", ">", "!="];
        public static string[] booleans = ["if", "else", "then"];
        public static string[] standard_stackops = ["drop", "dup", "swap", "rot", "over", "nip", "clear", "rev", "fetch", "peek" , "print"];
        public static string[] ext_stackops = ["sum", "prod"];
        public static string[] do_loop = ["do", "loop"];
        public static string[] definitions = ["store", "define"];
        public static string[] while_loop = ["begin-while", "end-while"];
        public static string[] comment = ["//"];
        public static string[] reserved;
        
        //public static int[] combinedArray = new int[extrawords.Length + var_func.Length+ standardwords.Length + mult_add_commands.Length + comparisons.Length+booleans.Length+standard_stackops.Length+ext_stackops.Length+do_loop.Length+comment.Length];
        //Buffer.BlockCopy();
    }

   

