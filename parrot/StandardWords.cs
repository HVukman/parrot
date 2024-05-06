using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
public sealed class StandardWords
{
    private static readonly Lazy<StandardWords> Lazy =
        new Lazy<StandardWords>(() => new StandardWords());

    /// <summary>
    /// Access point to methods and properties
    /// </summary>
    public static StandardWords Instance => Lazy.Value;
    
    string[] standardwords = ["+", "add", "*", "mul", "-", "sub", "/", "div", "drop", "dup", "swap", "rot", "over", "nip", "clear", "echo"];
}