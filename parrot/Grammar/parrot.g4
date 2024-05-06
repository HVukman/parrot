grammar parrot;

// Lexer rules
WS : [ \t\r\n]+ -> skip;

NUMBER : '-'? ('0'..'9')+ ('.' ('0'..'9')+)?;

WORD : (~[ \t\r\n])+;

// Parser rules
program : (statement | comment)* EOF;

statement : word | number | comment;

word : WORD;

number : NUMBER;

expression : (word | number)+; // New expression rule

comment : '(' ~('\n'|'\r')* '\r'? '\n'?;
